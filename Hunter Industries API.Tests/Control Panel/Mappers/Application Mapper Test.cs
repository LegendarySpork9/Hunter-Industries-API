// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class ApplicationMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the application model correctly.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            ApplicationModel application = new ApplicationModel
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "Phrase", IsDeleted = false },
                Settings = new List<ApplicationSettingModel>(),
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = application.ToListObject();

            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual("TestApp", actual.Name);
            Assert.AreEqual(false, actual.IsDeleted);
        }
    }
}
