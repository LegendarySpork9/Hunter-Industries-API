// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server downtime data.
    /// </summary>
    public class ServerDowntimeModel
    {
        public required string Time { get; set; }
        public required int Duration { get; set; }
    }
}
