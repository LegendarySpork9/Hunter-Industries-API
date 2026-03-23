// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.User;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Models.Requests.Filters;
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

namespace HunterIndustriesAPI.Tests.Controllers.User
{
    [TestClass]
    public class UserControllerTest
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
        /// Checks whether the Get method returns a 200 status code with a list of users.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)> { (1, "TestUser", "HashedPassword") }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string> { "User", "Assistant API" }, null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(new UserFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no users are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)>(), null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(new UserFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with user data.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)> { (1, "TestUser", "HashedPassword") }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string> { "User" }, null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with information when no user is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)>(), null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a user is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(new UserModel
            {
                Username = "NewUser",
                Password = "Password123",
                Scopes = new List<string> { "User" }
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

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.BadRequest, contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the user is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)> { (1, "TestUser", "HashedPassword") }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string> { "User" }, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(1, new UserModel
            {
                Password = "NewPassword"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(999, new UserModel
            {
                Password = "NewPassword"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.NotFound, contentResult.StatusCode);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the user is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 404 status code when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteNotFound()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserController controller = new UserController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new System.Uri("https://localhost/api/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.NotFound, contentResult.StatusCode);
        }

        #endregion
    }
}
