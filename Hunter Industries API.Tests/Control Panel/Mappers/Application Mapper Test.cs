// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            ApplicationModel application = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "Phrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = application.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "TestApp",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
