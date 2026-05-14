// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Mappers;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Mappers
{
    [TestClass]
    public class ConnectionMapperTest
    {
        /// <summary>
        /// Tests whether the ToListObject method maps the Name as "IPAddress:Port".
        /// </summary>
        [TestMethod]
        public void TestToListObject()
        {
            ConnectionModel connection = new ConnectionModel
            {
                Id = 1,
                IPAddress = "192.168.1.1",
                Port = 8080,
                IsDeleted = false
            };

            ConfigurationListObjectModel actual = connection.ToListObject();

            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual("192.168.1.1:8080", actual.Name);
            Assert.AreEqual(false, actual.IsDeleted);
        }
    }
}
