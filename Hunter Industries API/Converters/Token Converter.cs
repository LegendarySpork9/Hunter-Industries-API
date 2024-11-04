using HunterIndustriesAPI.Services;
using System.Security.Claims;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public class TokenConverter
    {
        private readonly TokenService TokenService;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public TokenConverter(TokenService _tokenService)
        {
            TokenService = _tokenService;
        }

        /// <summary>
        /// Converts the scope into a claim.
        /// </summary>
        public Claim[] GetClaims(string username)
        {
            switch (IsAdmin())
            {
                case true:

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("scope", "Assistant API"),
                        new Claim("scope", "Book Reader API"),
                        new Claim("scope", "Control Panel API")
                    };

                    return claims;

                default:

                    claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("scope", GetScope())
                    };

                    return claims;
            }
        }

        /// <summary>
        /// Returns whether the user is an admin.
        /// </summary>
        private bool IsAdmin()
        {
            switch (TokenService.ApplicationName())
            {
                case "API Admin": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns the scope based on the application name.
        /// </summary>
        private string GetScope()
        {
            switch (TokenService.ApplicationName())
            {
                case "Virtual Assistant": return "Assistant API";
                default: return string.Empty;
            }
        }
    }
}