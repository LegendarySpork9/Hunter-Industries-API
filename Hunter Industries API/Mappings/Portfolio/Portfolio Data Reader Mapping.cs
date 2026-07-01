// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Portfolio;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings.Portfolio
{
    /// <summary>
    /// </summary>
    public static class PortfolioDataReaderMapping
    {
        /// <summary>
        /// The SQL row to List mappings for single linked items.
        /// </summary>
        public static readonly Func<IDataReader, object> SingleLinkedItemMapper = reader =>
        {
            return (
                reader.GetInt32(0),
                reader.GetString(1));
        };

        /// <summary>
        /// The SQL row to model mappings for the build history record.
        /// </summary>
        public static readonly Func<IDataReader, object> BuildHistoryMapper = reader =>
        {
            BuildHistoryRecord buildHistory = new BuildHistoryRecord()
            {
                Version = reader.GetString(1),
                ReleaseDate = DateTime.SpecifyKind(
                    reader.GetDateTime(3),
                    DateTimeKind.Utc)
            };

            return (
                reader.GetInt32(0),
                buildHistory);
        };
    }
}