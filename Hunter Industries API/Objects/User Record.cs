namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class UserRecord
    {
        /// <summary>
        /// The id number of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}