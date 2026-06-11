// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration downtime api response.
    /// </summary>
    public class DowntimeModel
    {
        public required int Id { get; set; }
        public required string Time { get; set; }
        public required int Duration { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
