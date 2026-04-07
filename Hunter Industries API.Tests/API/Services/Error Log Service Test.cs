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
    public class ErrorLogServiceTest
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

        /// <summary>
        /// Checks whether the GetErrorLog method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLog()
        {
            List<ErrorLogRecord> records = new List<ErrorLogRecord>
            {
                new ErrorLogRecord
                {
                    Id = 1,
                    DateOccured = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IPAddress = "127.0.0.1",
                    Summary = "This is an error.",
                    Message = "This is a detailed error trace."
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((5, null));

            ErrorLogService service = new ErrorLogService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (List<ErrorLogRecord> actual, int totalRecords) = await service.GetErrorLog(0, null, null, new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc), 10, 1);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(5, totalRecords);
            Assert.AreEqual("127.0.0.1", actual[0].IPAddress);
            Assert.AreEqual("This is an error.", actual[0].Summary);
            Assert.AreEqual("This is a detailed error trace.", actual[0].Message);
        }

        /// <summary>
        /// Checks whether the GetErrorLog method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLogEmpty()
        {
            List<ErrorLogRecord> records = new List<ErrorLogRecord>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ErrorLogRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            ErrorLogService service = new ErrorLogService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object, _mockClock.Object);

            (List<ErrorLogRecord> actual, int totalRecords) = await service.GetErrorLog(0, null, null, new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc), 10, 1);

            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0, totalRecords);
        }
    }
}
