// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Server;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses.Statistics
{
    /// <summary>
    /// </summary>
    public class ServerResponseModel
    {
        /// <summary>
        /// The alerts by component.
        /// </summary>
        public List<AlertComponentRecord> ComponentAlerts { get; set; }
        /// <summary>
        /// The alerts by status.
        /// </summary>
        public List<AlertStatusRecord> StatusAlerts { get; set; }
        /// <summary>
        /// The most recent events per component.
        /// </summary>
        public List<EventComponentRecord> LatestEvents { get; set; }
        /// <summary>
        /// The recent alert records.
        /// </summary>
        public List<RecentAlertRecord> RecentAlerts { get; set; }
        /// <summary>
        /// The recent event records.
        /// </summary>
        public List<EventComponentRecord> RecentEvents { get; set; }
    }
}