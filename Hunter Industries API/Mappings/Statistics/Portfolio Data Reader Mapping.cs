// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Portfolio;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class PortfolioDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the top bar stats record.
        /// </summary>
        public static readonly Func<IDataReader, TopBarStatRecord> TopBarStatusMapper = reader =>
        {
            TopBarStatRecord topBarStats = new TopBarStatRecord()
            {
                Items = reader.GetInt32(0),
                Filters = reader.GetInt32(1),
                AIUsed = reader.GetInt32(2)
            };

            return topBarStats;
        };

        /// <summary>
        /// The SQL row to model mappings for the top five viewed items record.
        /// </summary>
        public static readonly Func<IDataReader, TopFiveViewedItemsRecord> TopFiveViewedItemMapper = reader =>
        {
            TopFiveViewedItemsRecord topFiveViewedItems = new TopFiveViewedItemsRecord()
            {
                Name = reader.GetString(0),
                SummaryViews = reader.GetInt32(1),
                FullDetailViews = reader.GetInt32(2),
                TotalViews = reader.GetInt32(3)
            };

            return topFiveViewedItems;
        };

        /// <summary>
        /// The SQL row to model mappings for the top five record.
        /// </summary>
        public static readonly Func<IDataReader, TopFiveRecord> TopFiveMapper = reader =>
        {
            TopFiveRecord topFive = new TopFiveRecord()
            {
                Name = reader.GetString(0),
                Uses = reader.GetInt32(1)
            };

            return topFive;
        };

        /// <summary>
        /// The SQL row to model mappings for the LLM used record.
        /// </summary>
        public static readonly Func<IDataReader, LLMUsedRecord> LLMUsedMapper = reader =>
        {
            LLMUsedRecord llmUsed = new LLMUsedRecord()
            {
                Company = reader.GetString(0),
                Model = reader.GetString(1),
                Uses = reader.GetInt32(2)
            };

            return llmUsed;
        };
    }
}