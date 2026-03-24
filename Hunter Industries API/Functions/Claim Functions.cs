// Copyright © - Unpublished - Toby Hunter
using System.Linq;
using System.Security.Claims;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class ClaimFunctions
    {
        /// <summary>
        /// Extracts the username from the claims principal.
        /// </summary>
        public static string GetUsername(ClaimsPrincipal principal)
        {
            return principal?.Claims?.FirstOrDefault(c => c.Type == "username")?.Value;
        }

        /// <summary>
        /// Extracts the application name from the claims principal.
        /// </summary>
        public static string GetApplicationName(ClaimsPrincipal principal)
        {
            return principal?.Claims?.FirstOrDefault(c => c.Type == "application")?.Value;
        }
    }
}
