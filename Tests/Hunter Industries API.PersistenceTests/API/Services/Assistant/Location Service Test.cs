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
    public class LocationServiceTest
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
        private LocationService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            LocationService service = new(
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
            string hostName,
            string ipAddress)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Location] (HostName, IPAddress) VALUES (@hostName, @ipAddress); " +
                    "DECLARE @locationId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO [User] ([Name]) VALUES ('TestUser'); " +
                    "DECLARE @userId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO AssistantInformation (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber) " +
                    "VALUES (@locationId, 2, 1, @userId, @assistantName, @idNumber);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
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
        /// Checks whether the GetAssistantLocation method returns a populated location response model.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantLocation()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "TestHost",
                "192.168.1.1");

            LocationService service = CreateService();
            LocationResponseModel actual = await service.GetAssistantLocation(
                "TestAssistant",
                "A001");

            Assert.AreEqual(
                "TestAssistant",
                actual.AssistantName);
            Assert.AreEqual(
                "A001",
                actual.IdNumber);
            Assert.AreEqual(
                "TestHost",
                actual.HostName);
            Assert.AreEqual(
                "192.168.1.1",
                actual.IPAddress);
        }

        /// <summary>
        /// Checks whether the GetAssistantLocation method returns an empty model when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantLocationEmpty()
        {
            LocationService service = CreateService();
            LocationResponseModel actual = await service.GetAssistantLocation(
                "TestAssistant",
                "A001");

            Assert.IsNull(actual.AssistantName);
            Assert.IsNull(actual.IdNumber);
            Assert.IsNull(actual.HostName);
            Assert.IsNull(actual.IPAddress);
        }

        /// <summary>
        /// Checks whether the AssistantLocationUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantLocationUpdated()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "OldHost",
                "10.0.0.1");

            LocationService service = CreateService();
            bool actual = await service.AssistantLocationUpdated(
                "TestAssistant",
                "A001",
                "NewHost",
                "10.0.0.1");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantLocationUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantLocationUpdatedFailed()
        {
            LocationService service = CreateService();
            bool actual = await service.AssistantLocationUpdated(
                "TestAssistant",
                "A001",
                "NewHost",
                "10.0.0.1");

            Assert.IsFalse(actual);
        }

    }
}
