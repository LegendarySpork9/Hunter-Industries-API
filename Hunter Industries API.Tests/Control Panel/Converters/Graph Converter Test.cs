// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Converters
{
    [TestClass]
    public class GraphConverterTest
    {
        #region GetMethodBadgeClass

        /// <summary>
        /// Tests whether the GetMethodBadgeClass method returns "bg-secondary" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetMethodBadgeClass()
        {
            string expected = "bg-secondary";
            string actual = GraphConverter.GetMethodBadgeClass("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodBadgeClass method returns "badge-method-get" when given "GET".
        /// </summary>
        [TestMethod]
        public void TestGetMethodBadgeClassGet()
        {
            string expected = "badge-method-get";
            string actual = GraphConverter.GetMethodBadgeClass("GET");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodBadgeClass method returns "badge-method-post" when given "POST".
        /// </summary>
        [TestMethod]
        public void TestGetMethodBadgeClassPost()
        {
            string expected = "badge-method-post";
            string actual = GraphConverter.GetMethodBadgeClass("POST");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodBadgeClass method returns "badge-method-patch" when given "PATCH".
        /// </summary>
        [TestMethod]
        public void TestGetMethodBadgeClassPatch()
        {
            string expected = "badge-method-patch";
            string actual = GraphConverter.GetMethodBadgeClass("PATCH");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetMethodBadgeClass method returns "badge-method-delete" when given "DELETE".
        /// </summary>
        [TestMethod]
        public void TestGetMethodBadgeClassDelete()
        {
            string expected = "badge-method-delete";
            string actual = GraphConverter.GetMethodBadgeClass("DELETE");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetStatusBadgeClass

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "bg-secondary" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass()
        {
            string expected = "bg-secondary";
            string actual = GraphConverter.GetStatusBadgeClass("999");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-200" when given "200".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass200()
        {
            string expected = "badge-status-200";
            string actual = GraphConverter.GetStatusBadgeClass("200 OK");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-201" when given "201".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass201()
        {
            string expected = "badge-status-201";
            string actual = GraphConverter.GetStatusBadgeClass("201 Created");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-400" when given "400".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass400()
        {
            string expected = "badge-status-400";
            string actual = GraphConverter.GetStatusBadgeClass("400 Bad Request");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-401" when given "401".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass401()
        {
            string expected = "badge-status-401";
            string actual = GraphConverter.GetStatusBadgeClass("401 Unauthorized");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-403" when given "403".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass403()
        {
            string expected = "badge-status-403";
            string actual = GraphConverter.GetStatusBadgeClass("403 Forbidden");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-404" when given "404".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass404()
        {
            string expected = "badge-status-404";
            string actual = GraphConverter.GetStatusBadgeClass("404 Not Found");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetStatusBadgeClass method returns "badge-status-500" when given "500".
        /// </summary>
        [TestMethod]
        public void TestGetStatusBadgeClass500()
        {
            string expected = "badge-status-500";
            string actual = GraphConverter.GetStatusBadgeClass("500 Internal Server Error");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion
    }
}
