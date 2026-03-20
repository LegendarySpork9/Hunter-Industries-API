// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.User;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Objects.User;
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

namespace Hunter_Industries_API.Tests.Controllers.User
{
    [TestClass]
    public class UserSettingsControllerTest
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
        /// Checks whether the Get method returns a 200 status code with settings data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (string, int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(string, int, string, string)> { ("TestApp", 1, "Theme", "Dark") }, null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(1, "TestApp");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no settings are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (string, int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(string, int, string, string)>(), null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(1, null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a setting is added.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(new UserSettingsModel
            {
                Username = "TestUser",
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.Created, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.BadRequest, contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the setting is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, SettingRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new SettingRecord { Id = 1, Name = "Theme", Value = "Dark" }, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(1, new SettingUpdateModel { Value = "Light" });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserSettingsController controller = new UserSettingsController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(999, new SettingUpdateModel { Value = "Light" });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.NotFound, contentResult.StatusCode);
        }

        #endregion
    }
}
