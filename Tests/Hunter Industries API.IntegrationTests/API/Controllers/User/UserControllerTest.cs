// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.User;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Models.Requests.Filters;
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

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers.User
{
    [TestClass]
    [DoNotParallelize]
    public class UserControllerTest
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
        private UserController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            UserController controller = new(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/user")),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }

        /// <summary>
        /// Inserts a user record and returns the generated user ID.
        /// </summary>
        private int InsertUser(
            string username,
            string password,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) OUTPUT inserted.UserId VALUES (@username, @password, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@password",
                        password);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted ? 1 : 0);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a user scope record into the database.
        /// </summary>
        private void InsertUserScope(
            int userId,
            int scopeId)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new(
                    "INSERT INTO UserScope (UserId, ScopeId) VALUES (@userId, @scopeId)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        userId);
                    cmd.Parameters.AddWithValue(
                        "@scopeId",
                        scopeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of users.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            int userId = InsertUser(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);

            UserController controller = CreateController();

            UserFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no users are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            UserController controller = CreateController();

            UserFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with user data.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            int userId = InsertUser(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);

            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(userId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with information when no user is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a user is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            UserController controller = CreateController();

            UserModel body = new()
            {
                Username = "NewUser",
                Password = "Password123",
                Scopes = ["User"]
            };

            IHttpActionResult actionResult = await controller.Post(body);

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
            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the user is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            int userId = InsertUser(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);

            UserController controller = CreateController();

            UserModel body = new()
            {
                Password = "NewPassword"
            };

            IHttpActionResult actionResult = await controller.Patch(
                userId,
                body);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            UserController controller = CreateController();

            UserModel body = new()
            {
                Password = "NewPassword"
            };

            IHttpActionResult actionResult = await controller.Patch(999, body);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the user is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            int userId = InsertUser(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);

            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Delete(userId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 404 status code when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteNotFound()
        {
            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 status code when a user with the username already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPostAlreadyExists()
        {
            InsertUser(
                "ExistingUser",
                "HashedPassword");

            UserController controller = CreateController();

            UserModel body = new()
            {
                Username = "ExistingUser",
                Password = "Password123",
                Scopes = ["User"]
            };

            IHttpActionResult actionResult = await controller.Post(body);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            UserController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }
    }
}
