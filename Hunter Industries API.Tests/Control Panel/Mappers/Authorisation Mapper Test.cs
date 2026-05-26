// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class AuthorisationMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the Phrase property to Name.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            AuthorisationModel authorisation = new()
            {
                Id = 1,
                Phrase = "TestPhrase",
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = authorisation.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "TestPhrase",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
