// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the portfolio stats data.
    /// </summary>
    public class PortfolioStatsModel
    {
        public required int Items { get; set; }
        public required int Filters { get; set; }
        public required int AIUsed { get; set; }
    }
}
