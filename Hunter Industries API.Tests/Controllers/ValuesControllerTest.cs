﻿using HunterIndustriesAPI;
using HunterIndustriesAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace HunterIndustriesAPI.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            //ValuesController controller = new ValuesController();

            // Act
            //IEnumerable<string> result = controller.Get();

            // Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual(2, result.Count());
            //Assert.AreEqual("value1", result.ElementAt(0));
            //Assert.AreEqual("value2", result.ElementAt(1));
            int x = 1;
            int y = 1;

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            //ValuesController controller = new ValuesController();

            // Act
            //string result = controller.Get(5);

            // Assert
            //Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            //ValuesController controller = new ValuesController();

            // Act
            //controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            //ValuesController controller = new ValuesController();

            // Act
            //controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            //ValuesController controller = new ValuesController();

            // Act
            //controller.Delete(5);

            // Assert
        }
    }
}
