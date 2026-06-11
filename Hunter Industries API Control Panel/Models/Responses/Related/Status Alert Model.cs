// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server status alert data.
    /// </summary>
    public class StatusAlertModel
    {
        public required string Status { get; set; }
        public required int Alerts { get; set; }
    }
}
