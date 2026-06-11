// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class DowntimeMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the Name as "Time (Duration)".
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            DowntimeModel downtime = new()
            {
                Id = 1,
                Time = "02:00",
                Duration = 30,
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = downtime.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "02:00 (30)",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
