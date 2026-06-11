// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using HunterIndustriesAPI.Objects.Statistics.Error;
using HunterIndustriesAPI.Objects.Statistics.Server;
using HunterIndustriesAPI.Objects.Statistics.Shared;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services
{
    [TestClass]
    public class StatisticServiceTest
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

        #region GetDashboardStatistic

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatistic()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new TopBarStatRecord
                        {
                            Applications = 1,
                            Users = 2,
                            Calls = new MonthlyStatRecord
                            {
                                ThisMonth = 3,
                                LastMonth = 2
                            },
                            LoginAttempts = new MonthlyStatRecord
                            {
                                ThisMonth = 4,
                                LastMonth = 3
                            },
                            Changes = new MonthlyStatRecord
                            {
                                ThisMonth = 5,
                                LastMonth = 4
                            },
                            Errors = new MonthlyStatRecord
                            {
                                ThisMonth = 6,
                                LastMonth = 5
                            }
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetDashboardStatistic("topBarStats");

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticUnknown()
        {
            Mock<IDatabase> mockDatabase = new();

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetDashboardStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        #endregion

        #region GetSharedStatistic

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatistic()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 10
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetSharedStatistic("endpointCalls");

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records when filtered by application.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticApplication()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 5
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetSharedStatistic(
                "endpointCalls",
                "application",
                1);

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records when filtered by user.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticUser()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 3
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetSharedStatistic(
                "endpointCalls",
                "user",
                1);

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticUnknown()
        {
            Mock<IDatabase> mockDatabase = new();

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetSharedStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        #endregion

        #region GetServerStatistic

        /// <summary>
        /// Checks whether the GetServerStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatistic()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new AlertComponentRecord
                        {
                            Component = "CPU",
                            Alerts = 3
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetServerStatistic(
                "componentAlerts",
                1);

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticUnknown()
        {
            Mock<IDatabase> mockDatabase = new();

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetServerStatistic(
                "unknown",
                1);

            Assert.AreEqual(
                0,
                records.Count);
        }

        #endregion

        #region GetErrorStatistic

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatistic()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new ErrorOverTimeRecord
                        {
                            Month = "January",
                            Errors = 5
                        }
                    ],
                    (Exception)null));

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetErrorStatistic("errorsOverTime");

            Assert.AreEqual(
                1,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticUnknown()
        {
            Mock<IDatabase> mockDatabase = new();

            StatisticService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> records = await service.GetErrorStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        #endregion
    }
}
