// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the downtime data for the api request.
    /// </summary>
    public class DowntimeRequestModel
    {
        public required string Time { get; set; }
        public required int Duration { get; set; }
    }
}
