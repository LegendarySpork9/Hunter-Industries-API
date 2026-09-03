// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace HunterIndustriesAPI.UnitTests.API.Functions
{
    [TestClass]
    public class IPAddressFunctionTest
    {
        /// <summary>
        /// Tests whether the FetchIpAddress method returns the CF-Connecting-IP header when present.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressCFConnectingIP()
        {
            string expected = "203.0.113.1";
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("CF-Connecting-IP", expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method returns the X-Forwarded-For header when CF-Connecting-IP is absent.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressXForwardedFor()
        {
            string expected = "198.51.100.1";
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("X-Forwarded-For", expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method returns Unknown when no headers or host address are available.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressFallback()
        {
            HttpRequestMessage request = new HttpRequestMessage();

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(
                "Unknown",
                actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method prioritises CF-Connecting-IP over X-Forwarded-For.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressCFConnectingIPPriority()
        {
            string expected = "203.0.113.1";
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("CF-Connecting-IP", expected);
            request.Headers.Add("X-Forwarded-For", "198.51.100.1");

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method prioritises X-Forwarded-For over the fallback.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressXForwardedForPriority()
        {
            string expected = "198.51.100.1";
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("X-Forwarded-For", expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(
                expected,
                actual);
        }
    }
}
