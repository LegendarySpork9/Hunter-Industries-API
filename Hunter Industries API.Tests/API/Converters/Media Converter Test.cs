// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class MediaConverterTest
    {
        #region GetSQLGetApplicationEntity

        /// <summary>
        /// Tests whether the GetSQLGetApplicationEntity method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetApplicationEntity()
        {
            string expected = "NoApplicationEntity";
            string actual = MediaConverter.GetSQLGetApplicationEntity("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetSQLCreateApplicationEntityLink

        /// <summary>
        /// Tests whether the GetSQLCreateApplicationEntityLink method returns "Unknown" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateApplicationEntityLink()
        {
            string expected = "NoApplicationEntityLink";
            string actual = MediaConverter.GetSQLCreateApplicationEntityLink("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion
    }
}
