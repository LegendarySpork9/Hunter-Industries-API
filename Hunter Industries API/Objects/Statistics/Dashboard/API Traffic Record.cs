// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Dashboard
{
    /// <summary>
    /// </summary>
    public class APITrafficRecord
    {
        /// <summary>
        /// The day of the calls.
        /// </summary>
        public string Day { get; set; }
        /// <summary>
        /// The number of sucessful API calls.
        /// </summary>
        public int SuccessfulCalls { get; set; }
        /// <summary>
        /// The number of unsucessful API calls.
        /// </summary>
        public int UnsuccessfulCalls { get; set; }
    }
}