// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the LLM Usage data.
    /// </summary>
    public class LLMUsedModel
    {
        public required string Company { get; set; }
        public required string Model { get; set; }
        public required int Uses { get; set; }
    }
}