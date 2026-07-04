// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Statistics.Portfolio
{
    /// <summary>
    /// </summary>
    public class TopFiveViewedItemsRecord
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The number of times the summary has been looked at.
        /// </summary>
        public int SummaryViews { get; set; }
        /// <summary>
        /// The number of times the full details have been looked at.
        /// </summary>
        public int FullDetailViews { get; set; }
        /// <summary>
        /// The total number of times the item has been viewed.
        /// </summary>
        public int TotalViews { get; set; }
    }
}