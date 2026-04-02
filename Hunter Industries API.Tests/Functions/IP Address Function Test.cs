// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using System.Web;

namespace HunterIndustriesAPI.Tests.Functions
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
            HttpRequestBase request = CreateMockRequest(cfConnectingIp: expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method returns the X-Forwarded-For header when CF-Connecting-IP is absent.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressXForwardedFor()
        {
            string expected = "198.51.100.1";
            HttpRequestBase request = CreateMockRequest(xForwardedFor: expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method returns the UserHostAddress when both headers are absent.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressUserHostAddress()
        {
            string expected = "127.0.0.1";
            HttpRequestBase request = CreateMockRequest(userHostAddress: expected);

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method prioritises CF-Connecting-IP over X-Forwarded-For.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressCFConnectingIPPriority()
        {
            string expected = "203.0.113.1";
            HttpRequestBase request = CreateMockRequest(cfConnectingIp: expected, xForwardedFor: "198.51.100.1");

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FetchIpAddress method prioritises X-Forwarded-For over UserHostAddress.
        /// </summary>
        [TestMethod]
        public void TestFetchIpAddressXForwardedForPriority()
        {
            string expected = "198.51.100.1";
            HttpRequestBase request = CreateMockRequest(xForwardedFor: expected, userHostAddress: "127.0.0.1");

            string actual = IPAddressFunction.FetchIpAddress(request);

            Assert.AreNotEqual("127.0.0.1", actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Creates a mock HttpRequestBase with the specified header values for testing.
        /// </summary>
        private HttpRequestBase CreateMockRequest(string cfConnectingIp = null, string xForwardedFor = null, string userHostAddress = null)
        {
            NameValueCollection headers = new NameValueCollection();

            if (cfConnectingIp != null)
            {
                headers.Add("CF-Connecting-IP", cfConnectingIp);
            }

            if (xForwardedFor != null)
            {
                headers.Add("X-Forwarded-For", xForwardedFor);
            }

            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(r => r.Headers).Returns(headers);
            mockRequest.Setup(r => r.UserHostAddress).Returns(userHostAddress);

            return mockRequest.Object;
        }
    }
}
