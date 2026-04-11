// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services
{
    [TestClass]
    public class ChangeServiceTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();

        [TestInitialize]
        public void Setup()
        {
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQL");
            _mockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns("SELECT 1");
        }

        /// <summary>
        /// Checks whether the LogChange method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestLogChange()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ChangeService service = new ChangeService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.LogChange(1, "Field", "OldValue", "NewValue");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LogChange method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestLogChangeFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            ChangeService service = new ChangeService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);

            bool actual = await service.LogChange(1, "Field", "OldValue", "NewValue");

            Assert.IsFalse(actual);
        }
    }
}
