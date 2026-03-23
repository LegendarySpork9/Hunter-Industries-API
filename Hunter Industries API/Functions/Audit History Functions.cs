// Copyright © - Unpublished - Toby Hunter
using System.Net.Http;
using System.Text.RegularExpressions;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class AuditHistoryFunctions
    {
        /// <summary>
        /// Extracts the API version from the request URL.
        /// </summary>
        public static string ExtractVersionFromRequest(HttpRequestMessage request)
        {
            Match match = Regex.Match(request.RequestUri.AbsolutePath, @"/v(\d+\.\d+)/");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
