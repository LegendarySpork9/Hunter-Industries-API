// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Objects.User;
using HunterIndustriesAPI.Services.User;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.User
{
    [TestClass]
    public class UserSettingsServiceTest
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

        #region GetUserSettings

        /// <summary>
        /// Checks whether the GetUserSettings method returns grouped settings.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettings()
        {
            SettingRecord settingOne = new()
            {
                Id = 1,
                Name = "Theme",
                Value = "Dark"
            };
            SettingRecord settingTwo = new()
            {
                Id = 2,
                Name = "Language",
                Value = "English"
            };

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, SettingRecord)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [("TestApp", settingOne), ("TestApp", settingTwo)],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<UserSettingRecord> actual = await service.GetUserSettings(
                1,
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, int, string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<UserSettingRecord> actual = await service.GetUserSettings(
                1,
                null);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetUserSetting

        /// <summary>
        /// Checks whether the GetUserSetting method returns a populated setting.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSetting()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, SettingRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new SettingRecord { Id = 1, Name = "Theme", Value = "Dark" },
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            SettingRecord actual = await service.GetUserSetting(1);

            Assert.AreEqual(
                1,
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, SettingRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            SettingRecord actual = await service.GetUserSetting(1);

            Assert.AreEqual(
                0,
                actual.Id);
        }

        #endregion

        #region UserSettingExists (string, string, string)

        /// <summary>
        /// Checks whether the UserSettingExists method returns true when a setting exists.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExists()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingExists(
                0,
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingExists(
                0,
                "TestApp",
                "Theme");

            Assert.IsFalse(actual);
        }

        #endregion

        #region UserSettingExists (int)

        /// <summary>
        /// Checks whether the UserSettingExists method returns true when a setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExistsId()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingExists method returns false when no setting exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingExistsIdNot()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingExists(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region UserSettingAdded

        /// <summary>
        /// Checks whether the UserSettingAdded method returns true when the setting is added.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingAdded()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool actual, int id) = await service.UserSettingAdded(new UserSettingsModel
            {
                UserId = 0,
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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    (Exception)null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (bool actual, int id) = await service.UserSettingAdded(new UserSettingsModel
            {
                UserId = 0,
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            });

            Assert.IsFalse(actual);
        }

        #endregion

        #region UserSettingUpdated

        /// <summary>
        /// Checks whether the UserSettingUpdated method returns true when the setting is updated.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingUpdated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingUpdated(
                1,
                "Light");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the UserSettingUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestUserSettingUpdatedFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            UserSettingsService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.UserSettingUpdated(
                1,
                "Light");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
