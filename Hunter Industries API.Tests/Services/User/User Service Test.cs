// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects.User;
using HunterIndustriesAPI.Services.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.Services.User
{
    [TestClass]
    public class UserServiceTest
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

        #region GetUsers

        /// <summary>
        /// Checks whether the GetUsers method returns a list with one user.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsers()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)> { (1, "TestUser", "HashedPassword") }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string> { "User", "Assistant API" }, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<UserRecord> actual = await service.GetUsers(0, null);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(1, actual[0].Id);
            Assert.AreEqual("TestUser", actual[0].Username);
            Assert.AreEqual("HashedPassword", actual[0].Password);
            Assert.AreEqual(2, actual[0].Scopes.Count);
        }

        /// <summary>
        /// Checks whether the GetUsers method returns an empty list when no users are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, (int, string, string)>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<(int, string, string)>(), null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<UserRecord> actual = await service.GetUsers(0, null);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region UserExists (string)

        /// <summary>
        /// Checks whether the UserExists method returns true when a user exists with the given username.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsUsername()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserExists("TestUser");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserExists method returns false when no user exists with the given username.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsUsernameNot()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserExists("TestUser");

            Assert.IsFalse(actual);
        }

        #endregion

        #region UserExists (int)

        /// <summary>
        /// Checks whether the UserExists method returns true when a user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsId()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserExists method returns false when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsIdNot()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserExists(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region UserCreated

        /// <summary>
        /// Checks whether the UserCreated method returns true and the user id when a user is created.
        /// </summary>
        [TestMethod]
        public async Task TestUserCreated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            (bool created, int userId) = await service.UserCreated("TestUser", "Password");

            Assert.IsTrue(created);
            Assert.AreEqual(1, userId);
        }

        /// <summary>
        /// Checks whether the UserCreated method returns false and zero when the creation fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserCreatedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            (bool created, int userId) = await service.UserCreated("TestUser", "Password");

            Assert.IsFalse(created);
            Assert.AreEqual(0, userId);
        }

        #endregion

        #region UserScopeCreated

        /// <summary>
        /// Checks whether the UserScopeCreated method returns true when all scopes are created.
        /// </summary>
        [TestMethod]
        public async Task TestUserScopeCreated()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserScopeCreated(1, new List<string> { "User", "Assistant API" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserScopeCreated method returns false when a scope creation fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserScopeCreatedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((null, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserScopeCreated(1, new List<string> { "User" });

            Assert.IsFalse(actual);
        }

        #endregion

        #region GetUserScopes

        /// <summary>
        /// Checks whether the GetUserScopes method returns a list of scopes.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserScopes()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string> { "User", "Assistant API" }, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<string> actual = await service.GetUserScopes(1);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("User", actual[0]);
            Assert.AreEqual("Assistant API", actual[1]);
        }

        /// <summary>
        /// Checks whether the GetUserScopes method returns an empty list when no scopes are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserScopesEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, string>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<string>(), null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            List<string> actual = await service.GetUserScopes(1);

            Assert.AreEqual(0, actual.Count);
        }

        #endregion

        #region UserDeleted

        /// <summary>
        /// Checks whether the UserDeleted method returns true when the user is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestUserDeleted()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserDeleted(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserDeleted method returns false when the deletion fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserDeletedFailed()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            UserService service = new UserService(_MockLogger.Object, _MockFileSystem.Object, _MockOptions.Object, _mockDatabase.Object);

            bool actual = await service.UserDeleted(1);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
