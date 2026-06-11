// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the object data for the configuration list.
    /// </summary>
    public class ConfigurationListObjectModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
