// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the downtime data for the api request.
    /// </summary>
    public class DowntimeUpdateRequestModel
    {
        public string? Time { get; set; }
        public int? Duration { get; set; }
    }
}
