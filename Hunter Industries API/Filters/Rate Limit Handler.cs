// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// </summary>
    public class RateLimitHandler : DelegatingHandler
    {
        private static readonly ConcurrentDictionary<string, RateLimitEntry> _Clients = new ConcurrentDictionary<string, RateLimitEntry>();
        private static readonly int _MaxRequests;
        private static readonly int _WindowSeconds;
        private readonly IClock _Clock;

        static RateLimitHandler()
        {
            string maxRequestsSetting = ConfigurationManager.AppSettings["RateLimitMaxRequests"];
            string windowSecondsSetting = ConfigurationManager.AppSettings["RateLimitWindowSeconds"];

            _MaxRequests = int.TryParse(
                maxRequestsSetting,
                out int max) ? max : 10;
            _WindowSeconds = int.TryParse(
                windowSecondsSetting,
                out int window) ? window : 60;
        }

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public RateLimitHandler(IClock clock)
        {
            _Clock = clock;
        }

        /// <summary>
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string clientIp = Functions.IPAddressFunction.FetchIpAddress(request);
            RateLimitEntry entry = _Clients.GetOrAdd(
                clientIp,
                _ => new RateLimitEntry(_Clock));

            lock (entry)
            {
                DateTime now = _Clock.UtcNow;

                if ((now - entry.WindowStart) > TimeSpan.FromSeconds(_WindowSeconds))
                {
                    entry.WindowStart = now;
                    entry.RequestCount = 0;
                }

                entry.RequestCount++;

                if (entry.RequestCount > _MaxRequests)
                {
                    HttpResponseMessage response = request.CreateResponse((HttpStatusCode)429);
                    response.Content = new StringContent("{\"error\":\"Too many requests. Please try again later.\"}");
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    int retryAfter = _WindowSeconds - (int)(now - entry.WindowStart).TotalSeconds;
                    response.Headers.Add(
                        "Retry-After",
                        Math.Max(
                            retryAfter,
                            1).ToString());

                    return Task.FromResult(response).Result;
                }
            }

            return await base.SendAsync(
                request,
                cancellationToken);
        }

        /// <summary>
        /// Tracks the request count and window start time for a single client.
        /// </summary>
        private class RateLimitEntry
        {
            public DateTime WindowStart { get; set; }
            public int RequestCount { get; set; }

            public RateLimitEntry(IClock clock)
            {
                WindowStart = clock.UtcNow;
            }
        }
    }
}