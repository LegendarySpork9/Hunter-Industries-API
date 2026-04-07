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
    public class VersionServiceTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();

        [TestInitialize]
        public void Setup()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("select 1");
            _mockOptions.Setup(o => o.ConnectionString).Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQLFiles");
        }

        #region GetAssistantVersion

        /// <summary>
        /// Checks whether the GetAssistantVersion method returns a populated version response model.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantVersion()
        {
            VersionResponseModel expected = new VersionResponseModel
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                Version = "2.5.0"
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, VersionResponseModel>>(), It.IsAny<SqlParameter[]>()).Result).Returns((expected, null));

            VersionService service = new VersionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            VersionResponseModel actual = await service.GetAssistantVersion("TestAssistant", "A001");

            Assert.AreEqual("TestAssistant", actual.AssistantName);
            Assert.AreEqual("A001", actual.IdNumber);
            Assert.AreEqual("2.5.0", actual.Version);
        }

        /// <summary>
        /// Checks whether the GetAssistantVersion method returns an empty model when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantVersionEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, VersionResponseModel>>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            VersionService service = new VersionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            VersionResponseModel actual = await service.GetAssistantVersion("TestAssistant", "A001");

            Assert.IsNull(actual.AssistantName);
            Assert.IsNull(actual.IdNumber);
            Assert.IsNull(actual.Version);
        }

        #endregion

        #region AssistantVersionUpdated

        /// <summary>
        /// Checks whether the AssistantVersionUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantVersionUpdated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            VersionService service = new VersionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantVersionUpdated("TestAssistant", "A001", "3.0.0");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantVersionUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantVersionUpdatedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            VersionService service = new VersionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantVersionUpdated("TestAssistant", "A001", "3.0.0");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
