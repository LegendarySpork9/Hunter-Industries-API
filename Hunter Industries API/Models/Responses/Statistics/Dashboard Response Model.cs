// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using HunterIndustriesAPI.Objects.Statistics.Shared;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses
{
    /// <summary>
    /// </summary>
    public class DashboardResponseModel
    {
        /// <summary>
        /// The top bar record.
        /// </summary>
        public TopBarStatRecord Metric { get; set; }
        /// <summary>
        /// The API traffic records.
        /// </summary>
        public List<APITrafficRecord> APITraffic { get; set; }
        /// <summary>
        /// The error records.
        /// </summary>
        public List<IPAndSummaryErrorRecord> Errors { get; set; }
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
        /// <summary>
        /// The login attempt records.
        /// </summary>
        public List<LoginAttemptRecord> LoginAttempts { get; set; }
        /// <summary>
        /// The server health records.
        /// </summary>
        public List<ServerHealthOverviewRecord> ServerHealth { get; set; }
    }
}