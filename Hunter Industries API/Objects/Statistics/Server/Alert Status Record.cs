// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Server
{
    /// <summary>
    /// </summary>
    public class AlertStatusRecord
    {
        /// <summary>
        /// The name of the status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// The number of alerts.
        /// </summary>
        public int Alerts { get; set; }
    }
}