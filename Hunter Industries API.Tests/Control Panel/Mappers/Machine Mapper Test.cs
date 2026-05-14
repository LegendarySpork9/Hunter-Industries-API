// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class MachineMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the HostName to Name.
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            MachineModel machine = new MachineModel
            {
                Id = 1,
                HostName = "SERVER-01",
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = machine.ToListObject();

            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual("SERVER-01", actual.Name);
            Assert.AreEqual(false, actual.IsDeleted);
        }
    }
}
