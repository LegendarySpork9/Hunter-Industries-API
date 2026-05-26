// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.ControlPanel.Functions
{
    [TestClass]
    public class ConfigurationFunctionTest
    {
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly APISettingsModel _ApiSettings = new() { AuthPayloadLocation = "C:\\auth.json" };

        #region GetControlPanelApplication

        /// <summary>
        /// Tests whether the GetControlPanelApplication method returns the matching application name.
        /// </summary>
        [TestMethod]
        public void TestGetControlPanelApplication()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            List<ApplicationModel> applications =
            [
                new ApplicationModel
                {
                    Id = 1,
                    Name = "Control Panel",
                    Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                    Settings = [],
                    IsDeleted = false
                }
            ];

            string actual = function.GetControlPanelApplication(applications);

            Assert.AreEqual(
                "Control Panel",
                actual);
        }

        /// <summary>
        /// Tests whether the GetControlPanelApplication method returns "Application Not Found" when no match exists.
        /// </summary>
        [TestMethod]
        public void TestGetControlPanelApplicationNotFound()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            List<ApplicationModel> applications =
            [
                new ApplicationModel
                {
                    Id = 1,
                    Name = "Control Panel",
                    Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                    Settings = [],
                    IsDeleted = false
                }
            ];

            string actual = function.GetControlPanelApplication(applications);

            Assert.AreEqual(
                "Application Not Found",
                actual);
        }

        #endregion

        #region IsControlPanelApplication

        /// <summary>
        /// Tests whether the IsControlPanelApplication method returns true for a matching application.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelApplication()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            ApplicationModel application = new()
            {
                Id = 1,
                Name = "Control Panel",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            bool actual = function.IsControlPanelApplication(application);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the IsControlPanelApplication method returns false for a non-matching application.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelApplicationFalse()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            ApplicationModel application = new()
            {
                Id = 1,
                Name = "Control Panel",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            bool actual = function.IsControlPanelApplication(application);

            Assert.IsFalse(actual);
        }

        #endregion

        #region IsControlPanelAuthorisation

        /// <summary>
        /// Tests whether the IsControlPanelAuthorisation method returns true for a matching phrase.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelAuthorisation()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            bool actual = function.IsControlPanelAuthorisation("TestPhrase");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the IsControlPanelAuthorisation method returns false for a non-matching phrase.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelAuthorisationFalse()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new(
                _MockFileSystem.Object,
                _ApiSettings);

            bool actual = function.IsControlPanelAuthorisation("TestPhrase");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
