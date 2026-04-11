// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Error;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class ErrorDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the error over time record.
        /// </summary>
        public static readonly Func<IDataReader, ErrorOverTimeRecord> ErrorOverTimeMapper = reader =>
        {
            ErrorOverTimeRecord errorOverTime = new ErrorOverTimeRecord
            {
                Month = reader.GetString(0),
                Errors = reader.GetInt32(1)
            };

            return errorOverTime;
        };

        /// <summary>
        /// The SQL row to model mappings for the IP error record.
        /// </summary>
        public static readonly Func<IDataReader, IPErrorRecord> IPErrorMapper = reader =>
        {
            IPErrorRecord ipError = new IPErrorRecord
            {
                IPAddress = reader.GetString(0),
                Errors = reader.GetInt32(1)
            };

            return ipError;
        };

        /// <summary>
        /// The SQL row to model mappings for the summary error record.
        /// </summary>
        public static readonly Func<IDataReader, SummaryErrorRecord> SummaryErrorMapper = reader =>
        {
            SummaryErrorRecord summaryError = new SummaryErrorRecord
            {
                Summary = reader.GetString(0),
                Errors = reader.GetInt32(1)
            };

            return summaryError;
        };
    }
}