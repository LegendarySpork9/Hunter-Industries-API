// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Implementations;
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
    public class StatisticControllerTest
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
        private StatisticController CreateController(string endpoint)
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            StatisticController controller = new(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri($"https://localhost/{endpoint}")),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }
        /// <summary>
        /// Checks whether the GetDashboard method returns a 200 with dashboard statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboard()
        {
            StatisticController controller = CreateController("v2.0/statistic/dashboard");

            IHttpActionResult actionResult = await controller.GetDashboard();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetServer method returns a 200 with server statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetServer()
        {
            StatisticController controller = CreateController("v2.0/statistic/server/1");

            IHttpActionResult actionResult = await controller.GetServer(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetError method returns a 200 with error statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetError()
        {
            StatisticController controller = CreateController("v2.0/statistic/error");

            IHttpActionResult actionResult = await controller.GetError();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetApplication method returns a 200 with application statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplication()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SET IDENTITY_INSERT Authorisation ON; " +
                    "INSERT INTO Authorisation (PhraseId, Phrase, IsDeleted) VALUES (1, 'testphrase', 0); " +
                    "SET IDENTITY_INSERT Authorisation OFF; " +
                    "SET IDENTITY_INSERT Application ON; " +
                    "INSERT INTO Application (ApplicationId, PhraseId, [Name], IsDeleted) VALUES (1, 1, 'TestApplication', 0); " +
                    "SET IDENTITY_INSERT Application OFF;",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticController controller = CreateController("v2.0/statistic/application/1");

            IHttpActionResult actionResult = await controller.GetApplication(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetUser method returns a 200 with user statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetUser()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('testuser', 'testhash', 0);",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticController controller = CreateController("v2.0/statistic/user/1");

            IHttpActionResult actionResult = await controller.GetUser(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetPortfolio method returns a 200 with portfolio statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolio()
        {
            StatisticController controller = CreateController("v2.2/statistic/portfolio");

            IHttpActionResult actionResult = await controller.GetPortfolio();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the GetDashboard method returns a 200 with empty data when called with an unknown endpoint path.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardUnknown()
        {
            StatisticController controller = CreateController("v2.0/statistic/unknown");

            IHttpActionResult actionResult = await controller.GetDashboard();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }
    }
}
