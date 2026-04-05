// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Dashboard
{
    /// <summary>
    /// </summary>
    public class LoginAttemptRecord
    {
        /// <summary>
        /// The name of the user that logged in.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The name of the application that made the call.
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// The successful number of login attempts made.
        /// </summary>
        public int SuccessfulAttempts { get; set; }
        /// <summary>
        /// The unsuccessful number of login attempts made.
        /// </summary>
        public int UnsuccessfulAttempts { get; set; }
        /// <summary>
        /// The total number of login attempts made.
        /// </summary>
        public int TotalAttempts { get; set; }
    }
}