// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Assistant;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Assistant
{
    [TestClass]
    [DoNotParallelize]
    public class VersionServiceTest
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
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private VersionService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            VersionService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts test assistant data into the database.
        /// </summary>
        private void InsertAssistantData(
            string assistantName,
            string idNumber,
            string version)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Location] (HostName, IPAddress) VALUES ('TestHost', '192.168.1.1'); " +
                    "DECLARE @locationId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO [User] ([Name]) VALUES ('TestUser'); " +
                    "DECLARE @userId INT = SCOPE_IDENTITY(); " +
                    "DECLARE @versionId INT; " +
                    "SELECT @versionId = VersionId FROM [Version] WHERE [Value] = @version; " +
                    "IF @versionId IS NULL BEGIN " +
                    "INSERT INTO [Version] ([Value]) VALUES (@version); " +
                    "SET @versionId = SCOPE_IDENTITY(); " +
                    "END; " +
                    "INSERT INTO AssistantInformation (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber) " +
                    "VALUES (@locationId, 2, @versionId, @userId, @assistantName, @idNumber);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@version",
                        version);
                    cmd.Parameters.AddWithValue(
                        "@assistantName",
                        assistantName);
                    cmd.Parameters.AddWithValue(
                        "@idNumber",
                        idNumber);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetAssistantVersion method returns a populated version response model.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantVersion()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "2.5.0");

            VersionService service = CreateService();
            VersionResponseModel actual = await service.GetAssistantVersion(
                "TestAssistant",
                "A001");

            Assert.AreEqual(
                "TestAssistant",
                actual.AssistantName);
            Assert.AreEqual(
                "A001",
                actual.IdNumber);
            Assert.AreEqual(
                "2.5.0",
                actual.Version);
        }

        /// <summary>
        /// Checks whether the GetAssistantVersion method returns an empty model when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantVersionEmpty()
        {
            VersionService service = CreateService();
            VersionResponseModel actual = await service.GetAssistantVersion(
                "TestAssistant",
                "A001");

            Assert.IsNull(actual.AssistantName);
            Assert.IsNull(actual.IdNumber);
            Assert.IsNull(actual.Version);
        }

        /// <summary>
        /// Checks whether the AssistantVersionUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantVersionUpdated()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "2.5.0");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM [Version] WHERE [Value] = '3.0.0') " +
                    "INSERT INTO [Version] ([Value]) VALUES ('3.0.0');",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            VersionService service = CreateService();
            bool actual = await service.AssistantVersionUpdated(
                "TestAssistant",
                "A001",
                "3.0.0");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantVersionUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantVersionUpdatedFailed()
        {
            VersionService service = CreateService();
            bool actual = await service.AssistantVersionUpdated(
                "TestAssistant",
                "A001",
                "3.0.0");

            Assert.IsFalse(actual);
        }

    }
}
