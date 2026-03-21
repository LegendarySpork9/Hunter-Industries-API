// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects.Assistant;
using HunterIndustriesAPI.Services.Assistant;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.Services.Assistant
{
    [TestClass]
    public class ConfigServiceTest
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

        #region GetAssistantConfig

        /// <summary>
        /// Checks whether the GetAssistantConfig method returns the assistant configurations, total count, and version.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantConfig()
        {
            List<AssistantConfiguration> configs = new List<AssistantConfiguration>
            {
                new AssistantConfiguration
                {
                    AssistantName = "TestAssistant",
                    IdNumber = "A001",
                    AssignedUser = "TestUser",
                    HostName = "TestHost",
                    Deletion = false,
                    Version = "1.0.0"
                }
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AssistantConfiguration>>(), It.IsAny<SqlParameter[]>()).Result).Returns((configs, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns(("2.0.0", null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            (List<AssistantConfiguration> results, int totalConfigs, string mostRecentVersion) = await service.GetAssistantConfig("TestAssistant", "A001");

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("TestAssistant", results[0].AssistantName);
            Assert.AreEqual(1, totalConfigs);
            Assert.AreEqual("2.0.0", mostRecentVersion);
        }

        /// <summary>
        /// Checks whether the GetAssistantConfig method returns an empty list, zero, and empty string when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetAssistantConfigEmpty()
        {
            List<AssistantConfiguration> configs = new List<AssistantConfiguration>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AssistantConfiguration>>(), It.IsAny<SqlParameter[]>()).Result).Returns((configs, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            (List<AssistantConfiguration> results, int totalConfigs, string mostRecentVersion) = await service.GetAssistantConfig(null, null);

            Assert.AreEqual(0, results.Count);
            Assert.AreEqual(0, totalConfigs);
            Assert.AreEqual(string.Empty, mostRecentVersion);
        }

        #endregion

        #region GetMostRecentVersion

        /// <summary>
        /// Checks whether the GetMostRecentVersion method returns the version string.
        /// </summary>
        [TestMethod]
        public async Task TestGetMostRecentVersion()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns(("3.1.0", null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            string result = await service.GetMostRecentVersion();

            Assert.AreEqual("3.1.0", result);
        }

        /// <summary>
        /// Checks whether the GetMostRecentVersion method returns an empty string when the result is null.
        /// </summary>
        [TestMethod]
        public async Task TestGetMostRecentVersionEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            string result = await service.GetMostRecentVersion();

            Assert.AreEqual(string.Empty, result);
        }

        #endregion

        #region AssistantExists

        /// <summary>
        /// Checks whether the AssistantExists method returns true when a matching assistant is found.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantExists()
        {
            List<(string, string)> results = new List<(string, string)>
            {
                ("TestAssistant", "A001")
            };

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantExists("TestAssistant", "A001");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantExists method returns false when no matching assistant is found.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantExistsNot()
        {
            List<(string, string)> results = new List<(string, string)>();

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((results, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantExists("TestAssistant", "A001");

            Assert.IsFalse(actual);
        }

        #endregion

        #region AssistantConfigCreated

        /// <summary>
        /// Checks whether the AssistantConfigCreated method returns true when all database calls succeed.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantConfigCreated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.SetupSequence(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null)).Returns((2, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantConfigCreated("TestAssistant", "A001", "TestUser", "TestHost");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the AssistantConfigCreated method returns false when the first ExecuteScalar returns null.
        /// </summary>
        [TestMethod]
        public async Task TestAssistantConfigCreatedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            ConfigService service = new ConfigService(_mockLogger.Object, _mockFileSystem.Object, _mockOptions.Object, _mockDatabase.Object);
            bool actual = await service.AssistantConfigCreated("TestAssistant", "A001", "TestUser", "TestHost");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
