// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Converters
{
    [TestClass]
    public class APIConverterTest
    {
        #region GetQuery

        /// <summary>
        /// Tests whether the GetQuery method returns an empty string when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetQuery()
        {
            string expected = string.Empty;
            string actual = APIConverter.GetQuery("/trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetQuery method returns "?includeDeleted=true" when given "/user".
        /// </summary>
        [TestMethod]
        public void TestGetQueryUser()
        {
            string expected = "?includeDeleted=true";
            string actual = APIConverter.GetQuery("/user");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetQuery method returns "?includeUsed=false" when given "/configuration/authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetQueryAuthorisation()
        {
            string expected = "?includeUsed=false";
            string actual = APIConverter.GetQuery("/configuration/authorisation");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion
    }
}
