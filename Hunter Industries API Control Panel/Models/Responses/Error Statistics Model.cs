// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the error statistics api response.
    /// </summary>
    public class ErrorStatisticsModel
    {
        public required List<ErrorSummaryModel> Errors { get; set; }
        public required List<IPErrorModel> IPErrors { get; set; }
        public required List<SummaryErrorModel> SummaryErrors { get; set; }
    }
}
