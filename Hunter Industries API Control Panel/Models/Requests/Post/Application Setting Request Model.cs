// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the application setting data for the api request.
    /// </summary>
    public class ApplicationSettingRequestModel
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required bool Required { get; set; }
    }
}
