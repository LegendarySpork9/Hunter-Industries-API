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
    public class ServerInformationServiceTest
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
        private ServerInformationService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerInformationService(
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
            int port,
            string webhookURL,
            long recipientId,
            bool isActive,
            string downtimeTime = null,
            int downtimeDuration = 0,
            int eventInterval = 300)
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
                    "SELECT @name, @eventInterval, @webhookURL, @recipientId, M.MachineId, G.GameId, C.ConnectionId, @downtimeId, @isActive " +
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
                        "@eventInterval",
                        eventInterval);
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
                        (object)downtimeId ?? System.DBNull.Value);
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
        /// Checks whether the GetServers method returns a list of servers.
        /// </summary>
        [TestMethod]
        public async Task TestGetServers()
        {
            InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true,
                "03:00",
                60);

            ServerInformationService service = CreateService();

            (List<ServerInformationRecord> actual, int totalRecords) = await service.GetServers(true);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "Test",
                actual[0].Name);
            Assert.AreEqual(
                "TestServer",
                actual[0].HostName);
            Assert.AreEqual(
                "TestGame",
                actual[0].Game);
            Assert.AreEqual(
                "1.0",
                actual[0].GameVersion);
            Assert.AreEqual(
                "127.0.0.1",
                actual[0].Connection.IPAddress);
            Assert.AreEqual(
                25565,
                actual[0].Connection.Port);
            Assert.AreEqual(
                "03:00",
                actual[0].Downtime.Time);
            Assert.AreEqual(
                60,
                actual[0].Downtime.Duration);
            Assert.AreEqual(
                "https://discord.com/api/webhooks/test",
                actual[0].WebhookURL);
            Assert.AreEqual(
                123456789,
                actual[0].RecipientId);
            Assert.IsTrue(actual[0].IsActive);
            Assert.AreEqual(
                1,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetServers method returns an empty list when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServersEmpty()
        {
            ServerInformationService service = CreateService();

            (List<ServerInformationRecord> actual, int totalRecords) = await service.GetServers(true);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns true when a server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExists()
        {
            InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true);

            ServerInformationService service = CreateService();

            bool actual = await service.ServerExists("Test");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns false when no server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsNot()
        {
            ServerInformationService service = CreateService();

            bool actual = await service.ServerExists("Test");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the GetServer method returns a populated record.
        /// </summary>
        [TestMethod]
        public async Task TestGetServer()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true,
                "03:00",
                60);

            ServerInformationService service = CreateService();

            ServerInformationRecord actual = await service.GetServer(serverId);

            Assert.AreEqual(
                serverId,
                actual.Id);
            Assert.AreEqual(
                "Test",
                actual.Name);
            Assert.AreEqual(
                "TestServer",
                actual.HostName);
        }

        /// <summary>
        /// Checks whether the GetServer method returns null when no server is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerEmpty()
        {
            ServerInformationService service = CreateService();

            ServerInformationRecord actual = await service.GetServer(999);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns true when a server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsId()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true);

            ServerInformationService service = CreateService();

            bool actual = await service.ServerExists(serverId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns false when no server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsIdNot()
        {
            ServerInformationService service = CreateService();

            bool actual = await service.ServerExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ServerAdded method returns true and the server id when a server is added.
        /// </summary>
        [TestMethod]
        public async Task TestServerAdded()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Machine (HostName, IsDeleted) VALUES ('TestServer', 0); " +
                    "INSERT INTO Game ([Name], [Version], IsDeleted) VALUES ('TestGame', '1.0', 0); " +
                    "INSERT INTO [Connection] (IPAddress, [Port], IsDeleted) VALUES ('127.0.0.1', 25565, 0); " +
                    "INSERT INTO Downtime ([Time], Duration, IsDeleted) VALUES ('03:00', 60, 0);",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            ServerInformationService service = CreateService();

            (bool added, int serverId) = await service.ServerAdded(new ServerInformationModel
            {
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0",
                IPAddress = "127.0.0.1",
                Port = 25565,
                WebhookURL = "https://discord.com/api/webhooks/test",
                RecipientId = 123456789,
                Time = "03:00",
                Duration = 60
            });

            Assert.IsTrue(added);
            Assert.IsTrue(serverId > 0);
        }

        /// <summary>
        /// Checks whether the ServerAdded method returns false and zero when the addition fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerAddedFailed()
        {
            ServerInformationService service = CreateService();

            (bool added, int serverId) = await service.ServerAdded(new ServerInformationModel
            {
                Name = "Test",
                HostName = "NonExistentHost",
                Game = "NonExistentGame",
                GameVersion = "1.0",
                IPAddress = "192.168.0.1",
                Port = 99999,
                WebhookURL = "https://discord.com/api/webhooks/test",
                RecipientId = 123456789,
                Time = "03:00",
                Duration = 60
            });

            Assert.IsFalse(added);
            Assert.AreEqual(
                0,
                serverId);
        }

        /// <summary>
        /// Checks whether the ServerUpdated method returns true when the server is updated.
        /// </summary>
        [TestMethod]
        public async Task TestServerUpdated()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0",
                "127.0.0.1",
                25565,
                "https://discord.com/api/webhooks/test",
                123456789,
                true,
                "03:00",
                60);

            ServerInformationService service = CreateService();

            bool actual = await service.ServerUpdated(
                serverId,
                new ServerUpdateModel
                {
                    IsActive = false,
                    HostName = "TestServer",
                    Game = "TestGame",
                    GameVersion = "1.0",
                    IPAddress = "127.0.0.1",
                    Port = 25565,
                    Time = "03:00",
                    Duration = 60
                });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerUpdatedFailed()
        {
            ServerInformationService service = CreateService();

            bool actual = await service.ServerUpdated(
                999,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            Assert.IsFalse(actual);
        }

    }
}
