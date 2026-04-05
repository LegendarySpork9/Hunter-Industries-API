// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Dashboard
{
    /// <summary>
    /// </summary>
    public class IPAndSummaryErrorRecord
    {
        /// <summary>
        /// The ip address of the failed call.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The summary of the error.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// The number of errors.
        /// </summary>
        public int Errors { get; set; }
    }
}