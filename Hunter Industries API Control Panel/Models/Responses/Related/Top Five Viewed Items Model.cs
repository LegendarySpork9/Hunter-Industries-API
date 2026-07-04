// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the top five viewed items data.
    /// </summary>
    public class TopFiveViewedItemsRecord
    {
        public required string Name { get; set; }
        public required int SummaryViews { get; set; }
        public required int FullDetailViews { get; set; }
        public required int TotalViews { get; set; }
    }
}