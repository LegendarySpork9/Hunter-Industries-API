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

namespace Hunter_Industries_API.Tests.Services.ServerStatus
{
    [TestClass]
    public class ServerEventServiceTest
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

        #region GetServerEvents

        /// <summary>
        /// Checks whether the GetServerEvents method returns a list of events.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerEvents()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ServerEventRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<ServerEventRecord>
                {
                    new ServerEventRecord
                    {
                        Component = "CPU",
                        Status = "Online",
                        DateOccured = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        Server = new RelatedServerRecord
                        {
                            Id = 1,
                            HostName = "TestServer",
                            Game = "TestGame",
                            GameVersion = "1.0"
                        }
                    }
                }, null));

            ServerEventService service = new ServerEventService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<ServerEventRecord> actual = await service.GetServerEvents("CPU");

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("CPU", actual[0].Component);
            Assert.AreEqual("Online", actual[0].Status);
            Assert.AreEqual("TestServer", actual[0].Server.HostName);
        }

        /// <summary>
        /// Checks whether the GetServerEvents method returns an empty list when no events are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerEventsEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, ServerEventRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<ServerEventRecord>(), null));

            ServerEventService service = new ServerEventService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<ServerEventRecord> actual = await service.GetServerEvents("CPU");

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region LogServerEvent

        /// <summary>
        /// Checks whether the LogServerEvent method returns true and the event id when logged successfully.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerEvent()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ServerEventService service = new ServerEventService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            (bool logged, int eventId) = await service.LogServerEvent(new ServerEventModel
            {
                Component = "CPU",
                Status = "Online",
                ServerId = 1,
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            Assert.IsTrue(logged);
            Assert.AreEqual(1, eventId);
        }

        /// <summary>
        /// Checks whether the LogServerEvent method returns false and zero when logging fails.
        /// </summary>
        [TestMethod]
        public async Task TestLogServerEventFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ServerEventService service = new ServerEventService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            (bool logged, int eventId) = await service.LogServerEvent(new ServerEventModel
            {
                Component = "CPU",
                Status = "Online",
                ServerId = 1,
                HostName = "TestServer",
                Game = "TestGame",
                GameVersion = "1.0"
            });

            Assert.IsFalse(logged);
            Assert.AreEqual(0, eventId);
        }

        #endregion
    }
}
