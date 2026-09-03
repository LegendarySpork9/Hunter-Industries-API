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
    public class ServerInformationControllerTest
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
        private ServerInformationController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerInformationController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverinformation")),
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
            string gameVersion,
            string ipAddress,
            int port,
            string webhookURL,
            long recipientId,
            bool isActive,
            string downtimeTime = null,
            int downtimeDuration = 0)
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
                    "IF NOT EXISTS (SELECT 1 FROM [Connection] WHERE IPAddress = @ipAddress AND [Port] = @port) " +
                    "BEGIN INSERT INTO [Connection] (IPAddress, [Port], IsDeleted) VALUES (@ipAddress, @port, 0) END;",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@port",
                        port);
                    cmd.ExecuteNonQuery();
                }

                int? downtimeId = null;

                if (downtimeTime != null)
                {
                    using (SqlCommand cmd = new(
                        "IF NOT EXISTS (SELECT 1 FROM Downtime WHERE [Time] = @time AND Duration = @duration) " +
                        "BEGIN INSERT INTO Downtime ([Time], Duration, IsDeleted) VALUES (@time, @duration, 0) END; " +
                        "SELECT DowntimeId FROM Downtime WHERE [Time] = @time AND Duration = @duration;",
                        conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@time",
                            downtimeTime);
                        cmd.Parameters.AddWithValue(
                            "@duration",
                            downtimeDuration);
                        downtimeId = (int)cmd.ExecuteScalar();
                    }
                }

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerInformation ([Name], EventInterval, WebhookURL, RecipientId, MachineId, GameId, ConnectionId, DowntimeId, IsActive) " +
                    "SELECT @name, 300, @webhookURL, @recipientId, M.MachineId, G.GameId, C.ConnectionId, @downtimeId, @isActive " +
                    "FROM Machine M " +
                    "JOIN Game G ON G.[Name] = @game AND G.[Version] = @gameVersion " +
                    "JOIN [Connection] C ON C.IPAddress = @ipAddress AND C.[Port] = @port " +
                    "WHERE M.HostName = @hostName; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@webhookURL",
                        webhookURL);
                    cmd.Parameters.AddWithValue(
                        "@recipientId",
                        recipientId);
                    cmd.Parameters.AddWithValue(
                        "@isActive",
                        isActive);
                    cmd.Parameters.AddWithValue(
                        "@downtimeId",
                        (object)downtimeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@game",
                        game);
                    cmd.Parameters.AddWithValue(
                        "@gameVersion",
                        gameVersion);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@port",
                        port);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of servers.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            InsertServerInformation(
                "Test",
                "TestServer",
                "Minecraft",
                "1.7.10",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true,
                "02:00:00",
                60);

            ServerInformationController controller = CreateController();

            ServerInformationFilterModel filters = new()
            {
                IsActive = true
            };

            IHttpActionResult actionResult = await controller.Get(filters);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            ServerInformationController controller = CreateController();

            ServerInformationFilterModel filters = new()
            {
                IsActive = true
            };

            IHttpActionResult actionResult = await controller.Get(filters);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a server is added.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Machine (HostName, IsDeleted) VALUES ('TestServer', 0); " +
                    "INSERT INTO Game ([Name], [Version], IsDeleted) VALUES ('Minecraft', '1.7.10', 0); " +
                    "INSERT INTO [Connection] (IPAddress, [Port], IsDeleted) VALUES ('127.0.0.1', 25565, 0); " +
                    "INSERT INTO Downtime ([Time], Duration, IsDeleted) VALUES ('02:00:00', 60, 0);", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            ServerInformationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(new ServerInformationModel
            {
                Name = "Test",
                HostName = "TestServer",
                Game = "Minecraft",
                GameVersion = "1.7.10",
                IPAddress = "127.0.0.1",
                Port = 25565,
                WebhookURL = "https://discord.com/api/webhooks/test",
                RecipientId = 123456789,
                Time = "02:00:00",
                Duration = 60
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
            ServerInformationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the server is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "Minecraft",
                "1.7.10",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                false,
                "02:00:00",
                60);

            ServerInformationController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri($"https://localhost/v2.0/serverstatus/serverinformation/{serverId}"));

            IHttpActionResult actionResult = await controller.Patch(
                serverId,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the model is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            ServerInformationController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.0/serverstatus/serverinformation/1"));

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            ServerInformationController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.0/serverstatus/serverinformation/999"));

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when a server with the name already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNameExists()
        {
            InsertServerInformation(
                "ExistingServer",
                "ExistingHost",
                "Minecraft",
                "1.7.10",
                "192.168.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true);

            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "Minecraft",
                "1.7.10",
                "127.0.0.1",
                25566,
                "https://discord.com/api/webhooks/test2",
                987654321,
                true);

            ServerInformationController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri($"https://localhost/v2.0/serverstatus/serverinformation/{serverId}"));

            IHttpActionResult actionResult = await controller.Patch(
                serverId,
                new ServerUpdateModel
                {
                    Name = "ExistingServer"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

    }
}
