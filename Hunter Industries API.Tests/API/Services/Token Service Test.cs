// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPI.Abstractions;
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
    public class TokenServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();

        [TestInitialize]
        public void Setup()
        {
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQL");
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("SELECT 1");
        }

        #region ApplicationName

        /// <summary>
        /// Checks whether the ApplicationName method returns the application name from the database.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationName()
        {
            string expected = "TestApplication";

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    expected,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            string actual = await service.ApplicationName();

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Checks whether the ApplicationName method returns an empty string when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationNameEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            string actual = await service.ApplicationName();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetUsers

        /// <summary>
        /// Checks whether the GetUsers method returns the correct usernames and passwords.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsers()
        {
            List<(string, string)> users =
            [
                ("admin", "password1"),
                ("user", "password2")
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    users,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            (string[] usernames, string[] passwords) = await service.GetUsers();

            Assert.AreEqual(
                2,
                usernames.Length);
            Assert.AreEqual(
                2,
                passwords.Length);
            Assert.AreEqual(
                "admin",
                usernames[0]);
            Assert.AreEqual(
                "password1",
                passwords[0]);
            Assert.AreEqual(
                "user",
                usernames[1]);
            Assert.AreEqual(
                "password2",
                passwords[1]);
        }

        /// <summary>
        /// Checks whether the GetUsers method returns empty arrays when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersEmpty()
        {
            List<(string, string)> users = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    users,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            (string[] usernames, string[] passwords) = await service.GetUsers();

            Assert.AreEqual(
                0,
                usernames.Length);
            Assert.AreEqual(
                0,
                passwords.Length);
        }

        #endregion

        #region GetAuthorisationPhrases

        /// <summary>
        /// Checks whether the GetAuthorisationPhrases method returns the correct phrases.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuthorisationPhrases()
        {
            List<string> phrases = ["phrase1", "phrase2"];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    phrases,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            string[] actual = await service.GetAuthorisationPhrases();

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "phrase1",
                actual[0]);
            Assert.AreEqual(
                "phrase2",
                actual[1]);
        }

        /// <summary>
        /// Checks whether the GetAuthorisationPhrases method returns an empty array when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuthorisationPhrasesEmpty()
        {
            List<string> phrases = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, string>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    phrases,
                    null));

            TokenService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object,
                "testphrase");

            string[] actual = await service.GetAuthorisationPhrases();

            Assert.AreEqual(
                0,
                actual.Length);
        }

        #endregion
    }
}
