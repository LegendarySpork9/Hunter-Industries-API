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
    public class DeletionServiceTest
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

        #region GetAssistantDeletion

        /// <summary>
        /// Checks whether the GetAssistantDeletion method returns a populated deletion response model.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantDeletion()
        {
            DeletionResponseModel expected = new DeletionResponseModel
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                Deletion = true
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, DeletionResponseModel>>(), It.IsAny<SqlParameter[]>()).Result).Returns((expected, null));

            DeletionService service = new DeletionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            DeletionResponseModel actual = await service.GetAssistantDeletion("TestAssistant", "A001");

            Assert.AreEqual("TestAssistant", actual.AssistantName);
            Assert.AreEqual("A001", actual.IdNumber);
            Assert.IsTrue(actual.Deletion);
        }

        /// <summary>
        /// Checks whether the GetAssistantDeletion method returns an empty model when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantDeletionEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, DeletionResponseModel>>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            DeletionService service = new DeletionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            DeletionResponseModel actual = await service.GetAssistantDeletion("TestAssistant", "A001");

            Assert.IsNull(actual.AssistantName);
            Assert.IsNull(actual.IdNumber);
            Assert.IsFalse(actual.Deletion);
        }

        #endregion

        #region AssistantDeletionUpdated

        /// <summary>
        /// Checks whether the AssistantDeletionUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantDeletionUpdated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            DeletionService service = new DeletionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantDeletionUpdated("TestAssistant", "A001", true);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantDeletionUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantDeletionUpdatedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            DeletionService service = new DeletionService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantDeletionUpdated("TestAssistant", "A001", true);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
