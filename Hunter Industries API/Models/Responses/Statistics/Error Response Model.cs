// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Error;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses
{
    /// <summary>
    /// </summary>
    public class ErrorResponseModel
    {
        /// <summary>
        /// The error records over a period of time.
        /// </summary>
        public List<ErrorOverTimeRecord> Errors { get; set; }
        /// <summary>
        /// The error records by ip.
        /// </summary>
        public List<IPErrorRecord> IPErrors { get; set; }
        /// <summary>
        /// The error records by summary.
        /// </summary>
        public List<SummaryErrorRecord> SummaryErrors { get; set; }
    }
}