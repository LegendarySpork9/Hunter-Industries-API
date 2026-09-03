// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.ServerStatus;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
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

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers.ServerStatus
{
    [TestClass]
    [DoNotParallelize]
    public class ServerAlertControllerTest
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
                new HttpRequest(
                    null,
                    "http://localhost",
                    null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        /// <summary>
        /// Creates a controller instance with real database dependencies for testing.
        /// </summary>
        private ServerAlertController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerAlertController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serveralert")),
                Configuration = new HttpConfiguration()
            };
        }

        /// <summary>
        /// Inserts server information and returns the generated ID.
        /// </summary>
        private int InsertServerInformation(
            string name,
            string hostName,
            string game,
            string gameVersion)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Machine WHERE HostName = @hostName) " +
                    "BEGIN INSERT INTO Machine (HostName, IsDeleted) VALUES (@hostName, 0) END;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Game WHERE [Name] = @game AND [Version] = @gameVersion) " +
                    "BEGIN INSERT INTO Game ([Name], [Version], IsDeleted) VALUES (@game, @gameVersion, 0) END;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@game",
                        game);
                    cmd.Parameters.AddWithValue(
                        "@gameVersion",
                        gameVersion);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM [Connection] WHERE IPAddress = '127.0.0.1' AND [Port] = 25565) " +
                    "BEGIN INSERT INTO [Connection] (IPAddress, [Port], IsDeleted) VALUES ('127.0.0.1', 25565, 0) END;",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerInformation ([Name], EventInterval, WebhookURL, RecipientId, MachineId, GameId, ConnectionId, IsActive) " +
                    "SELECT @name, 300, 'https://discord.com/api/webhooks/test', 123456789, M.MachineId, G.GameId, C.ConnectionId, 1 " +
                    "FROM Machine M " +
                    "JOIN Game G ON G.[Name] = @game AND G.[Version] = @gameVersion " +
                    "JOIN [Connection] C ON C.IPAddress = '127.0.0.1' AND C.[Port] = 25565 " +
                    "WHERE M.HostName = @hostName; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@game",
                        game);
                    cmd.Parameters.AddWithValue(
                        "@gameVersion",
                        gameVersion);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a user setting record and returns the generated ID.
        /// </summary>
        private int InsertUserSetting(
            string reporter,
            string applicationName)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('testuser', 'testpass', 0); " +
                    "DECLARE @userId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('testphrase', 0); " +
                    "DECLARE @phraseId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @applicationName, 0); " +
                    "DECLARE @appId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO UserSetting (UserId, ApplicationId, [Name], [Value]) VALUES (@userId, @appId, 'DiscordName', @reporter); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@reporter",
                        reporter);
                    cmd.Parameters.AddWithValue(
                        "@applicationName",
                        applicationName);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a server alert record and returns the generated ID.
        /// </summary>
        private int InsertServerAlert(
            int serverInformationId,
            int userSettingId,
            string component,
            string componentStatus,
            string alertStatus)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerAlert (ServerInformationId, UserSettingId, ComponentId, ComponentStatusId, AlertStatusId, DateOccured) " +
                    "SELECT @serverId, @userSettingId, C.ComponentId, CS.ComponentStatusId, SAS.AlertStatusId, GETUTCDATE() " +
                    "FROM Component C " +
                    "JOIN ComponentStatus CS ON CS.[Value] = @componentStatus " +
                    "JOIN ServerAlertStatus SAS ON SAS.[Value] = @alertStatus " +
                    "WHERE C.[Name] = @component; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@serverId",
                        serverInformationId);
                    cmd.Parameters.AddWithValue(
                        "@userSettingId",
                        userSettingId);
                    cmd.Parameters.AddWithValue(
                        "@component",
                        component);
                    cmd.Parameters.AddWithValue(
                        "@componentStatus",
                        componentStatus);
                    cmd.Parameters.AddWithValue(
                        "@alertStatus",
                        alertStatus);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 with server alerts.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            int userSettingId = InsertUserSetting(
                "System",
                "UnitTests");
            InsertServerAlert(
                serverId,
                userSettingId,
                "PC",
                "Offline",
                "Reported");

            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(new ServerAlertFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 with info when no alerts are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(new ServerAlertFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 with server alerts when filtered by server name.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltered()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            int userSettingId = InsertUserSetting(
                "System",
                "UnitTests");
            InsertServerAlert(
                serverId,
                userSettingId,
                "PC",
                "Offline",
                "Reported");

            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(new ServerAlertFilterModel
            {
                ServerName = "Test"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by ID method returns a 200 with the server alert.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            int userSettingId = InsertUserSetting(
                "System",
                "UnitTests");
            int alertId = InsertServerAlert(
                serverId,
                userSettingId,
                "PC",
                "Offline",
                "Reported");

            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(alertId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by ID method returns a 200 with info when no alert is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 when a server alert is logged.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            InsertUserSetting(
                "System",
                "UnitTests");

            ServerAlertController controller = CreateController();
            controller.RequestContext.Principal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("application", "UnitTests"),
                    new System.Security.Claims.Claim("username", "testuser")
                }));

            IHttpActionResult actionResult = await controller.Post(new ServerAlertModel
            {
                Reporter = "System",
                Component = "PC",
                ComponentStatus = "Offline",
                AlertStatus = "Reported",
                ServerId = serverId,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.IsNotNull(
                contentResult,
                $"contentResult null, type={actionResult?.GetType().Name}");
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode,
                $"Body: {Newtonsoft.Json.JsonConvert.SerializeObject(contentResult.Content)}");
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 when the server alert already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPostAlertExists()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            int userSettingId = InsertUserSetting(
                "System",
                "UnitTests");
            InsertServerAlert(
                serverId,
                userSettingId,
                "PC",
                "Offline",
                "Reported");

            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(new ServerAlertModel
            {
                Reporter = "System",
                Component = "PC",
                ComponentStatus = "Offline",
                AlertStatus = "Reported",
                ServerId = serverId,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 when the model is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(new ServerAlertModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 200 when the alert is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            int userSettingId = InsertUserSetting(
                "System",
                "UnitTests");
            int alertId = InsertServerAlert(
                serverId,
                userSettingId,
                "PC",
                "Offline",
                "Reported");

            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(alertId, new AlertUpdateModel
            {
                Status = "Resolved"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 when the alert is not found.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            ServerAlertController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(999, new AlertUpdateModel
            {
                Status = "Resolved"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
