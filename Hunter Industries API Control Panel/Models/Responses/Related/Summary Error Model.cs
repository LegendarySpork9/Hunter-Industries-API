// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the summary error data.
    /// </summary>
    public class SummaryErrorModel
    {
        public required string Summary { get; set; }
        public required int Errors { get; set; }
    }
}
