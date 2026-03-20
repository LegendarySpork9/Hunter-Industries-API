// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Assistant;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Objects.Assistant;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Hunter_Industries_API.Tests.Controllers.Assistant
{
    [TestClass]
    public class ConfigControllerTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();
        private readonly Mock<IClock> _mockClock = new Mock<IClock>();

        [TestInitialize]
        public void Setup()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("select 1");
            _mockOptions.Setup(o => o.ConnectionString).Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQLFiles");
            _mockClock.Setup(c => c.DefaultDate).Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _mockClock.Setup(c => c.UtcNow).Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://localhost", null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        #region Get

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with config data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AssistantConfiguration> configs = new List<AssistantConfiguration>
            {
                new AssistantConfiguration
                {
                    AssistantName = "TestAssistant",
                    IdNumber = "A001",
                    AssignedUser = "TestUser",
                    HostName = "TestHost",
                    Deletion = false,
                    Version = "1.0.0"
                }
            };

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AssistantConfiguration>>(), It.IsAny<SqlParameter[]>()).Result).Returns((configs, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns(("2.0.0", null));

            ConfigController controller = new ConfigController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            AssistantFilterModel filters = new AssistantFilterModel()
            {
                AssistantName = "TestAssistant",
                AssistantId = "A001"
            };

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with an info message when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AssistantConfiguration> configs = new List<AssistantConfiguration>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AssistantConfiguration>>(), It.IsAny<SqlParameter[]>()).Result).Returns((configs, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ConfigController controller = new ConfigController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            AssistantFilterModel filters = new AssistantFilterModel();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a config is created successfully.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<(string, string)> existsResults = new List<(string, string)>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((existsResults, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1.0.0", null));

            ConfigController controller = new ConfigController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            ConfigModel request = new ConfigModel()
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                AssignedUser = "TestUser",
                HostName = "TestHost"
            };

            IHttpActionResult actionResult = await controller.Post(request);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Created, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the model is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            ConfigController controller = new ConfigController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(null);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.BadRequest, contentResult.StatusCode);
        }

        #endregion
    }
}
