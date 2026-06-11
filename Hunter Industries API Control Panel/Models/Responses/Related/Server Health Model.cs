// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server health data.
    /// </summary>
    public class ServerHealthModel
    {
        public required string Day { get; set; }
        public required decimal Uptime { get; set; }
    }
}
