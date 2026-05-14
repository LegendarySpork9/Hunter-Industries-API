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
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly APISettingsModel _apiSettings = new APISettingsModel { AuthPayloadLocation = "C:\\auth.json" };

        #region GetControlPanelApplication

        /// <summary>
        /// Tests whether the GetControlPanelApplication method returns the matching application name.
        /// </summary>
        [TestMethod]
        public void TestGetControlPanelApplication()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            List<ApplicationModel> applications = new List<ApplicationModel>
            {
                new ApplicationModel
                {
                    Id = 1,
                    Name = "Control Panel",
                    Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                    Settings = new List<ApplicationSettingModel>(),
                    IsDeleted = false
                }
            };

            string actual = function.GetControlPanelApplication(applications);

            Assert.AreEqual("Control Panel", actual);
        }

        /// <summary>
        /// Tests whether the GetControlPanelApplication method returns "Application Not Found" when no match exists.
        /// </summary>
        [TestMethod]
        public void TestGetControlPanelApplicationNotFound()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            List<ApplicationModel> applications = new List<ApplicationModel>
            {
                new ApplicationModel
                {
                    Id = 1,
                    Name = "Control Panel",
                    Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                    Settings = new List<ApplicationSettingModel>(),
                    IsDeleted = false
                }
            };

            string actual = function.GetControlPanelApplication(applications);

            Assert.AreEqual("Application Not Found", actual);
        }

        #endregion

        #region IsControlPanelApplication

        /// <summary>
        /// Tests whether the IsControlPanelApplication method returns true for a matching application.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelApplication()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            ApplicationModel application = new ApplicationModel
            {
                Id = 1,
                Name = "Control Panel",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = new List<ApplicationSettingModel>(),
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
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            ApplicationModel application = new ApplicationModel
            {
                Id = 1,
                Name = "Control Panel",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = new List<ApplicationSettingModel>(),
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
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"TestPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            bool actual = function.IsControlPanelAuthorisation("TestPhrase");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the IsControlPanelAuthorisation method returns false for a non-matching phrase.
        /// </summary>
        [TestMethod]
        public void TestIsControlPanelAuthorisationFalse()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("{\"phrase\":\"OtherPhrase\"}");

            ConfigurationFunction function = new ConfigurationFunction(_mockFileSystem.Object, _apiSettings);

            bool actual = function.IsControlPanelAuthorisation("TestPhrase");

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
