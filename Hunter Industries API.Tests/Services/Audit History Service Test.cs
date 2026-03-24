// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.Services
{
    [TestClass]
    public class AuditHistoryServiceTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();
        private readonly Mock<IClock> _mockClock = new Mock<IClock>();

        [TestInitialize]
        public void Setup()
        {
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQL");
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("SELECT 1");
            _mockClock.Setup(c => c.DefaultDate).Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _mockClock.Setup(c => c.UtcNow).Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        }

        #region LogRequest

        /// <summary>
        /// Checks whether the LogRequest method returns true and the audit ID when the database returns an ID.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequest()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            AuditHistoryService service = new AuditHistoryService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (bool logged, int auditId) = await service.LogRequest("127.0.0.1", 1, 1, 1, 1);

            Assert.IsTrue(logged);
            Assert.AreEqual(1, auditId);
        }

        /// <summary>
        /// Checks whether the LogRequest method returns false and zero when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLogRequestFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            AuditHistoryService service = new AuditHistoryService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (bool logged, int auditId) = await service.LogRequest("127.0.0.1", 1, 1, 1, 1);

            Assert.IsFalse(logged);
            Assert.AreEqual(0, auditId);
        }

        #endregion

        #region LogLoginAttempt

        /// <summary>
        /// Checks whether the LogLoginAttempt method completes without throwing an exception.
        /// </summary>
        [TestMethod]
        public async Task TestLogLoginAttempt()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            AuditHistoryService service = new AuditHistoryService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            await service.LogLoginAttempt(1, true, "admin", "password", "phrase");
        }

        #endregion

        #region GetAuditHistory

        /// <summary>
        /// Checks whether the GetAuditHistory method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistory()
        {
            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>
            {
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    EndpointVersion = "v1.0",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = new string[0],
                    LoginAttempt = null,
                    Change = new List<ChangeRecord>()
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((5, null));

            AuditHistoryService service = new AuditHistoryService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (List<AuditHistoryRecord> actual, int totalRecords) = await service.GetAuditHistory(0, null, null, null, null, new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc), 10, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(5, totalRecords);
            Assert.AreEqual("127.0.0.1", actual[0].IPAddress);
            Assert.AreEqual("token", actual[0].Endpoint);
        }

        /// <summary>
        /// Checks whether the GetAuditHistory method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryEmpty()
        {
            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            AuditHistoryService service = new AuditHistoryService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (List<AuditHistoryRecord> actual, int totalRecords) = await service.GetAuditHistory(0, null, null, null, null, new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc), 10, 1);

            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0, totalRecords);
        }

        #endregion
    }
}
