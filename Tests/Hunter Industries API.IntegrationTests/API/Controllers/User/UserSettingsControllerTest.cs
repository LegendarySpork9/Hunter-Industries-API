// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.User;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Objects.User;
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
    public class UserSettingsControllerTest
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
        private UserSettingsController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            UserSettingsController controller = new(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/usersettings/1")),
                Configuration = new HttpConfiguration()
            };

            return controller;
        }

        /// <summary>
        /// Inserts prerequisite test data for user settings testing.
        /// </summary>
        private void InsertTestData(
            string phrase,
            string applicationName,
            string username,
            string password)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES (@phrase, 0); " +
                    "DECLARE @phraseId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @appName, 0); " +
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES (@username, @password, 0);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phrase",
                        phrase);
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@password",
                        password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves the user ID for the given username.
        /// </summary>
        private int GetUserId(string username)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT UserId FROM APIUser WHERE Username = @username",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a user setting record and returns the generated ID.
        /// </summary>
        private int InsertUserSetting(
            int userId,
            string applicationName,
            string settingName,
            string settingValue)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO UserSetting (UserId, ApplicationId, [Name], [Value]) " +
                    "SELECT @userId, ApplicationId, @name, @value " +
                    "FROM [Application] WHERE [Name] = @appName; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        userId);
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        settingName);
                    cmd.Parameters.AddWithValue(
                        "@value",
                        settingValue);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with settings data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(userId, "TestApp");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no settings are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            UserSettingsController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(1, null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a setting is added.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");

            UserSettingsController controller = CreateController();

            UserSettingsModel body = new()
            {
                UserId = userId,
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
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
            UserSettingsController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the setting is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            int settingId = InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsController controller = CreateController();

            SettingUpdateModel body = new() { Value = "Light" };

            IHttpActionResult actionResult = await controller.Patch(settingId, body);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            UserSettingsController controller = CreateController();

            SettingUpdateModel body = new() { Value = "Light" };

            IHttpActionResult actionResult = await controller.Patch(999, body);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

    }
}
