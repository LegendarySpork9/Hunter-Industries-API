// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPI.Services.ServerStatus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.Services.ServerStatus
{
    [TestClass]
    public class ServerInformationServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _MockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _MockOptions = new Mock<IDatabaseOptions>();

        [TestInitialize]
        public void Setup()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("select 1");
            _MockOptions.Setup(o => o.SQLFiles).Returns(@"C:\SQL");
        }

        #region GetServers

        /// <summary>
        /// Checks whether the GetServers method returns a list of servers.
        /// </summary>
        [TestMethod]
        public async Task TestGetServers()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<ServerInformationRecord>
                {
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
                }, null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<ServerInformationRecord> actual = await service.GetServers(true);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Test", actual[0].Name);
            Assert.AreEqual("TestServer", actual[0].HostName);
            Assert.AreEqual("TestGame", actual[0].Game);
            Assert.AreEqual("1.0", actual[0].GameVersion);
            Assert.AreEqual("127.0.0.1", actual[0].Connection.IPAddress);
            Assert.AreEqual(25565, actual[0].Connection.Port);
            Assert.AreEqual("03:00", actual[0].Downtime.Time);
            Assert.AreEqual(60, actual[0].Downtime.Duration);
            Assert.IsTrue(actual[0].IsActive);
        }

        /// <summary>
        /// Checks whether the GetServers method returns an empty list when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServersEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<ServerInformationRecord>(), null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<ServerInformationRecord> actual = await service.GetServers(true);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region ServerExists

        /// <summary>
        /// Checks whether the ServerExists method returns true when a server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExists()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.ServerExists("Test");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ServerExists method returns false when no server exists.
        /// </summary>
        [TestMethod]
        public async Task TestServerExistsNot()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.ServerExists("Test");

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
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

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
            Assert.AreEqual(1, serverId);
        }

        /// <summary>
        /// Checks whether the ServerAdded method returns false and zero when the addition fails.
        /// </summary>
        [TestMethod]
        public async Task TestServerAddedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ServerInformationService service = new ServerInformationService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

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
            Assert.AreEqual(0, serverId);
        }

        #endregion
    }
}
