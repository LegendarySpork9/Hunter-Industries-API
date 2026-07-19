// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the portfolio statistics api response.
    /// </summary>
    public class PortfolioStatisticsModel
    {
        public required PortfolioStatsModel Metrics { get; set; }
        public required List<TopFiveViewedItemsRecord> TopFiveViewedItems { get; set; }
        public required List<TopFiveModel> TopFiveFrameworks { get; set; }
        public required List<TopFiveModel> TopFiveLanguages { get; set; }
        public required List<TopFiveModel> TopFiveEnvironments { get; set; }
        public required List<LLMUsedModel> LLMUsed { get; set; }
    }
}