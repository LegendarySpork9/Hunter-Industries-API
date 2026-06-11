// Copyright © 11/06/2026 Toby Hunter
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
    public class ServerInformationServiceTest
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

        #region GetServers

        /// <summary>
        /// Checks whether the GetServers method returns a list of servers.
        /// </summary>
        [TestMethod]
        public async Task TestGetServers()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new ServerInformationRecord
                        {
                            Id = 1,
                            Name = "Test",
                            HostName = "TestServer",
                            Game = "TestGame",
                            GameVersion = "1.0",
                            Connection = new ConnectionRecord { IPAddress = "127.0.0.1", Port = 25565 },
                            Downtime = new DowntimeRecord { Time = "03:00", Duration = 60 },
                            IsActive = true
                        }
                    ],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<ServerInformationRecord> actual = await service.GetServers(true);

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
            Assert.IsTrue(actual[0].IsActive);
        }

        /// <summary>
        /// Checks whether the GetServers method returns an empty list when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServersEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<ServerInformationRecord> actual = await service.GetServers(true);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region ServerExists

        /// <summary>
        /// Checks whether the ServerExists method returns true when a server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExists()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerExists("Test");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns false when no server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsNot()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerExists("Test");

            Assert.IsFalse(actual);
        }

        #endregion

        #region GetServer

        /// <summary>
        /// Checks whether the GetServer method returns a populated record.
        /// </summary>
        [TestMethod]
        public async Task TestGetServer()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new ServerInformationRecord
                    {
                        Id = 1,
                        Name = "Test",
                        HostName = "TestServer",
                        Game = "TestGame",
                        GameVersion = "1.0",
                        Connection = new ConnectionRecord { IPAddress = "127.0.0.1", Port = 25565 },
                        Downtime = new DowntimeRecord { Time = "03:00", Duration = 60 },
                        IsActive = true
                    },
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ServerInformationRecord actual = await service.GetServer(1);

            Assert.AreEqual(
                1,
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (ServerInformationRecord)null,
                    (Exception)null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ServerInformationRecord actual = await service.GetServer(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region ServerExists (int)

        /// <summary>
        /// Checks whether the ServerExists method returns true when a server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsId()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns false when no server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsIdNot()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerExists(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region ServerAdded

        /// <summary>
        /// Checks whether the ServerAdded method returns true and the server id when a server is added.
        /// </summary>
        [TestMethod]
        public async Task TestServerAdded()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool added, int serverId) = await service.ServerAdded(new ServerInformationModel
            {
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0",
                IPAddress = "127.0.0.1",
                Port = 25565,
                Time = "03:00",
                Duration = 60
            });

            Assert.IsTrue(added);
            Assert.AreEqual(
                1,
                serverId);
        }

        /// <summary>
        /// Checks whether the ServerAdded method returns false and zero when the addition fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerAddedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool added, int serverId) = await service.ServerAdded(new ServerInformationModel
            {
                Name = "Test",
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0",
                IPAddress = "127.0.0.1",
                Port = 25565,
                Time = "03:00",
                Duration = 60
            });

            Assert.IsFalse(added);
            Assert.AreEqual(
                0,
                serverId);
        }

        #endregion

        #region ServerUpdated

        /// <summary>
        /// Checks whether the ServerUpdated method returns true when the server is updated.
        /// </summary>
        [TestMethod]
        public async Task TestServerUpdated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerUpdated(
                1,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerUpdatedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            ServerInformationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ServerUpdated(
                1,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
