// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class AuditHistoryServiceTest
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
        private (AuditHistoryService service, DatabaseWrapper database) CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            mockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            DatabaseWrapper database = new(mockOptions.Object);

            ChangeService changeService = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            AuditHistoryService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database,
                mockClock.Object,
                changeService);

            return (service, database);
        }

        /// <summary>
        /// Inserts an audit history record and returns the generated ID.
        /// </summary>
        private int InsertAuditHistory(
            string ipAddress,
            int endpointId,
            int endpointVersionId,
            int methodId,
            int statusId)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured) " +
                    "OUTPUT INSERTED.AuditId " +
                    "VALUES (@ip, @eid, @evid, @mid, @sid, GETUTCDATE())",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@ip",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@eid",
                        endpointId);
                    cmd.Parameters.AddWithValue(
                        "@evid",
                        endpointVersionId);
                    cmd.Parameters.AddWithValue(
                        "@mid",
                        methodId);
                    cmd.Parameters.AddWithValue(
                        "@sid",
                        statusId);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a login attempt record into the database.
        /// </summary>
        private void InsertLoginAttempt(
            int auditId,
            bool isSuccessful)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO LoginAttempt (AuditId, DateOccured, IsSuccessful) " +
                    "VALUES (@auditId, GETUTCDATE(), @isSuccessful)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@auditId",
                        auditId);
                    cmd.Parameters.AddWithValue(
                        "@isSuccessful",
                        isSuccessful);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a change record into the database.
        /// </summary>
        private void InsertChange(
            int auditId,
            string field,
            string oldValue,
            string newValue)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Change] (AuditId, Field, OldValue, NewValue) " +
                    "VALUES (@auditId, @field, @oldValue, @newValue)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@auditId",
                        auditId);
                    cmd.Parameters.AddWithValue(
                        "@field",
                        field);
                    cmd.Parameters.AddWithValue(
                        "@oldValue",
                        oldValue);
                    cmd.Parameters.AddWithValue(
                        "@newValue",
                        newValue);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks whether the LogRequest method returns true and the audit ID when the database returns an ID.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequest()
        {
            (AuditHistoryService service, _) = CreateService();

            (bool logged, int auditId) = await service.LogRequest(
                "127.0.0.1",
                1,
                1,
                1,
                1);

            Assert.IsTrue(logged);
            Assert.AreEqual(
                true,
                auditId > 0);
        }

        /// <summary>
        /// Checks whether the LogRequest method returns false and zero when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequestFailed()
        {
            (AuditHistoryService service, _) = CreateService();

            (bool logged, int auditId) = await service.LogRequest(
                "127.0.0.1",
                1,
                1,
                1,
                999);

            Assert.IsFalse(logged);
            Assert.AreEqual(
                0,
                auditId);
        }

        /// <summary>
        /// Checks whether the LogLoginAttempt method completes without throwing an exception.
        /// </summary>
        [TestMethod]
        public async Task TestLogLoginAttempt()
        {
            (AuditHistoryService service, _) = CreateService();

            int auditId = InsertAuditHistory("127.0.0.1", 1, 1, 2, 1);

            await service.LogLoginAttempt(
                auditId,
                true,
                "admin",
                "password",
                "phrase");
        }

        /// <summary>
        /// Checks whether the UpdateResponseBody method completes without throwing an exception.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateResponseBody()
        {
            (AuditHistoryService service, _) = CreateService();

            int auditId = InsertAuditHistory("127.0.0.1", 1, 1, 1, 1);

            await service.UpdateResponseBody(
                auditId,
                "{\"statusCode\":200}");
        }

        /// <summary>
        /// Checks whether the GetAuditHistory method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistory()
        {
            InsertAuditHistory("127.0.0.1", 1, 1, 2, 1);

            (AuditHistoryService service, _) = CreateService();

            (List<AuditHistoryRecord> actual, int totalRecords) = await service.GetAuditHistory(
                null,
                null,
                null,
                null,
                new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                1,
                totalRecords);
            Assert.AreEqual(
                "127.0.0.1",
                actual[0].IPAddress);
        }

        /// <summary>
        /// Checks whether the GetAuditHistory method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryEmpty()
        {
            (AuditHistoryService service, _) = CreateService();

            (List<AuditHistoryRecord> actual, int totalRecords) = await service.GetAuditHistory(
                null,
                null,
                null,
                null,
                new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                10,
                1);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetAuditHistoryId method returns the correct record with login attempt and changes.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryId()
        {
            int auditId = InsertAuditHistory(
                "127.0.0.1",
                1,
                1,
                2,
                1);
            InsertLoginAttempt(
                auditId,
                true);
            InsertChange(
                auditId,
                "Username",
                "admin",
                "superadmin");

            (AuditHistoryService service, _) = CreateService();

            AuditHistoryRecord actual = await service.GetAuditHistoryId(auditId);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                auditId,
                actual.Id);
            Assert.AreEqual(
                "127.0.0.1",
                actual.IPAddress);
            Assert.IsNotNull(actual.LoginAttempt);
            Assert.AreEqual(
                true,
                actual.LoginAttempt.IsSuccessful);
            Assert.IsNotNull(actual.Change);
            Assert.AreEqual(
                1,
                actual.Change.Count);
        }

        /// <summary>
        /// Checks whether the GetAuditHistoryId method returns null when the database returns no result.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryIdNotFound()
        {
            (AuditHistoryService service, _) = CreateService();

            AuditHistoryRecord actual = await service.GetAuditHistoryId(999);

            Assert.IsNull(actual);
        }

    }
}
