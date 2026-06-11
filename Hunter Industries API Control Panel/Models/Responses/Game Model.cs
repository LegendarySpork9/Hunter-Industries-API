// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration game api response.
    /// </summary>
    public class GameModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Version { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
