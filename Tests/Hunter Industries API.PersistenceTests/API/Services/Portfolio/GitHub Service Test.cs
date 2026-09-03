// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Portfolio
{
    [TestClass]
    [DoNotParallelize]
    public class GitHubServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly IFileSystem _FileSystem = new FileSystemWrapper();

        /// <summary>
        /// Creates the test database and schema.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            (_ConnectionString, _DatabaseName) = LocalDbTestHelper.CreateDatabase();
            LocalDbTestHelper.CreateSchema(_ConnectionString);
            _SqlFilesPath = LocalDbTestHelper.GetSqlFilesPath();
            CreateGitHubTables(_ConnectionString);
        }

        /// <summary>
        /// Drops the test database.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            LocalDbTestHelper.DropDatabase(_DatabaseName);
        }

        /// <summary>
        /// Clears the test data between tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            LocalDbTestHelper.ClearData(_ConnectionString);
            ClearGitHubData(_ConnectionString);
        }

        /// <summary>
        /// Creates the GitHub-specific tables in the test database.
        /// </summary>
        private static void CreateGitHubTables(string connectionString)
        {
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Repository')
                    CREATE TABLE [dbo].[Repository](
                        [RepositoryId] [int] IDENTITY(1,1) NOT NULL,
                        [Name] [varchar](255) NOT NULL,
                        CONSTRAINT [PK_Repository] PRIMARY KEY CLUSTERED ([RepositoryId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow')
                    CREATE TABLE [dbo].[Workflow](
                        [WorkflowId] [int] IDENTITY(1,1) NOT NULL,
                        [Name] [varchar](255) NOT NULL,
                        CONSTRAINT [PK_Workflow] PRIMARY KEY CLUSTERED ([WorkflowId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Status')
                    CREATE TABLE [dbo].[Status](
                        [StatusId] [int] IDENTITY(1,1) NOT NULL,
                        [Value] [varchar](50) NOT NULL,
                        CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([StatusId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Type')
                    CREATE TABLE [dbo].[Type](
                        [TypeId] [int] IDENTITY(1,1) NOT NULL,
                        [Value] [varchar](50) NOT NULL,
                        CONSTRAINT [PK_Type] PRIMARY KEY CLUSTERED ([TypeId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkflowRun')
                    CREATE TABLE [dbo].[WorkflowRun](
                        [WorkflowRunId] [int] IDENTITY(1,1) NOT NULL,
                        [RepositoryId] [int] NOT NULL,
                        [WorkflowId] [int] NOT NULL,
                        [ConclusionId] [int] NOT NULL,
                        [RunNumber] [int] NOT NULL,
                        CONSTRAINT [PK_WorkflowRun] PRIMARY KEY CLUSTERED ([WorkflowRunId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Issue')
                    CREATE TABLE [dbo].[Issue](
                        [IssueId] [int] IDENTITY(1,1) NOT NULL,
                        [RepositoryId] [int] NOT NULL,
                        [StatusId] [int] NOT NULL,
                        [TypeId] [int] NOT NULL,
                        [AssigneeId] [int] NOT NULL,
                        [Number] [int] NOT NULL,
                        [Title] [varchar](255) NOT NULL,
                        CONSTRAINT [PK_Issue] PRIMARY KEY CLUSTERED ([IssueId] ASC)
                    );

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[User]') AND name = 'Username')
                    ALTER TABLE [dbo].[User] ADD [Username] [varchar](255) NULL;",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Clears all data from the GitHub-specific tables.
        /// </summary>
        private static void ClearGitHubData(string connectionString)
        {
            string[] tables =
            [
                "WorkflowRun",
                "Issue",
                "Workflow",
                "Repository",
                "[Status]",
                "[Type]"
            ];

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                foreach (string table in tables)
                {
                    string tableName = table.StartsWith("[") ? table : $"[{table}]";

                    using (SqlCommand cmd = new(
                        $"DELETE FROM {tableName}",
                        conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private GitHubService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.GitHubConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            GitHubService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts CI status test data into the database.
        /// </summary>
        private void InsertCIStatusData(
            string repository,
            string workflow,
            string status,
            int runNumber)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Repository WHERE [Name] = @repo) " +
                    "INSERT INTO Repository ([Name]) VALUES (@repo); " +
                    "IF NOT EXISTS (SELECT 1 FROM Workflow WHERE [Name] = @workflow) " +
                    "INSERT INTO Workflow ([Name]) VALUES (@workflow); " +
                    "IF NOT EXISTS (SELECT 1 FROM [Status] WHERE [Value] = @status) " +
                    "INSERT INTO [Status] ([Value]) VALUES (@status); " +
                    "INSERT INTO WorkflowRun (RepositoryId, WorkflowId, ConclusionId, RunNumber) " +
                    "SELECT R.RepositoryId, W.WorkflowId, S.StatusId, @runNumber " +
                    "FROM Repository R, Workflow W, [Status] S " +
                    "WHERE R.[Name] = @repo AND W.[Name] = @workflow AND S.[Value] = @status;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@repo",
                        repository);
                    cmd.Parameters.AddWithValue(
                        "@workflow",
                        workflow);
                    cmd.Parameters.AddWithValue(
                        "@status",
                        status);
                    cmd.Parameters.AddWithValue(
                        "@runNumber",
                        runNumber);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts issue test data into the database.
        /// </summary>
        private void InsertIssueData(
            string repository,
            string status,
            string type,
            string username,
            int number,
            string title)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Repository WHERE [Name] = @repo) " +
                    "INSERT INTO Repository ([Name]) VALUES (@repo); " +
                    "IF NOT EXISTS (SELECT 1 FROM [Status] WHERE [Value] = @status) " +
                    "INSERT INTO [Status] ([Value]) VALUES (@status); " +
                    "IF NOT EXISTS (SELECT 1 FROM [Type] WHERE [Value] = @type) " +
                    "INSERT INTO [Type] ([Value]) VALUES (@type); " +
                    "IF NOT EXISTS (SELECT 1 FROM [User] WHERE Username = @username) " +
                    "INSERT INTO [User] ([Name], Username) VALUES (@username, @username); " +
                    "INSERT INTO Issue (RepositoryId, StatusId, TypeId, AssigneeId, Number, Title) " +
                    "SELECT R.RepositoryId, S.StatusId, T.TypeId, U.UserId, @number, @title " +
                    "FROM Repository R, [Status] S, [Type] T, [User] U " +
                    "WHERE R.[Name] = @repo AND S.[Value] = @status AND T.[Value] = @type AND U.Username = @username;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@repo",
                        repository);
                    cmd.Parameters.AddWithValue(
                        "@status",
                        status);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@number",
                        number);
                    cmd.Parameters.AddWithValue(
                        "@title",
                        title);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetCIStatus method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetCIStatus()
        {
            InsertCIStatusData(
                "TestRepo",
                "Build",
                "Passing",
                1);
            InsertCIStatusData(
                "TestRepo",
                "Tests",
                "Passing",
                1);

            GitHubService service = CreateService();

            List<GitHubCIStatusRecord> actual = await service.GetCIStatus("TestRepo");

            Assert.AreEqual(
                2,
                actual.Count);
            Assert.IsTrue(
                actual.Exists(r => r.Workflow == "Build" && r.Status == "Passing"));
            Assert.IsTrue(
                actual.Exists(r => r.Workflow == "Tests" && r.Status == "Passing"));
        }

        /// <summary>
        /// Checks whether the GetCIStatus method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetCIStatusEmpty()
        {
            GitHubService service = CreateService();

            List<GitHubCIStatusRecord> actual = await service.GetCIStatus("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetIssueBreakdown method returns the correct record.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueBreakdown()
        {
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                1,
                "Fix login bug");
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                2,
                "Fix logout bug");
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer2",
                3,
                "Fix signup bug");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                4,
                "Add dashboard");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                5,
                "Add profile");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer2",
                6,
                "Add settings");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer2",
                7,
                "Add notifications");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                8,
                "Add reports");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer2",
                9,
                "Add search");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                10,
                "Add export");

            GitHubService service = CreateService();

            GitHubIssueBreakdownRecord actual = await service.GetIssueBreakdown("TestRepo");

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                10,
                actual.TotalIssues);
            Assert.AreEqual(
                3,
                actual.Bugs);
            Assert.AreEqual(
                7,
                actual.NewFeatures);
        }

        /// <summary>
        /// Checks whether the GetIssueBreakdown method returns null when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueBreakdownNull()
        {
            GitHubService service = CreateService();

            GitHubIssueBreakdownRecord actual = await service.GetIssueBreakdown("NonExistent");

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                0,
                actual.TotalIssues);
        }

        /// <summary>
        /// Checks whether the GetIssueAssigneeBreakdown method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueAssigneeBreakdown()
        {
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                1,
                "Fix bug 1");
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                2,
                "Fix bug 2");
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                3,
                "Fix bug 3");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                4,
                "Add feature 1");
            InsertIssueData(
                "TestRepo",
                "Open",
                "new feature",
                "Developer1",
                5,
                "Add feature 2");

            GitHubService service = CreateService();

            List<GitHubIssueAssigneeBreakdownRecord> actual = await service.GetIssueAssigneeBreakdown("TestRepo");

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "Developer1",
                actual[0].Name);
            Assert.AreEqual(
                5,
                actual[0].Issues);
        }

        /// <summary>
        /// Checks whether the GetIssueAssigneeBreakdown method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueAssigneeBreakdownEmpty()
        {
            GitHubService service = CreateService();

            List<GitHubIssueAssigneeBreakdownRecord> actual = await service.GetIssueAssigneeBreakdown("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetIssueInProgressBreakdown method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueInProgressBreakdown()
        {
            InsertIssueData(
                "TestRepo",
                "Open",
                "bug",
                "Developer1",
                42,
                "Fix login bug");

            GitHubService service = CreateService();

            List<GitHubIssueInProgressBreakdownRecord> actual = await service.GetIssueInProgressBreakdown("TestRepo");

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                42,
                actual[0].Id);
            Assert.AreEqual(
                "Developer1",
                actual[0].Assignee);
            Assert.AreEqual(
                "Fix login bug",
                actual[0].Title);
            Assert.AreEqual(
                "Bug",
                actual[0].Type);
        }

        /// <summary>
        /// Checks whether the GetIssueInProgressBreakdown method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueInProgressBreakdownEmpty()
        {
            GitHubService service = CreateService();

            List<GitHubIssueInProgressBreakdownRecord> actual = await service.GetIssueInProgressBreakdown("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

    }
}
