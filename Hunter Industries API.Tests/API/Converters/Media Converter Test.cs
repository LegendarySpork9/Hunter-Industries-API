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
        /// Tests whether the GetSQLGetApplicationEntity method returns "NoApplicationEntity" when given any value.
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

        /// <summary>
        /// Tests whether the GetSQLGetApplicationEntity method returns the link sql when given "Portfolio".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetApplicationEntityPortfolio()
        {
            string expected = @"join PortfolioItemImage PII with (nolock) on Media.MediaId = PII.MediaId
where PortfolioItemId = @entityId";
            string actual = MediaConverter.GetSQLGetApplicationEntity("Portfolio");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region HasApplicationEntityLink

        /// <summary>
        /// Tests whether the HasApplicationEntityLink method returns false when given any value.
        /// </summary>
        [TestMethod]
        public void TestHasApplicationEntityLink()
        {
            bool actual = MediaConverter.HasApplicationEntityLink("Trombone");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the HasApplicationEntityLink method returns true when given "Portfolio".
        /// </summary>
        [TestMethod]
        public void TestHasApplicationEntityLinkPortfolio()
        {
            bool actual = MediaConverter.HasApplicationEntityLink("Portfolio");

            Assert.IsTrue(actual);
        }

        #endregion

        #region GetSQLCreateApplicationEntityLink

        /// <summary>
        /// Tests whether the GetSQLCreateApplicationEntityLink method returns "NoApplicationEntityLink" when given any value.
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

        /// <summary>
        /// Tests whether the GetSQLCreateApplicationEntityLink method returns "CreatePortfolioItemImage.sql" when given "Portfolio".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateApplicationEntityLinkPortfolio()
        {
            string expected = "CreatePortfolioItemImage.sql";
            string actual = MediaConverter.GetSQLCreateApplicationEntityLink("Portfolio");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion
    }
}
