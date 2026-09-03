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
    public class ErrorLogServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IClock> _MockClock = new();
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

            _MockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private ErrorLogService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            ErrorLogService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database,
                _MockClock.Object);

            return service;
        }

        /// <summary>
        /// Inserts an error log record into the database.
        /// </summary>
        private void InsertErrorLog(
            DateTime dateOccured,
            string ipAddress,
            string summary,
            string message)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ErrorLog (DateOccured, IPAddress, Summary, [Message]) VALUES (@dateOccured, @ipAddress, @summary, @message)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@summary",
                        summary);
                    cmd.Parameters.AddWithValue(
                        "@message",
                        message);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks whether the GetErrorLog method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorLog()
        {
            InsertErrorLog(
                new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                "127.0.0.1",
                "This is an error.",
                "This is a detailed error trace.");

            ErrorLogService service = CreateService();

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
                1,
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
            ErrorLogService service = CreateService();

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
            InsertErrorLog(
                new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                "127.0.0.1",
                "This is an error.",
                "This is a detailed error trace.");

            int insertedId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 ErrorId FROM ErrorLog",
                    conn))
                {
                    insertedId = (int)cmd.ExecuteScalar();
                }
            }

            ErrorLogService service = CreateService();

            ErrorLogRecord actual = await service.GetErrorLogId(insertedId);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                insertedId,
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
            ErrorLogService service = CreateService();

            ErrorLogRecord actual = await service.GetErrorLogId(999);

            Assert.IsNull(actual);
        }
    }
}
