// Copyright © - Unpublished - Toby Hunter
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
        public static string FetchIpAddress(HttpRequestBase request) => request.Headers["CF-Connecting-IP"] ?? request.Headers["X-Forwarded-For"] ?? request.UserHostAddress;
    }
}
