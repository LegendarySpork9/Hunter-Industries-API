// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class ComponentMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the component model correctly.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            ComponentModel component = new ComponentModel
            {
                Id = 1,
                Name = "CPU",
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = component.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "CPU",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
