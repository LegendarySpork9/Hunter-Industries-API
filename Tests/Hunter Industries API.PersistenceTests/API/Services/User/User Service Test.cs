// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Objects.User;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.User;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class UserServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly IFileSystem _FileSystem = new FileSystemWrapper();

        /// <summary>
        /// Creates the test database and schema.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            (_ConnectionString, _DatabaseName) = LocalDbTestHelper.CreateDatabase();
            LocalDbTestHelper.CreateSchema(_ConnectionString);
            _SqlFilesPath = LocalDbTestHelper.GetSqlFilesPath();
        }

        /// <summary>
        /// Drops the test database.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            LocalDbTestHelper.DropDatabase(_DatabaseName);
        }

        /// <summary>
        /// Clears the test data between tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            LocalDbTestHelper.ClearData(_ConnectionString);
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private UserService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            UserService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts a user record into the database.
        /// </summary>
        private void InsertUser(
            string username,
            string password,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES (@username, @password, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@password",
                        password);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a user record and returns the generated user ID.
        /// </summary>
        private int InsertUserAndGetId(
            string username,
            string password,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) OUTPUT inserted.UserId VALUES (@username, @password, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@password",
                        password);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted ? 1 : 0);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Inserts a user scope record into the database.
        /// </summary>
        private void InsertUserScope(int userId, int scopeId)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO UserScope (UserId, ScopeId) VALUES (@userId, @scopeId)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        userId);
                    cmd.Parameters.AddWithValue(
                        "@scopeId",
                        scopeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetUsers method returns a list with one user.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsers()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);
            InsertUserScope(
                userId,
                2);

            UserService service = CreateService();

            (List<UserRecord> actual, int totalRecords) = await service.GetUsers(null);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                userId,
                actual[0].Id);
            Assert.AreEqual(
                "TestUser",
                actual[0].Username);
            Assert.AreEqual(
                "HashedPassword",
                actual[0].Password);
            Assert.AreEqual(
                2,
                actual[0].Scopes.Count);
            Assert.IsFalse(actual[0].IsDeleted);
            Assert.AreEqual(
                1,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetUsers method returns an empty list when no users are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersEmpty()
        {
            UserService service = CreateService();

            (List<UserRecord> actual, int totalRecords) = await service.GetUsers(null);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetUser method returns a single user.
        /// </summary>
        [TestMethod]
        public async Task TestGetUser()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);
            InsertUserScope(
                userId,
                2);

            UserService service = CreateService();

            UserRecord actual = await service.GetUser(userId);

            Assert.AreEqual(
                userId,
                actual.Id);
            Assert.AreEqual(
                "TestUser",
                actual.Username);
            Assert.AreEqual(
                "HashedPassword",
                actual.Password);
            Assert.AreEqual(
                2,
                actual.Scopes.Count);
            Assert.IsFalse(actual.IsDeleted);
        }

        /// <summary>
        /// Checks whether the GetUser method returns a default user when no user is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserEmpty()
        {
            UserService service = CreateService();

            UserRecord actual = await service.GetUser(999);

            Assert.AreEqual(
                0,
                actual.Id);
            Assert.IsNull(actual.Username);
        }

        /// <summary>
        /// Checks whether the UserExists method returns true when a user exists with the given username.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsUsername()
        {
            InsertUser(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            bool actual = await service.UserExists("TestUser");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserExists method returns false when no user exists with the given username.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsUsernameNot()
        {
            UserService service = CreateService();

            bool actual = await service.UserExists("TestUser");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the UserExists method returns true when a user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsId()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            bool actual = await service.UserExists(userId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserExists method returns false when no user exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserExistsIdNot()
        {
            UserService service = CreateService();

            bool actual = await service.UserExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the UserCreated method returns true and the user id when a user is created.
        /// </summary>
        [TestMethod]
        public async Task TestUserCreated()
        {
            UserService service = CreateService();

            (bool created, int userId) = await service.UserCreated(
                "TestUser",
                "Password");

            Assert.IsTrue(created);
            Assert.IsTrue(userId > 0);
        }

        /// <summary>
        /// Checks whether the UserCreated method returns false and zero when the creation fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserCreatedFailed()
        {
            UserService service = CreateService();

            (bool created, int userId) = await service.UserCreated(
                new string('A', 256),
                "Password");

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                userId);
        }

        /// <summary>
        /// Checks whether the UserScopeCreated method returns true when all scopes are created.
        /// </summary>
        [TestMethod]
        public async Task TestUserScopeCreated()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            bool actual = await service.UserScopeCreated(
                userId,
                new List<string> { "User", "Assistant API" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserScopeCreated method returns false when a scope creation fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserScopeCreatedFailed()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            bool actual = await service.UserScopeCreated(
                userId,
                new List<string> { "NonExistentScope" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the GetUserScopes method returns a list of scopes.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserScopes()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");
            InsertUserScope(
                userId,
                1);
            InsertUserScope(
                userId,
                2);

            UserService service = CreateService();

            List<string> actual = await service.GetUserScopes(userId);

            Assert.AreEqual(
                2,
                actual.Count);
            Assert.AreEqual(
                "User",
                actual[0]);
            Assert.AreEqual(
                "Assistant API",
                actual[1]);
        }

        /// <summary>
        /// Checks whether the GetUserScopes method returns an empty list when no scopes are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserScopesEmpty()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            List<string> actual = await service.GetUserScopes(userId);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the UserDeleted method returns true when the user is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestUserDeleted()
        {
            int userId = InsertUserAndGetId(
                "TestUser",
                "HashedPassword");

            UserService service = CreateService();

            bool actual = await service.UserDeleted(userId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserDeleted method returns false when the deletion fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserDeletedFailed()
        {
            UserService service = CreateService();

            bool actual = await service.UserDeleted(999);

            Assert.IsFalse(actual);
        }

    }
}
