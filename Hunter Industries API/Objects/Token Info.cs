using System;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// The application the call was made from.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// The scopes the token can access.
        /// </summary>
        public IEnumerable<string> Scope { get; set; }
        /// <summary>
        /// The date and time it was issued.
        /// </summary>
        public DateTime Issued { get; set; }
        /// <summary>
        /// The date and time it expires.
        /// </summary>
        public DateTime Expires { get; set; }
    }
}