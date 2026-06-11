// Copyright © - 11/06/2026 - Toby Hunter
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
    public class ErrorLogServiceTest
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

        /// <summary>
        /// Checks whether the GetErrorLog method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLog()
        {
            List<ErrorLogRecord> records =
            [
                new ErrorLogRecord
                {
                    Id = 1,
                    DateOccured = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IPAddress = "127.0.0.1",
                    Summary = "This is an error.",
                    Message = "This is a detailed error trace."
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(),
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

            ErrorLogService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            (List<ErrorLogRecord> actual, int totalRecords) = await service.GetErrorLog(
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
                "This is an error.",
                actual[0].Summary);
            Assert.AreEqual(
                "This is a detailed error trace.",
                actual[0].Message);
        }

        /// <summary>
        /// Checks whether the GetErrorLog method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLogEmpty()
        {
            List<ErrorLogRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(),
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

            ErrorLogService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            (List<ErrorLogRecord> actual, int totalRecords) = await service.GetErrorLog(
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
        /// Checks whether the GetErrorLogId method returns the correct record.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLogId()
        {
            ErrorLogRecord expected = new ErrorLogRecord
            {
                Id = 1,
                DateOccured = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IPAddress = "127.0.0.1",
                Summary = "This is an error.",
                Message = "This is a detailed error trace."
            };

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    expected,
                    (Exception)null));

            ErrorLogService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            ErrorLogRecord actual = await service.GetErrorLogId(1);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "127.0.0.1",
                actual.IPAddress);
            Assert.AreEqual(
                "This is an error.",
                actual.Summary);
            Assert.AreEqual(
                "This is a detailed error trace.",
                actual.Message);
        }

        /// <summary>
        /// Checks whether the GetErrorLogId method returns null when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLogIdEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (ErrorLogRecord)null,
                    (Exception)null));

            ErrorLogService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                _MockClock.Object);

            ErrorLogRecord actual = await service.GetErrorLogId(1);

            Assert.IsNull(actual);
        }
    }
}
