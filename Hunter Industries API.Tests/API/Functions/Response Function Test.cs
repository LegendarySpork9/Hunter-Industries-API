// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.API.Functions
{
    [TestClass]
    public class ResponseFunctionTest
    {
        /// <summary>
        /// Tests whether the GetModelJSON method returns the correct JSON when given an anonymous object.
        /// </summary>
        [TestMethod]
        public void TestGetModelJSON()
        {
            string expected = "{\"Name\":\"Test\",\"Value\":1}";
            string actual = ResponseFunction.GetModelJSON(new { Name = "Test", Value = 1 });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetModelJSON method returns "null" when given null.
        /// </summary>
        [TestMethod]
        public void TestGetModelJSONNull()
        {
            string expected = "null";
            string actual = ResponseFunction.GetModelJSON(null);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetModelJSON method returns an empty JSON object when given an empty object.
        /// </summary>
        [TestMethod]
        public void TestGetModelJSONEmpty()
        {
            string expected = "{}";
            string actual = ResponseFunction.GetModelJSON(new { });

            Assert.AreEqual(expected, actual);
        }
    }
}
