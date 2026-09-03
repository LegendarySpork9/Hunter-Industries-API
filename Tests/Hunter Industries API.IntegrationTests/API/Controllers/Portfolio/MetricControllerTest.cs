// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Portfolio;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
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

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers.Portfolio
{
    [TestClass]
    [DoNotParallelize]
    public class MetricControllerTest
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

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://localhost", null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        /// <summary>
        /// Creates a controller instance with real database dependencies for testing.
        /// </summary>
        private MetricController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new MetricController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object);
        }

        /// <summary>
        /// Inserts a portfolio item record and returns the generated ID.
        /// </summary>
        private int InsertPortfolioItem()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Web Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                    "SELECT PIT.PortfolioItemTypeId, 'TestItem', 'Summary', 'Description', 'icon.png', 'Notes', 'https://github.com/test', GETUTCDATE(), GETUTCDATE(), 0 " +
                    "FROM PortfolioItemType PIT WHERE PIT.[Name] = 'Web Application'; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the list of metrics.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            MetricController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio/metric"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when the metric is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            int itemId = InsertPortfolioItem();

            MetricController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/metric"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new ItemMetricModel
                {
                    Id = itemId,
                    Metric = "summary"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            MetricController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/metric"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 404 status code when the portfolio item does not exist.
        /// </summary>
        [TestMethod]
        public async Task TestPostNotFound()
        {
            MetricController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/metric"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new ItemMetricModel
                {
                    Id = 999,
                    Metric = "summary"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
