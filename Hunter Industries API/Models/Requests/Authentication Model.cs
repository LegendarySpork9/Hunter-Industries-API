using Microsoft.Extensions.Primitives;

namespace HunterIndustriesAPI.Models.Requests
{
    /// <summary>
    /// </summary>
    public class AuthenticationModel
    {
        /// <summary>
        /// The application identifier.
        /// </summary>
        public string Phrase { get; set; }
        /// <summary>
        /// The basic authorisation header.
        /// </summary>
        public StringValues AuthHeader { get; set; }
        /// <summary>
        /// The decoded username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The decoded password.
        /// </summary>
        public string Password { get; set; }
    }
}