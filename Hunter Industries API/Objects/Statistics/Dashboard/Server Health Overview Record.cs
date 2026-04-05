// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Dashboard
{
    /// <summary>
    /// </summary>
    public class ServerHealthOverviewRecord
    {
        /// <summary>
        /// The id number of the server.
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// The percentage of time the server was online.
        /// </summary>
        public float Uptime { get; set; }
        /// <summary>
        /// The number of logged events.
        /// </summary>
        public int Events { get; set; }
        /// <summary>
        /// The number of logged alerts.
        /// </summary>
        public int Alerts { get; set; }
    }
}