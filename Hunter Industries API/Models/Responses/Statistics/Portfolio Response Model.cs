// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Portfolio;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses.Statistics
{
    /// <summary>
    /// </summary>
    public class PortfolioResponseModel
    {
        /// <summary>
        /// The top bar record.
        /// </summary>
        public TopBarStatRecord Metrics { get; set; }
        /// <summary>
        /// The top five viewed items record.
        /// </summary>
        public List<TopFiveViewedItemsRecord> TopFiveViewedItems { get; set; }
        /// <summary>
        /// The top five framework records.
        /// </summary>
        public List<TopFiveRecord> TopFiveFrameworks { get; set; }
        /// <summary>
        /// The top five language records.
        /// </summary>
        public List<TopFiveRecord> TopFiveLanguages { get; set; }
        /// <summary>
        /// The top five environment records.
        /// </summary>
        public List<TopFiveRecord> TopFiveEnvironments { get; set; }
        /// <summary>
        /// The llm used records.
        /// </summary>
        public List<LLMUsedRecord> LLMUsed { get; set; }
    }
}