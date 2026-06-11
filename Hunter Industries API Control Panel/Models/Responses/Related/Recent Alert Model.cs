// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the recent alert data.
    /// </summary>
    public class RecentAlertModel
    {
        public required int Id { get; set; }
        public required string Reporter { get; set; }
        public required string Component { get; set; }
        public required string ComponentStatus { get; set; }
        public required string AlertStatus { get; set; }
        public required DateTime AlertDate { get; set; }
    }
}
