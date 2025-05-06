using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public class TokenConverter
    {
        /// <summary>
        /// Converts the scope into a claim.
        /// </summary>
        public Claim[] GetClaims(List<string> scopes)
        {
            Claim[] claims = Array.Empty<Claim>();

            foreach (string scope in scopes)
            {
                claims = claims.Append(new Claim("scope", scope)).ToArray();
            }

            return claims;
        }
    }
}