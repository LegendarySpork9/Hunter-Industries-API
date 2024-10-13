using HunterIndustriesAPI.Objects;

namespace HunterIndustriesAPI.Models.Responses
{
    /// <summary>
    /// </summary>
    public class TokenResponseModel
    {
        /// <summary>
        /// The type of token.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The value of the token.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// The time until expiration.
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// The information about the token.
        /// </summary>
        public TokenInfo Info { get; set; }
    }
}