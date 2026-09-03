// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Functions;
using HunterIndustriesAPICommon.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers
{
    [TestClass]
    [DoNotParallelize]
    public class TokenControllerTest
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

            Models.ValidationModel.Issuer = "http://localhost";
            Models.ValidationModel.Audience = "TestAudience";
            Models.ValidationModel.SecretKey = "ThisIsATestSecretKeyThatIsLongEnoughForHmacSha256!";

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
        private TokenController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            TokenController controller = new(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v1.0/auth/token")),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 when no body and no auth header are provided.
        /// </summary>
        [TestMethod]
        public async Task TestPostNullBody()
        {
            TokenController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 401 when credentials do not match.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidCredentials()
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
                    "SET IDENTITY_INSERT Application OFF; " +
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('testuser', 'wronghash', 0);",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            TokenController controller = CreateController();
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass")));

            IHttpActionResult actionResult = await controller.Post(new AuthenticationModel
            {
                Phrase = "testphrase"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 when valid credentials are provided.
        /// </summary>
        [TestMethod]
        public async Task TestPostValid()
        {
            string hashedPassword = HashFunction.HashString("testpass");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SET IDENTITY_INSERT Authorisation ON; " +
                    "INSERT INTO Authorisation (PhraseId, Phrase, IsDeleted) VALUES (1, 'testphrase', 0); " +
                    "SET IDENTITY_INSERT Authorisation OFF; " +
                    "SET IDENTITY_INSERT Application ON; " +
                    "INSERT INTO Application (ApplicationId, PhraseId, [Name], IsDeleted) VALUES (1, 1, 'TestApplication', 0); " +
                    "SET IDENTITY_INSERT Application OFF; " +
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('testuser', @hashedPassword, 0);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hashedPassword",
                        hashedPassword);
                    cmd.ExecuteNonQuery();
                }
            }

            TokenController controller = CreateController();
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass")));

            IHttpActionResult actionResult = await controller.Post(new AuthenticationModel
            {
                Phrase = "testphrase"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the body is missing the phrase field.
        /// </summary>
        [TestMethod]
        public async Task TestPostMissingPhrase()
        {
            TokenController controller = CreateController();
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass")));

            IHttpActionResult actionResult = await controller.Post(new AuthenticationModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }
    }
}
