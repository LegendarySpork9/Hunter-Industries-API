// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration api response.
    /// </summary>
    public class ConfigurationModel
    {
        public required List<string> ConfigurationObjects { get; set; }
    }
}
