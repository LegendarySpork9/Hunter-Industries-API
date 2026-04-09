// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models
{
    /// <summary>
    /// Stores the settings to access the API.
    /// </summary>
    public class APISettingsModel
    {
        public string BaseURL { get; set; }
        public string Version { get; set; }
        public string Credentials { get; set; }
        public string PayloadLocation { get; set; }
    }
}
