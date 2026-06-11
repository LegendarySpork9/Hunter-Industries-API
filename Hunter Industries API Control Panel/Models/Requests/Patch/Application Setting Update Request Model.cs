// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the application setting data for the api request.
    /// </summary>
    public class ApplicationSettingUpdateRequestModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public bool? Required { get; set; }
    }
}
