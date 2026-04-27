// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Server
{
    /// <summary>
    /// </summary>
    public class ServerHealthOverviewRecord
    {
        /// <summary>
        /// The day of the uptime.
        /// </summary>
        public string Day { get; set; }
        /// <summary>
        /// The percentage of time the server was online.
        /// </summary>
        public decimal Uptime { get; set; }
    }
}