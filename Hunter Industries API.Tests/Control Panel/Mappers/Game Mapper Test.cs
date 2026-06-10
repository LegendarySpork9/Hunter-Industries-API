// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class GameMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the Name as "Name (Version)".
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            GameModel game = new()
            {
                Id = 1,
                Name = "Minecraft",
                Version = "1.20.4",
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = game.ToListObject();

            Assert.AreEqual(
                1,
                actual.Id);
            Assert.AreEqual(
                "Minecraft (1.20.4)",
                actual.Name);
            Assert.IsFalse(
                actual.IsDeleted);
        }
    }
}
