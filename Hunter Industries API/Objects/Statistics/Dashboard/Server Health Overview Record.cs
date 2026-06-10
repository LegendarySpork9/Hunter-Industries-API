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
        /// The name of the server.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The percentage of time the server was online.
        /// </summary>
        public decimal Uptime { get; set; }
    }
}