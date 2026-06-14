// Copyright © - 14/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class DomainMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the domain model correctly.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            DomainModel domain = new()
            {
                Id = 1,
                Host = "example.com",
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = domain.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "example.com",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
