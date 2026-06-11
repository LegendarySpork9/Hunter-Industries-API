// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server component alert data.
    /// </summary>
    public class ComponentAlertModel
    {
        public required string Component { get; set; }
        public required int Alerts { get; set; }
    }
}
