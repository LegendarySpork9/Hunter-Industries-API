// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the server statistics api response.
    /// </summary>
    public class ServerStatisticsModel
    {
        public required List<ComponentAlertModel> ComponentAlerts { get; set; }
        public required List<StatusAlertModel> StatusAlerts { get; set; }
        public required List<ServerEventModel> LatestEvents { get; set; }
        public required List<ServerHealthModel> ServerHealth { get; set; }
        public required List<RecentAlertModel> RecentAlerts { get; set; }
        public required List<ServerEventModel> RecentEvents { get; set; }
    }
}
