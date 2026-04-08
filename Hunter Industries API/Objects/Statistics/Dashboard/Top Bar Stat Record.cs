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
        public MonthlyStatRecord Calls { get; set; }
        /// <summary>
        /// The number of login attempts made.
        /// </summary>
        public MonthlyStatRecord LoginAttempts { get; set; }
        /// <summary>
        /// The number of changes made.
        /// </summary>
        public MonthlyStatRecord Changes { get; set; }
        /// <summary>
        /// The number of errors occured.
        /// </summary>
        public MonthlyStatRecord Errors { get; set; }
    }
}
