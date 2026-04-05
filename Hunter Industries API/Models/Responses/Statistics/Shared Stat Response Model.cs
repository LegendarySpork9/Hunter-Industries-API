// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Shared;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses
{
    /// <summary>
    /// </summary>
    public class SharedStatResponseModel
    {
        /// <summary>
        /// The endpoint call records.
        /// </summary>
        public List<EndpointCallRecord> EndpointCalls { get; set; }
        /// <summary>
        /// The method call records.
        /// </summary>
        public List<MethodCallRecord> MethodCalls { get; set; }
        /// <summary>
        /// The status call records.
        /// </summary>
        public List<StatusCallRecord> StatusCalls { get; set; }
        /// <summary>
        /// The change call records.
        /// </summary>
        public List<ChangeCallRecord> Changes { get; set; }
    }
}