// Copyright © - 11/06/2026 - Toby Hunter
using System.Linq;
using System.Net.Http;
using System.Web;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class IPAddressFunction
    {
        /// <summary>
        /// Returns the IP address for logging.
        /// </summary>
        public static string FetchIpAddress(HttpRequestMessage request)
        {
            if (request.Headers.TryGetValues(
                "CF-Connecting-IP",
                out var cfValues))
            {
                return cfValues.First();
            }

            if (request.Headers.TryGetValues(
                "X-Forwarded-For",
                out var xffValues))
            {
                return xffValues.First();
            }

            return HttpContext.Current?.Request?.UserHostAddress ?? "Unknown";
        }
    }
}
