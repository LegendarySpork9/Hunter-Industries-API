// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Objects.Assistant;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Assistant;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Assistant
{
    [TestClass]
    [DoNotParallelize]
    public class ConfigServiceTest
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
        private ConfigService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            ConfigService service = new(
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
            string userName,
            string hostName,
            string ipAddress,
            int deletionStatusId,
            string version)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Location] (HostName, IPAddress) VALUES (@hostName, @ipAddress); " +
                    "DECLARE @locationId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO [User] ([Name]) VALUES (@userName); " +
                    "DECLARE @userId INT = SCOPE_IDENTITY(); " +
                    "DECLARE @versionId INT; " +
                    "SELECT @versionId = VersionId FROM [Version] WHERE [Value] = @version; " +
                    "IF @versionId IS NULL BEGIN " +
                    "INSERT INTO [Version] ([Value]) VALUES (@version); " +
                    "SET @versionId = SCOPE_IDENTITY(); " +
                    "END; " +
                    "INSERT INTO AssistantInformation (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber) " +
                    "VALUES (@locationId, @deletionStatusId, @versionId, @userId, @assistantName, @idNumber);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@userName",
                        userName);
                    cmd.Parameters.AddWithValue(
                        "@version",
                        version);
                    cmd.Parameters.AddWithValue(
                        "@deletionStatusId",
                        deletionStatusId);
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
        /// Checks whether the GetAssistantConfig method returns the assistant configurations, total count, and version.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantConfig()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost",
                "192.168.1.1",
                2,
                "1.0.0");

            ConfigService service = CreateService();

            (List<AssistantConfiguration> results, int totalConfigs, string mostRecentVersion) = await service.GetAssistantConfig(
                "TestAssistant",
                "A001");

            Assert.AreEqual(
                1,
                results.Count);
            Assert.AreEqual(
                "TestAssistant",
                results[0].AssistantName);
            Assert.AreEqual(
                1,
                totalConfigs);
            Assert.AreEqual(
                "1.0.0",
                mostRecentVersion);
        }

        /// <summary>
        /// Checks whether the GetAssistantConfig method returns an empty list, zero, and empty string when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantConfigEmpty()
        {
            ConfigService service = CreateService();

            (List<AssistantConfiguration> results, int totalConfigs, string mostRecentVersion) = await service.GetAssistantConfig(
                null,
                null);

            Assert.AreEqual(
                0,
                results.Count);
            Assert.AreEqual(
                0,
                totalConfigs);
            Assert.AreEqual(
                "0.0.0",
                mostRecentVersion);
        }

        /// <summary>
        /// Checks whether the GetMostRecentVersion method returns the version string.
        /// </summary>
        [TestMethod]
        public async Task TestGetMostRecentVersion()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Version] ([Value]) VALUES ('3.1.0')",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            ConfigService service = CreateService();
            string result = await service.GetMostRecentVersion();

            Assert.AreEqual(
                "3.1.0",
                result);
        }

        /// <summary>
        /// Checks whether the GetMostRecentVersion method returns the seed version when no additional versions exist.
        /// </summary>
        [TestMethod]
        public async Task TestGetMostRecentVersionSeedOnly()
        {
            ConfigService service = CreateService();
            string result = await service.GetMostRecentVersion();

            Assert.AreEqual(
                "0.0.0",
                result);
        }

        /// <summary>
        /// Checks whether the AssistantExists method returns true when a matching assistant is found.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantExists()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost",
                "192.168.1.1",
                2,
                "0.0.0");

            ConfigService service = CreateService();
            bool actual = await service.AssistantExists(
                "TestAssistant",
                "A001");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantExists method returns false when no matching assistant is found.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantExistsNot()
        {
            ConfigService service = CreateService();
            bool actual = await service.AssistantExists(
                "TestAssistant",
                "A001");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the AssistantConfigCreated method returns true when all database calls succeed.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantConfigCreated()
        {
            ConfigService service = CreateService();
            bool actual = await service.AssistantConfigCreated(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost");

            Assert.IsTrue(actual);

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM AssistantInformation WHERE [Name] = 'TestAssistant' AND IDNumber = 'A001'",
                    conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    Assert.AreEqual(
                        1,
                        count);
                }
            }
        }

        /// <summary>
        /// Checks whether the AssistantConfigCreated method creates the assistant with the correct deletion status and version.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantConfigCreatedDefaults()
        {
            ConfigService service = CreateService();
            await service.AssistantConfigCreated(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT AI.DeletionStatusId, V.[Value] " +
                    "FROM AssistantInformation AI " +
                    "JOIN [Version] V ON AI.VersionId = V.VersionId " +
                    "WHERE AI.[Name] = 'TestAssistant' AND AI.IDNumber = 'A001'",
                    conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.AreEqual(
                            2,
                            reader.GetInt32(0));
                        Assert.AreEqual(
                            "0.0.0",
                            reader.GetString(1));
                    }
                }
            }
        }

    }
}
