// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPI.Services.ServerStatus;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.ServerStatus
{
    [TestClass]
    public class ServerAlertServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();

        [TestInitialize]
        public void Setup()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("select 1");
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns(@"C:\SQL");
        }

        #region GetServerAlerts

        /// <summary>
        /// Checks whether the GetServerAlerts method returns a list of alerts and the total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlerts()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerAlertRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new ServerAlertRecord
                        {
                            Id = 1,
                            Reporter = "System",
                            Component = "CPU",
                            ComponentStatus = "Critical",
                            AlertStatus = "Open",
                            AlertDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                            Server = new RelatedServerRecord
                            {
                                Id = 1,
                                Name = "Test",
                                HostName = "TestServer",
                                Game = "TestGame",
                                GameVersion = "1.0"
                            }
                        }
                    ],
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    5,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (List<ServerAlertRecord> alerts, int total) = await service.GetServerAlerts(
                10,
                1);

            Assert.AreEqual(
                1,
                alerts.Count);
            Assert.AreEqual(
                1,
                alerts[0].Id);
            Assert.AreEqual(
                "System",
                alerts[0].Reporter);
            Assert.AreEqual(
                "CPU",
                alerts[0].Component);
            Assert.AreEqual(
                5,
                total);
        }

        /// <summary>
        /// Checks whether the GetServerAlerts method returns an empty list and zero when no alerts are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlertsEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerAlertRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (List<ServerAlertRecord> alerts, int total) = await service.GetServerAlerts(
                10,
                1);

            Assert.AreEqual(
                0,
                alerts.Count);
            Assert.AreEqual(
                0,
                total);
        }

        #endregion

        #region GetServerAlert

        /// <summary>
        /// Checks whether the GetServerAlert method returns a populated record.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlert()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerAlertRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new ServerAlertRecord
                    {
                        Id = 1,
                        Reporter = "System",
                        Component = "CPU",
                        ComponentStatus = "Critical",
                        AlertStatus = "Open",
                        AlertDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        Server = new RelatedServerRecord
                        {
                            Name = "Test",
                            HostName = "TestServer",
                            Game = "TestGame",
                            GameVersion = "1.0"
                        }
                    },
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ServerAlertRecord actual = await service.GetServerAlert(1);

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "System",
                actual.Reporter);
            Assert.AreEqual(
                "CPU",
                actual.Component);
            Assert.AreEqual(
                "Critical",
                actual.ComponentStatus);
            Assert.AreEqual(
                "Open",
                actual.AlertStatus);
        }

        /// <summary>
        /// Checks whether the GetServerAlert method returns an empty record when no alert is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerAlertEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerAlertRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ServerAlertRecord actual = await service.GetServerAlert(1);

            Assert.AreEqual(
                0,
                actual.Id);
        }

        #endregion

        #region LogServerAlert

        /// <summary>
        /// Checks whether the LogServerAlert method returns true and the alert id when logged successfully.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerAlert()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool logged, int alertId) = await service.LogServerAlert(new ServerAlertModel
            {
                Reporter = "System",
                Component = "CPU",
                ComponentStatus = "Critical",
                AlertStatus = "Open",
                ServerId = 1,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            }, "UnitTests");

            Assert.IsTrue(logged);
            Assert.AreEqual(
                1,
                alertId);
        }

        /// <summary>
        /// Checks whether the LogServerAlert method returns false and zero when logging fails.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerAlertFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool logged, int alertId) = await service.LogServerAlert(new ServerAlertModel
            {
                Reporter = "System",
                Component = "CPU",
                ComponentStatus = "Critical",
                AlertStatus = "Open",
                ServerId = 1,
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            }, "UnitTests");

            Assert.IsFalse(logged);
            Assert.AreEqual(
                0,
                alertId);
        }

        #endregion

        #region ServerAlertExists (ServerId, Component)

        /// <summary>
        /// Checks whether the ServerAlertExists method returns true when an alert with the server id and component exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsByServerIdAndComponent()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertExists(
                1,
                "CPU");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns false when no alert with the server id and component exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsByServerIdAndComponentNot()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertExists(
                1,
                "CPU");

            Assert.IsFalse(actual);
        }

        #endregion

        #region ServerAlertExists (Id)

        /// <summary>
        /// Checks whether the ServerAlertExists method returns true when an alert exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExists()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertExists method returns false when no alert exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertExistsNot()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertExists(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region ServerAlertUpdated

        /// <summary>
        /// Checks whether the ServerAlertUpdated method returns true when the alert is updated.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertUpdated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertUpdated(
                1,
                "Closed");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerAlertUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerAlertUpdatedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            ServerAlertService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerAlertUpdated(
                1,
                "Closed");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
