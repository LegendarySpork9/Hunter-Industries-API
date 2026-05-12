// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the error summary data.
    /// </summary>
    public class ErrorSummaryModel
    {
        public required string Month { get; set; }
        public required int Errors { get; set; }
    }
}
