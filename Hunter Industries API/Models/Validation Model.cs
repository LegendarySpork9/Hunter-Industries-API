namespace HunterIndustriesAPI.Models
{
    /// <summary>
    /// </summary>
    public static class ValidationModel
    {
        /// <summary>
        /// The valid issuer of the token.
        /// </summary>
        public static string Issuer { get; set; }
        /// <summary>
        /// The valid audience of the token.
        /// </summary>
        public static string Audience { get; set; }
        /// <summary>
        /// The valid secret key of the token.
        /// </summary>
        public static string SecretKey { get; set; }
    }
}