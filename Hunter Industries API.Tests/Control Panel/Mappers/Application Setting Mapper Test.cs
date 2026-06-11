// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class ApplicationSettingMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the application setting model correctly.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            ApplicationSettingModel setting = new()
            {
                Id = 1,
                Name = "Theme",
                Type = "String",
                Required = true,
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = setting.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "Theme",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
