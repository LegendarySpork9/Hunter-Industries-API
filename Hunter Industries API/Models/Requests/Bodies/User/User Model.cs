using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Requests.Bodies.User
{
    /// <summary>
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// The scopes applied to the user.
        /// </summary>
        public List<string> Scopes { get; set; }
    }
}