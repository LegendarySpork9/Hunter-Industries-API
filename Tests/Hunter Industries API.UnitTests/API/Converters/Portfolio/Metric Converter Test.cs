// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.UnitTests.API.Converters.Portfolio
{
    [TestClass]
    public class MetricConverterTest
    {

        /// <summary>
        /// Tests whether the GetUpdateSQL method returns "Unknown.sql" when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetUpdateSQL()
        {
            string expected = "Unknown.sql";
            string actual = MetricConverter.GetUpdateSQL("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetUpdateSQL method returns "SummaryViewsUpdated.sql" when given "summary".
        /// </summary>
        [TestMethod]
        public void TestGetUpdateSQLSummary()
        {
            string expected = "SummaryViewsUpdated.sql";
            string actual = MetricConverter.GetUpdateSQL("summary");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetUpdateSQL method returns "FullDetailViewsUpdated.sql" when given "full".
        /// </summary>
        [TestMethod]
        public void TestGetUpdateSQLFull()
        {
            string expected = "FullDetailViewsUpdated.sql";
            string actual = MetricConverter.GetUpdateSQL("full");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMetricName method returns the input unchanged when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetMetricName()
        {
            string expected = "Trombone";
            string actual = MetricConverter.GetMetricName("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMetricName method returns "Summary Views" when given "summary".
        /// </summary>
        [TestMethod]
        public void TestGetMetricNameSummary()
        {
            string expected = "Summary Views";
            string actual = MetricConverter.GetMetricName("summary");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMetricName method returns "Full Detail Views" when given "full".
        /// </summary>
        [TestMethod]
        public void TestGetMetricNameFull()
        {
            string expected = "Full Detail Views";
            string actual = MetricConverter.GetMetricName("full");

            Assert.AreEqual(
                expected,
                actual);
        }

    }
}
