namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class LoginAttemptRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the user attempting to log in.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The phrase the user used to log in.
        /// </summary>
        public string Phrase { get; set; }
        /// <summary>
        /// Whether the attempt was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }
    }
}