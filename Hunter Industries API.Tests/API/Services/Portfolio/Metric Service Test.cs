// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.Portfolio
{
    [TestClass]
    public class MetricServiceTest
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

        /// <summary>
        /// Checks whether the MetricUpdated method returns true when the update is successful.
        /// </summary>
        [TestMethod]
        public async Task TestMetricUpdated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));

            MetricService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MetricUpdated(
                new ItemMetricModel
                {
                    Id = 1,
                    Metric = "summary"
                });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MetricUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestMetricUpdatedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    (Exception)null));

            MetricService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MetricUpdated(
                new ItemMetricModel
                {
                    Id = 1,
                    Metric = "summary"
                });

            Assert.IsFalse(actual);
        }
    }
}
