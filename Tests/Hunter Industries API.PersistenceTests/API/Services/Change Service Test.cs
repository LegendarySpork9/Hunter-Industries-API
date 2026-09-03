// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class ChangeServiceTest
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
        private ChangeService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ChangeService(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);
        }

        /// <summary>
        /// Inserts an audit history record and returns the generated ID.
        /// </summary>
        private int InsertAuditHistoryRecord()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (EndpointId, EndpointVersionId, MethodId, StatusId, IPAddress, DateOccured) " +
                    "VALUES (1, 1, 1, 1, '127.0.0.1', GETUTCDATE()); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    return System.Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Checks whether the LogChange method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestLogChange()
        {
            int auditId = InsertAuditHistoryRecord();
            ChangeService service = CreateService();

            bool actual = await service.LogChange(
                auditId,
                "Field",
                "OldValue",
                "NewValue");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LogChange method returns false when the audit ID does not exist.
        /// </summary>
        [TestMethod]
        public async Task TestLogChangeFailed()
        {
            ChangeService service = CreateService();

            bool actual = await service.LogChange(
                99999,
                "Field",
                "OldValue",
                "NewValue");

            Assert.IsFalse(actual);
        }
    }
}
