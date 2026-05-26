// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Functions;
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

namespace HunterIndustriesAPI.Tests.API.Controllers
{
    [TestClass]
    public class TokenControllerTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();
        private readonly Mock<IClock> _MockClock = new();

        [TestInitialize]
        public void Setup()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("select 1");
            _MockOptions.Setup(o => o.ConnectionString)
                .Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQLFiles");
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
        /// Checks whether the Post method returns a 400 when no body and no auth header are provided.
        /// </summary>
        [TestMethod]
        public async Task TestPostNullBody()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            TokenController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v1.0/auth/token")),
                Configuration = new HttpConfiguration()
            };

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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [("admin", "wronghash")],
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    ["testphrase"],
                    null));

            TokenController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v1.0/auth/token")),
                Configuration = new HttpConfiguration()
            };
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass")));

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

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [("testuser", hashedPassword)],
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    ["testphrase"],
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "TestApplication",
                    null));

            TokenController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v1.0/auth/token")),
                Configuration = new HttpConfiguration()
            };
            controller.Request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass")));

            IHttpActionResult actionResult = await controller.Post(new AuthenticationModel
            {
                Phrase = "testphrase"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }
    }
}
