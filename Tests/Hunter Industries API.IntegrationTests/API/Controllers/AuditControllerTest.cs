// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers
{
    [TestClass]
    [DoNotParallelize]
    public class AuditControllerTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private Mock<ILoggerService> _MockLogger;
        private Mock<IClock> _MockClock;
        private IFileSystem _FileSystem = new FileSystemWrapper();

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

            _MockLogger = new Mock<ILoggerService>();
            _MockClock = new Mock<IClock>();
            _MockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            HttpContext.Current = new HttpContext(
                new HttpRequest(
                    null,
                    "http://localhost",
                    null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        /// <summary>
        /// Creates a controller instance with real database dependencies for testing.
        /// </summary>
        private AuditController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            AuditController controller = new(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/audithistory")),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with audit history data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured) " +
                    "VALUES ('127.0.0.1', 1, 1, 2, 1, '2024-01-01 10:00:00')",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            AuditController controller = CreateController();

            AuditHistoryFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 204 status code when filters exclude the current call and no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmptyFilterMismatch()
        {
            AuditController controller = CreateController();

            AuditHistoryFilterModel filters = new()
            {
                Endpoint = "token"
            };

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.NoContent,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 204 status code when on page 2 with no records.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmptyPageTwo()
        {
            AuditController controller = CreateController();

            AuditHistoryFilterModel filters = new()
            {
                PageNumber = 2
            };

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.NoContent,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the current call record when the database has no records but filters match.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmptyWithCurrentCall()
        {
            AuditController controller = CreateController();

            AuditHistoryFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with a single audit record.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            int auditId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (IPAddress, EndpointId, EndpointVersionId, MethodId, StatusId, DateOccured) " +
                    "OUTPUT INSERTED.AuditId " +
                    "VALUES ('127.0.0.1', 1, 1, 2, 1, '2024-01-01 10:00:00')",
                    conn))
                {
                    auditId = (int)cmd.ExecuteScalar();
                }
            }

            AuditController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(auditId);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with an error message when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            AuditController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(999);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

    }
}
