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
    public class ServerAlertServiceTest
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
        private ServerAlertService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ServerAlertService(
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
        /// Checks whether the GetServerAlerts method returns a list of alerts and the total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlerts()
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

            ServerAlertService service = CreateService();

            (List<ServerAlertRecord> alerts, int total) = await service.GetServerAlerts(
                null,
                10,
                1);

            Assert.AreEqual(
                1,
                alerts.Count);
            Assert.IsTrue(alerts[0].Id > 0);
            Assert.AreEqual(
                "System",
                alerts[0].Reporter);
            Assert.AreEqual(
                "PC",
                alerts[0].Component);
            Assert.AreEqual(
                1,
                total);
        }

        /// <summary>
        /// Checks whether the GetServerAlerts method returns an empty list and zero when no alerts are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlertsEmpty()
        {
            ServerAlertService service = CreateService();

            (List<ServerAlertRecord> alerts, int total) = await service.GetServerAlerts(
                null,
                10,
                1);

            Assert.AreEqual(
                0,
                alerts.Count);
            Assert.AreEqual(
                0,
                total);
        }

        /// <summary>
        /// Checks whether the GetServerAlerts method returns filtered results when a server name is provided.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlertsFiltered()
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

            ServerAlertService service = CreateService();

            (List<ServerAlertRecord> alerts, int total) = await service.GetServerAlerts(
                "Test",
                10,
                1);

            Assert.AreEqual(
                1,
                alerts.Count);
            Assert.AreEqual(
                "Test",
                alerts[0].Server.Name);
            Assert.AreEqual(
                1,
                total);
        }

        /// <summary>
        /// Checks whether the GetServerAlert method returns a populated record.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlert()
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

            ServerAlertService service = CreateService();

            ServerAlertRecord actual = await service.GetServerAlert(alertId);

            Assert.AreEqual(
                alertId,
                actual.Id);
            Assert.AreEqual(
                "System",
                actual.Reporter);
            Assert.AreEqual(
                "PC",
                actual.Component);
            Assert.AreEqual(
                "Offline",
                actual.ComponentStatus);
            Assert.AreEqual(
                "Reported",
                actual.AlertStatus);
        }

        /// <summary>
        /// Checks whether the GetServerAlert method returns an empty record when no alert is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlertEmpty()
        {
            ServerAlertService service = CreateService();

            ServerAlertRecord actual = await service.GetServerAlert(999);

            Assert.AreEqual(
                0,
                actual.Id);
        }

        /// <summary>
        /// Checks whether the LogServerAlert method returns true and the alert id when logged successfully.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerAlert()
        {
            int serverId = InsertServerInformation(
                "Test",
                "TestServer",
                "TestGame",
                "1.0");
            InsertUserSetting(
                "System",
                "UnitTests");

            ServerAlertService service = CreateService();

            (bool logged, int alertId) = await service.LogServerAlert(new ServerAlertModel
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
            }, "UnitTests");

            Assert.IsTrue(logged);
            Assert.IsTrue(alertId > 0);
        }

        /// <summary>
        /// Checks whether the LogServerAlert method returns false and zero when logging fails.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerAlertFailed()
        {
            ServerAlertService service = CreateService();

            (bool logged, int alertId) = await service.LogServerAlert(new ServerAlertModel
            {
                Reporter = "NonExistent",
                Component = "NonExistent",
                ComponentStatus = "NonExistent",
                AlertStatus = "NonExistent",
                ServerId = 999,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            }, "NonExistent");

            Assert.IsFalse(logged);
            Assert.AreEqual(
                0,
                alertId);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns true when an alert with the server id and component exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsByServerIdAndComponent()
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

            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertExists(
                serverId,
                "PC");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns false when no alert with the server id and component exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsByServerIdAndComponentNot()
        {
            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertExists(
                999,
                "PC");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns true when an alert exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExists()
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

            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertExists(alertId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns false when no alert exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsNot()
        {
            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertUpdated method returns true when the alert is updated.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertUpdated()
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

            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertUpdated(
                alertId,
                "Resolved");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertUpdatedFailed()
        {
            ServerAlertService service = CreateService();

            bool actual = await service.ServerAlertUpdated(
                999,
                "Resolved");

            Assert.IsFalse(actual);
        }

    }
}
