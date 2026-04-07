// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Dashboard
{
    /// <summary>
    /// </summary>
    public class TopBarStatRecord
    {
        /// <summary>
        /// The number of applications registered.
        /// </summary>
        public int Applications { get; set; }
        /// <summary>
        /// The number of users registered.
        /// </summary>
        public int Users { get; set; }
        /// <summary>
        /// The number of calls made.
        /// </summary>
        public int Calls { get; set; }
        /// <summary>
        /// The number of login attempts made.
        /// </summary>
        public int LoginAttempts { get; set; }
        /// <summary>
        /// The number of changes made.
        /// </summary>
        public int Changes { get; set; }
        /// <summary>
        /// The number of errors occured.
        /// </summary>
        public int Errors { get; set; }
    }
}