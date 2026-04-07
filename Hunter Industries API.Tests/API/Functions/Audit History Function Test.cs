// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;

namespace HunterIndustriesAPI.Tests.API.Functions
{
    [TestClass]
    public class AuditHistoryFunctionTest
    {
        #region ExtractVersionFromRequest

        /// <summary>
        /// Tests whether the ExtractVersionFromRequest method returns "1.0" when given a v1.0 URL.
        /// </summary>
        [TestMethod]
        public void TestExtractVersionFromRequestV10()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.hunter-industries.co.uk/api/v1.0/user"));

            string expected = "1.0";
            string actual = AuditHistoryFunction.ExtractVersionFromRequest(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the ExtractVersionFromRequest method returns "1.1" when given a v1.1 URL.
        /// </summary>
        [TestMethod]
        public void TestExtractVersionFromRequestV11()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.hunter-industries.co.uk/api/v1.1/user"));

            string expected = "1.1";
            string actual = AuditHistoryFunction.ExtractVersionFromRequest(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the ExtractVersionFromRequest method returns null when no version is present.
        /// </summary>
        [TestMethod]
        public void TestExtractVersionFromRequestNoVersion()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.hunter-industries.co.uk/api/user"));

            string actual = AuditHistoryFunction.ExtractVersionFromRequest(request);

            Assert.IsNull(actual);
        }

        #endregion
    }
}
