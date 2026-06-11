// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the setting information.
    /// </summary>
    public class SettingModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
