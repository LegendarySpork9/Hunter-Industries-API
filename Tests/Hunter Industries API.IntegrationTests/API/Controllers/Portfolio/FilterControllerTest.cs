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
    public class FilterControllerTest
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
        private FilterController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new FilterController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object);
        }

        /// <summary>
        /// Inserts a portfolio filter record and returns the generated ID.
        /// </summary>
        private int InsertFilter(
            string name,
            string type,
            string filterOperator = null,
            string path = null,
            string values = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioFilter ([Name], [Type], [Operator], [Path], [Values]) " +
                    "VALUES (@name, @type, @operator, @path, @values); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);
                    cmd.Parameters.AddWithValue(
                        "@operator",
                        (object)filterOperator ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@path",
                        (object)path ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@values",
                        (object)values ?? DBNull.Value);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of filters.
        /// </summary>
        [TestMethod]
        public async Task TestGetFilters()
        {
            InsertFilter(
                "Language",
                "tag",
                values: "C#,Python,JavaScript");

            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no filters are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltersEmpty()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a tag filter is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Type = "tag",
                    Values = "C#,Python,JavaScript"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a null check filter is created.
        /// </summary>
        [TestMethod]
        public async Task TestPostNullCheckFilter()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Has LLM Usage",
                    Type = "null",
                    Operator = "has value",
                    Path = "llmUsage"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a comparison filter is created.
        /// </summary>
        [TestMethod]
        public async Task TestPostComparisonFilter()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "More Bugs than Features",
                    Type = "comparison",
                    Operator = "greater than",
                    Path = "gitHubInformation.issueBreakdown.bugs",
                    Values = "gitHubInformation.issueBreakdown.newFeatures"
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
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the operator is invalid for the type.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidOperator()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Invalid Filter",
                    Type = "numeric",
                    Operator = "contains",
                    Path = "unitTestCoverage",
                    Values = "50"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the path is missing for a non-tag filter.
        /// </summary>
        [TestMethod]
        public async Task TestPostMissingPath()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Missing Path",
                    Type = "numeric",
                    Operator = "greater than",
                    Values = "5"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the operator is missing for a non-tag filter.
        /// </summary>
        [TestMethod]
        public async Task TestPostMissingOperator()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Missing Operator",
                    Type = "boolean",
                    Path = "isDeleted"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 status code when a record already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPostAlreadyExists()
        {
            InsertFilter(
                "Language",
                "tag",
                values: "C#,Python,JavaScript");

            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio/filter"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Type = "tag",
                    Values = "C#,Python,JavaScript"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the record is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            int filterId = InsertFilter(
                "Language",
                "tag",
                values: "C#,Python,JavaScript");

            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri($"https://localhost/v2.2/portfolio/filter/{filterId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                filterId,
                new FilterModel
                {
                    Name = "Language Updated"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the body is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.2/portfolio/filter/1"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.2/portfolio/filter/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new FilterModel
                {
                    Name = "Language Updated"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            int filterId = InsertFilter(
                "Language",
                "tag",
                values: "C#,Python,JavaScript");

            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri($"https://localhost/v2.2/portfolio/filter/{filterId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(filterId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteNotFound()
        {
            FilterController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri("https://localhost/v2.2/portfolio/filter/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
