// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.ServerStatus;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
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
    public class ServerEventControllerTest
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
        private ServerEventController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerEventController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverevent")),
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
        /// Inserts a component information record into the database.
        /// </summary>
        private void InsertComponentInformation(
            int serverInformationId,
            string component,
            string status)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ComponentInformation (ServerInformationId, ComponentId, ComponentStatusId, DateOccured) " +
                    "SELECT @serverId, C.ComponentId, CS.ComponentStatusId, GETUTCDATE() " +
                    "FROM Component C " +
                    "JOIN ComponentStatus CS ON CS.[Value] = @status " +
                    "WHERE C.[Name] = @component;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@serverId",
                        serverInformationId);
                    cmd.Parameters.AddWithValue(
                        "@component",
                        component);
                    cmd.Parameters.AddWithValue(
                        "@status",
                        status);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 with server events.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            InsertComponentInformation(
                serverId,
                "PC",
                "Online");

            ServerEventController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get("PC");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 with info when no events are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            ServerEventController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get("Unknown Component");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 when a server event is logged.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");

            ServerEventController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(new ServerEventModel
            {
                Component = "PC",
                Status = "Online",
                ServerId = serverId,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 when the model is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            ServerEventController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(new ServerEventModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }
    }
}
