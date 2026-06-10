// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services
{
    [TestClass]
    public class AuditHistoryServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();
        private readonly Mock<IClock> _MockClock = new();

        [TestInitialize]
        public void Setup()
        {
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQL");
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("SELECT 1");
            _MockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        }

        #region LogRequest

        /// <summary>
        /// Checks whether the LogRequest method returns true and the audit ID when the database returns an ID.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequest()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            (bool logged, int auditId) = await service.LogRequest(
                "127.0.0.1",
                1,
                1,
                1,
                1);

            Assert.IsTrue(logged);
            Assert.AreEqual(
                1,
                auditId);
        }

        /// <summary>
        /// Checks whether the LogRequest method returns false and zero when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequestFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            (bool logged, int auditId) = await service.LogRequest(
                "127.0.0.1",
                1,
                1,
                1,
                1);

            Assert.IsFalse(logged);
            Assert.AreEqual(
                0,
                auditId);
        }

        #endregion

        #region LogLoginAttempt

        /// <summary>
        /// Checks whether the LogLoginAttempt method completes without throwing an exception.
        /// </summary>
        [TestMethod]
        public async Task TestLogLoginAttempt()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            await service.LogLoginAttempt(
                1,
                true,
                "admin",
                "password",
                "phrase");
        }

        #endregion

        #region GetAuditHistory

        /// <summary>
        /// Checks whether the GetAuditHistory method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistory()
        {
            List<AuditHistoryRecord> records =
            [
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    EndpointVersion = "v1.0",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = [],
                    LoginAttempt = null,
                    Change = []
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    5,
                    null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

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
                5,
                totalRecords);
            Assert.AreEqual(
                "127.0.0.1",
                actual[0].IPAddress);
            Assert.AreEqual(
                "token",
                actual[0].Endpoint);
        }

        /// <summary>
        /// Checks whether the GetAuditHistory method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryEmpty()
        {
            List<AuditHistoryRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

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

        #endregion

        #region GetAuditHistoryId

        /// <summary>
        /// Checks whether the GetAuditHistoryId method returns the correct record with login attempt and changes.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryId()
        {
            AuditHistoryRecord record = new()
            {
                Id = 1,
                IPAddress = "127.0.0.1",
                Endpoint = "token",
                EndpointVersion = "v1.0",
                Method = "POST",
                Status = "OK",
                OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                Paramaters = []
            };

            LoginAttemptRecord loginAttempt = new()
            {
                Id = 1,
                Username = "admin",
                IsSuccessful = true
            };

            List<ChangeRecord> changes =
            [
                new ChangeRecord
                {
                    Id = 1,
                    Field = "Username",
                    OldValue = "admin",
                    NewValue = "superadmin"
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    record,
                    (Exception)null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LoginAttemptRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    loginAttempt,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ChangeRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    changes,
                    (Exception)null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            AuditHistoryRecord actual = await service.GetAuditHistoryId(1);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "127.0.0.1",
                actual.IPAddress);
            Assert.IsNotNull(actual.LoginAttempt);
            Assert.AreEqual(
                "admin",
                actual.LoginAttempt.Username);
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (AuditHistoryRecord)null,
                    (Exception)null));

            AuditHistoryService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            AuditHistoryRecord actual = await service.GetAuditHistoryId(999);

            Assert.IsNull(actual);
        }

        #endregion
    }
}
