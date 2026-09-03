// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Portfolio;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
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
    public class PortfolioControllerTest
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
        private PortfolioController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new PortfolioController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object);
        }

        /// <summary>
        /// Inserts prerequisite records for portfolio testing.
        /// </summary>
        private void InsertPortfolioPrerequisites()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Web Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Console Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Console Application'); " +
                    "IF NOT EXISTS (SELECT 1 FROM LLMCompany WHERE [Name] = 'Anthropic') " +
                    "INSERT INTO LLMCompany ([Name]) VALUES ('Anthropic'); " +
                    "IF NOT EXISTS (SELECT 1 FROM LLMModel WHERE [Name] = 'Claude') " +
                    "BEGIN " +
                    "DECLARE @companyId INT = (SELECT LLMCompanyId FROM LLMCompany WHERE [Name] = 'Anthropic'); " +
                    "INSERT INTO LLMModel (LLMCompanyId, [Name]) VALUES (@companyId, 'Claude'); " +
                    "END " +
                    "IF NOT EXISTS (SELECT 1 FROM Framework WHERE [Name] = '.NET') " +
                    "INSERT INTO Framework ([Name]) VALUES ('.NET'); " +
                    "IF NOT EXISTS (SELECT 1 FROM Language WHERE [Name] = 'C#') " +
                    "INSERT INTO Language ([Name]) VALUES ('C#'); " +
                    "IF NOT EXISTS (SELECT 1 FROM Environment WHERE [Name] = 'Windows') " +
                    "INSERT INTO Environment ([Name]) VALUES ('Windows');",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a portfolio item record and returns the generated ID.
        /// </summary>
        private int InsertPortfolioItem(
            string name = "TestItem",
            string type = "Web Application",
            bool withLLM = true)
        {
            InsertPortfolioPrerequisites();

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                string sql;

                if (withLLM)
                {
                    sql = "INSERT INTO PortfolioItem (TypeId, LLMModelId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, UnitTestCoverage, LLMUsageNotes, DateCreated, DateUpdated, IsDeleted) " +
                          "SELECT PIT.PortfolioItemTypeId, LM.LLMModelId, @name, 'Test summary', 'Test description', 'https://example.com/icon.png', 'Initial release', 'https://github.com/test/repo', 85.5, 'Used for code generation', GETUTCDATE(), GETUTCDATE(), 0 " +
                          "FROM PortfolioItemType PIT, LLMModel LM " +
                          "WHERE PIT.[Name] = @type AND LM.[Name] = 'Claude'; " +
                          "SELECT SCOPE_IDENTITY();";
                }

                else
                {
                    sql = "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                          "SELECT PIT.PortfolioItemTypeId, @name, 'Test summary', 'Test description', 'https://example.com/icon.png', 'Initial release', 'https://github.com/test/repo', GETUTCDATE(), GETUTCDATE(), 0 " +
                          "FROM PortfolioItemType PIT " +
                          "WHERE PIT.[Name] = @type; " +
                          "SELECT SCOPE_IDENTITY();";
                }

                using (SqlCommand cmd = new(
                    sql,
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of portfolio items.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolio()
        {
            InsertPortfolioItem();

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no items are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioEmpty()
        {
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a single portfolio item.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioById()
        {
            int itemId = InsertPortfolioItem();

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri($"https://localhost/v2.2/portfolio/{itemId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(itemId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no item is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioByIdEmpty()
        {
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.2/portfolio/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a portfolio item is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            InsertPortfolioPrerequisites();

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new ItemModel
                {
                    Name = "Test",
                    Type = "Console Application",
                    IconURL = "https://example.com/icon.png",
                    Summary = "A test portfolio item.",
                    Description = "A test portfolio item made in C#.",
                    Frameworks = [".NET"],
                    Languages = ["C#"],
                    Environments = ["Windows"],
                    DemoLink = null,
                    ReleaseNotes = "A new test portfolio item.",
                    BuildHistory =
                    [
                        new BuildHistoryRecord
                        {
                            Version = "1.0.0",
                            ReleaseDate = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc)
                        }
                    ],
                    UnitTestCoverage = null,
                    GitHubLink = "https://github.com/test/repo",
                    LLMUsage = null,
                    LLMUsageNotes = null
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
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(null);

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
            InsertPortfolioItem(
                "Test",
                "Console Application",
                false);

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.2/portfolio"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(
                new ItemModel
                {
                    Name = "Test",
                    Type = "Console Application",
                    IconURL = "https://example.com/icon.png",
                    Summary = "A test portfolio item.",
                    Description = "A test portfolio item made in C#.",
                    Frameworks = [".NET"],
                    Languages = ["C#"],
                    Environments = ["Windows"],
                    DemoLink = null,
                    ReleaseNotes = "A new test portfolio item.",
                    BuildHistory =
                    [
                        new BuildHistoryRecord
                        {
                            Version = "1.0.0",
                            ReleaseDate = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc)
                        }
                    ],
                    UnitTestCoverage = null,
                    GitHubLink = "https://github.com/test/repo",
                    LLMUsage = null,
                    LLMUsageNotes = null
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
            int itemId = InsertPortfolioItem(
                "TestItem",
                "Web Application",
                false);

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri($"https://localhost/v2.2/portfolio/{itemId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                itemId,
                new ItemModel
                {
                    Name = "TestItem Updated"
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
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.2/portfolio/1"));
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
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.2/portfolio/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new ItemModel
                {
                    Name = "TestItem Updated"
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
            int itemId = InsertPortfolioItem();

            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri($"https://localhost/v2.2/portfolio/{itemId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(itemId);

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
            PortfolioController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri("https://localhost/v2.2/portfolio/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
