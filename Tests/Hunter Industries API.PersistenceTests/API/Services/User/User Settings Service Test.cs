// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
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
    public class UserSettingsServiceTest
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
        private UserSettingsService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            UserSettingsService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts prerequisite test data for user settings testing.
        /// </summary>
        private void InsertTestData(
            string phrase,
            string applicationName,
            string username,
            string password)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES (@phrase, 0); " +
                    "DECLARE @phraseId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @appName, 0); " +
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES (@username, @password, 0);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phrase",
                        phrase);
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);
                    cmd.Parameters.AddWithValue(
                        "@password",
                        password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a user setting record and returns the generated ID.
        /// </summary>
        private int InsertUserSetting(
            int userId,
            string applicationName,
            string settingName,
            string settingValue)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO UserSetting (UserId, ApplicationId, [Name], [Value]) " +
                    "SELECT @userId, ApplicationId, @name, @value " +
                    "FROM [Application] WHERE [Name] = @appName; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        userId);
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        settingName);
                    cmd.Parameters.AddWithValue(
                        "@value",
                        settingValue);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Retrieves the user ID for the given username.
        /// </summary>
        private int GetUserId(string username)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT UserId FROM APIUser WHERE Username = @username",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@username",
                        username);

                    return (int)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetUserSettings method returns grouped settings.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettings()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");
            InsertUserSetting(
                userId,
                "TestApp",
                "Language",
                "English");

            UserSettingsService service = CreateService();

            List<UserSettingRecord> actual = await service.GetUserSettings(
                userId,
                null);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "TestApp",
                actual[0].Application);
            Assert.AreEqual(
                2,
                actual[0].Settings.Count);
        }

        /// <summary>
        /// Checks whether the GetUserSettings method returns an empty list when no settings are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettingsEmpty()
        {
            UserSettingsService service = CreateService();

            List<UserSettingRecord> actual = await service.GetUserSettings(
                1,
                null);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetUserSetting method returns a populated setting.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSetting()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            int settingId = InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsService service = CreateService();

            SettingRecord actual = await service.GetUserSetting(settingId);

            Assert.AreEqual(
                settingId,
                actual.Id);
            Assert.AreEqual(
                "Theme",
                actual.Name);
            Assert.AreEqual(
                "Dark",
                actual.Value);
        }

        /// <summary>
        /// Checks whether the GetUserSetting method returns an empty setting when no setting is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettingEmpty()
        {
            UserSettingsService service = CreateService();

            SettingRecord actual = await service.GetUserSetting(999);

            Assert.AreEqual(
                0,
                actual.Id);
        }

        /// <summary>
        /// Checks whether the UserSettingExists method returns true when a setting exists.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExists()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingExists(
                userId,
                "TestApp",
                "Theme");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingExists method returns false when no setting exists.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExistsNot()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");

            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingExists(
                userId,
                "TestApp",
                "Theme");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingExists method returns true when a setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExistsId()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            int settingId = InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingExists(settingId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingExists method returns false when no setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExistsIdNot()
        {
            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingAdded method returns true when the setting is added.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingAdded()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");

            UserSettingsService service = CreateService();

            (bool actual, int id) = await service.UserSettingAdded(new UserSettingsModel
            {
                UserId = userId,
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingAdded method returns false when the addition fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingAddedFailed()
        {
            UserSettingsService service = CreateService();

            (bool actual, int id) = await service.UserSettingAdded(new UserSettingsModel
            {
                UserId = 999,
                Application = "NonExistentApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingUpdated method returns true when the setting is updated.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingUpdated()
        {
            InsertTestData(
                "phrase1",
                "TestApp",
                "admin",
                "password1");
            int userId = GetUserId("admin");
            int settingId = InsertUserSetting(
                userId,
                "TestApp",
                "Theme",
                "Dark");

            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingUpdated(
                settingId,
                "Light");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingUpdatedFailed()
        {
            UserSettingsService service = CreateService();

            bool actual = await service.UserSettingUpdated(
                999,
                "Light");

            Assert.IsFalse(actual);
        }

    }
}
