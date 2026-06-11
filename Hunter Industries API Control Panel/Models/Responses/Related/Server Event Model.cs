// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server latest event data.
    /// </summary>
    public class ServerEventModel
    {
        public required string Component { get; set; }
        public required string Status { get; set; }
        public required DateTime DateOccured { get; set; }
    }
}
