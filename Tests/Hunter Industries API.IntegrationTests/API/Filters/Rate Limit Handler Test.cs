// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.IntegrationTests.API.Filters
{
    [TestClass]
    public class RateLimitHandlerTest
    {
        /// <summary>
        /// Tests whether a single request is allowed through.
        /// </summary>
        [TestMethod]
        public async Task TestSingleRequestAllowed()
        {
            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            RateLimitHandler handler = new(mockClock.Object);
            handler.InnerHandler = new MockInnerHandler();

            HttpMessageInvoker invoker = new(handler);
            HttpRequestMessage request = new(HttpMethod.Get, "http://localhost/api/auth/token");
            request.Headers.Add("X-Forwarded-For", "192.168.1.100");

            HttpResponseMessage response = await invoker.SendAsync(
                request,
                CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        /// <summary>
        /// Tests whether the handler returns 429 after exceeding the rate limit.
        /// </summary>
        [TestMethod]
        public async Task TestRateLimitExceeded()
        {
            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            RateLimitHandler handler = new(mockClock.Object);
            handler.InnerHandler = new MockInnerHandler();

            HttpMessageInvoker invoker = new(handler);
            string uniqueIp = $"10.0.0.{new Random().Next(1, 255)}";

            for (int i = 0; i < 10; i++)
            {
                HttpRequestMessage request = new(HttpMethod.Get, "http://localhost/api/auth/token");
                request.Headers.Add("X-Forwarded-For", uniqueIp);

                await invoker.SendAsync(
                    request,
                    CancellationToken.None);
            }

            HttpRequestMessage blockedRequest = new(HttpMethod.Get, "http://localhost/api/auth/token");
            blockedRequest.Headers.Add("X-Forwarded-For", uniqueIp);

            HttpResponseMessage response = await invoker.SendAsync(
                blockedRequest,
                CancellationToken.None);

            Assert.AreEqual(
                (HttpStatusCode)429,
                response.StatusCode);
        }

        /// <summary>
        /// Tests whether the handler includes a Retry-After header when rate limited.
        /// </summary>
        [TestMethod]
        public async Task TestRateLimitRetryAfterHeader()
        {
            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            RateLimitHandler handler = new(mockClock.Object);
            handler.InnerHandler = new MockInnerHandler();

            HttpMessageInvoker invoker = new(handler);
            string uniqueIp = $"10.0.1.{new Random().Next(1, 255)}";

            for (int i = 0; i < 11; i++)
            {
                HttpRequestMessage request = new(HttpMethod.Get, "http://localhost/api/auth/token");
                request.Headers.Add("X-Forwarded-For", uniqueIp);

                await invoker.SendAsync(
                    request,
                    CancellationToken.None);
            }

            HttpRequestMessage blockedRequest = new(HttpMethod.Get, "http://localhost/api/auth/token");
            blockedRequest.Headers.Add("X-Forwarded-For", uniqueIp);

            HttpResponseMessage response = await invoker.SendAsync(
                blockedRequest,
                CancellationToken.None);

            Assert.IsTrue(response.Headers.Contains("Retry-After"));
        }

        /// <summary>
        /// Tests whether the handler resets after the window expires.
        /// </summary>
        [TestMethod]
        public async Task TestRateLimitResetsAfterWindow()
        {
            DateTime currentTime = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.UtcNow)
                .Returns(() => currentTime);

            RateLimitHandler handler = new(mockClock.Object);
            handler.InnerHandler = new MockInnerHandler();

            HttpMessageInvoker invoker = new(handler);
            string uniqueIp = $"10.0.2.{new Random().Next(1, 255)}";

            for (int i = 0; i < 10; i++)
            {
                HttpRequestMessage request = new(HttpMethod.Get, "http://localhost/api/auth/token");
                request.Headers.Add("X-Forwarded-For", uniqueIp);

                await invoker.SendAsync(
                    request,
                    CancellationToken.None);
            }

            currentTime = currentTime.AddSeconds(61);

            HttpRequestMessage resetRequest = new(HttpMethod.Get, "http://localhost/api/auth/token");
            resetRequest.Headers.Add("X-Forwarded-For", uniqueIp);

            HttpResponseMessage response = await invoker.SendAsync(
                resetRequest,
                CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        /// <summary>
        /// Tests whether different IPs are tracked independently.
        /// </summary>
        [TestMethod]
        public async Task TestRateLimitPerIP()
        {
            Mock<IClock> mockClock = new();
            mockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            RateLimitHandler handler = new(mockClock.Object);
            handler.InnerHandler = new MockInnerHandler();

            HttpMessageInvoker invoker = new(handler);
            string ip1 = $"10.0.3.{new Random().Next(1, 255)}";
            string ip2 = $"10.0.4.{new Random().Next(1, 255)}";

            for (int i = 0; i < 10; i++)
            {
                HttpRequestMessage request = new(HttpMethod.Get, "http://localhost/api/auth/token");
                request.Headers.Add("X-Forwarded-For", ip1);

                await invoker.SendAsync(
                    request,
                    CancellationToken.None);
            }

            HttpRequestMessage otherIpRequest = new(HttpMethod.Get, "http://localhost/api/auth/token");
            otherIpRequest.Headers.Add("X-Forwarded-For", ip2);

            HttpResponseMessage response = await invoker.SendAsync(
                otherIpRequest,
                CancellationToken.None);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        /// <summary>
        /// Returns a 200 OK response for any request.
        /// </summary>
        private class MockInnerHandler : HttpMessageHandler
        {
            /// <summary>
            /// Sends the request and returns a 200 OK response.
            /// </summary>
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}
