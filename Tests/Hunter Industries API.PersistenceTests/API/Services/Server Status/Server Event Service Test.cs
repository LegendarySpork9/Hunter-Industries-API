// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.ServerStatus;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.ServerStatus
{
    [TestClass]
    [DoNotParallelize]
    public class ServerEventServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
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
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private ServerEventService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerEventService(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);
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
            int port)
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

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerInformation ([Name], EventInterval, WebhookURL, RecipientId, MachineId, GameId, ConnectionId, IsActive) " +
                    "SELECT @name, 300, 'https://discord.com/api/webhooks/test', 123456789, M.MachineId, G.GameId, C.ConnectionId, 1 " +
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
        /// Checks whether the GetServerEvents method returns a list of events.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerEvents()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565);

            InsertComponentInformation(
                serverId,
                "PC",
                "Online");

            ServerEventService service = CreateService();

            List<ServerEventRecord> actual = await service.GetServerEvents("PC");

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "PC",
                actual[0].Component);
            Assert.AreEqual(
                "Online",
                actual[0].Status);
            Assert.AreEqual(
                "Test",
                actual[0].Server.Name);
        }

        /// <summary>
        /// Checks whether the GetServerEvents method returns an empty list when no events are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerEventsEmpty()
        {
            ServerEventService service = CreateService();

            List<ServerEventRecord> actual = await service.GetServerEvents("PC");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the LogServerEvent method returns true and the event id when logged successfully.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerEvent()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565);

            ServerEventService service = CreateService();

            (bool logged, int eventId) = await service.LogServerEvent(new ServerEventModel
            {
                Component = "PC",
                Status = "Online",
                ServerId = serverId,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            Assert.IsTrue(logged);
            Assert.IsTrue(eventId > 0);
        }

        /// <summary>
        /// Checks whether the LogServerEvent method returns false and zero when logging fails.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerEventFailed()
        {
            ServerEventService service = CreateService();

            (bool logged, int eventId) = await service.LogServerEvent(new ServerEventModel
            {
                Component = "NonExistentComponent",
                Status = "NonExistentStatus",
                ServerId = 999,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            Assert.IsFalse(logged);
            Assert.AreEqual(
                0,
                eventId);
        }

    }
}
