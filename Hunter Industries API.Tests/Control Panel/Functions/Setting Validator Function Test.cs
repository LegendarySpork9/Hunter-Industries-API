// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.ControlPanel.Functions
{
    [TestClass]
    public class SettingValidatorFunctionTest
    {
        #region ValidateApplicationSettings

        /// <summary>
        /// Tests whether the ValidateApplicationSettings method returns an empty list for valid settings.
        /// </summary>
        [TestMethod]
        public void TestValidateApplicationSettingsValid()
        {
            ApplicationModel application = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings =
                [
                    new ApplicationSettingModel { Id = 1, Name = "Theme", Type = "String", Required = true, IsDeleted = false }
                ],
                IsDeleted = false
            };

            List<UserSettingRequestModel> pendingSettings =
            [
                new UserSettingRequestModel { UserId = 1, Application = "TestApp", SettingName = "Theme", SettingValue = "Dark" }
            ];

            List<string> errors = SettingValidatorFunction.ValidateApplicationSettings(
                application,
                null,
                pendingSettings);

            Assert.AreEqual(
                0,
                errors.Count);
        }

        /// <summary>
        /// Tests whether the ValidateApplicationSettings method returns an error for a missing required setting.
        /// </summary>
        [TestMethod]
        public void TestValidateApplicationSettingsMissingRequired()
        {
            ApplicationModel application = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings =
                [
                    new ApplicationSettingModel { Id = 1, Name = "Theme", Type = "String", Required = true, IsDeleted = false }
                ],
                IsDeleted = false
            };

            List<string> errors = SettingValidatorFunction.ValidateApplicationSettings(
                application,
                null,
                []);

            Assert.AreEqual(
                1,
                errors.Count);
            Assert.AreEqual(
                "Theme is required.",
                errors[0]);
        }

        /// <summary>
        /// Tests whether the ValidateApplicationSettings method returns an error for an invalid type.
        /// </summary>
        [TestMethod]
        public void TestValidateApplicationSettingsInvalidType()
        {
            ApplicationModel application = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings =
                [
                    new ApplicationSettingModel { Id = 1, Name = "Count", Type = "Int32", Required = false, IsDeleted = false }
                ],
                IsDeleted = false
            };

            List<UserSettingRequestModel> pendingSettings =
            [
                new UserSettingRequestModel { UserId = 1, Application = "TestApp", SettingName = "Count", SettingValue = "NotANumber" }
            ];

            List<string> errors = SettingValidatorFunction.ValidateApplicationSettings(
                application,
                null,
                pendingSettings);

            Assert.AreEqual(
                1,
                errors.Count);
            Assert.AreEqual(
                "Count must be a valid Int32.",
                errors[0]);
        }

        /// <summary>
        /// Tests whether the ValidateApplicationSettings method uses existing user settings as fallback.
        /// </summary>
        [TestMethod]
        public void TestValidateApplicationSettingsExistingSettingsFallback()
        {
            ApplicationModel application = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings =
                [
                    new ApplicationSettingModel { Id = 1, Name = "Theme", Type = "String", Required = true, IsDeleted = false }
                ],
                IsDeleted = false
            };

            UserSettingModel currentSettings = new()
            {
                Application = "TestApp",
                Settings =
                [
                    new SettingModel { Id = 1, Name = "Theme", Value = "Dark" }
                ]
            };

            List<string> errors = SettingValidatorFunction.ValidateApplicationSettings(
                application,
                currentSettings,
                []);

            Assert.AreEqual(
                0,
                errors.Count);
        }

        #endregion

        #region TryConvert

        /// <summary>
        /// Tests whether the TryConvert method returns true for a valid Boolean conversion.
        /// </summary>
        [TestMethod]
        public void TestTryConvertBoolean()
        {
            bool actual = SettingValidatorFunction.TryConvert(
                "Boolean",
                "True");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the TryConvert method returns true for a valid Int32 conversion.
        /// </summary>
        [TestMethod]
        public void TestTryConvertInt32()
        {
            bool actual = SettingValidatorFunction.TryConvert(
                "Int32",
                "42");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the TryConvert method returns false for an invalid Int32 conversion.
        /// </summary>
        [TestMethod]
        public void TestTryConvertInt32Invalid()
        {
            bool actual = SettingValidatorFunction.TryConvert(
                "Int32",
                "NotANumber");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the TryConvert method returns false for an unknown type.
        /// </summary>
        [TestMethod]
        public void TestTryConvertUnknownType()
        {
            bool actual = SettingValidatorFunction.TryConvert(
                "Trombone",
                "42");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
