// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Services.Assistant;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.Assistant
{
    [TestClass]
    public class LocationServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();

        [TestInitialize]
        public void Setup()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("select 1");
            _MockOptions.Setup(o => o.ConnectionString)
                .Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQLFiles");
        }

        #region GetAssistantLocation

        /// <summary>
        /// Checks whether the GetAssistantLocation method returns a populated location response model.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantLocation()
        {
            LocationResponseModel expected = new()
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                HostName = "TestHost",
                IPAddress = "192.168.1.1"
            };

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LocationResponseModel>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    expected,
                    null));

            LocationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);
            LocationResponseModel actual = await service.GetAssistantLocation(
                "TestAssistant",
                "A001");

            Assert.AreEqual(
                "TestAssistant",
                actual.AssistantName);
            Assert.AreEqual(
                "A001",
                actual.IdNumber);
            Assert.AreEqual(
                "TestHost",
                actual.HostName);
            Assert.AreEqual(
                "192.168.1.1",
                actual.IPAddress);
        }

        /// <summary>
        /// Checks whether the GetAssistantLocation method returns an empty model when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantLocationEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LocationResponseModel>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            LocationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);
            LocationResponseModel actual = await service.GetAssistantLocation(
                "TestAssistant",
                "A001");

            Assert.IsNull(actual.AssistantName);
            Assert.IsNull(actual.IdNumber);
            Assert.IsNull(actual.HostName);
            Assert.IsNull(actual.IPAddress);
        }

        #endregion

        #region AssistantLocationUpdated

        /// <summary>
        /// Checks whether the AssistantLocationUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantLocationUpdated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            LocationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);
            bool actual = await service.AssistantLocationUpdated(
                "TestAssistant",
                "A001",
                "NewHost",
                "10.0.0.1");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantLocationUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantLocationUpdatedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            LocationService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);
            bool actual = await service.AssistantLocationUpdated(
                "TestAssistant",
                "A001",
                "NewHost",
                "10.0.0.1");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
