// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the dashboard statistics api response.
    /// </summary>
    public class DashboardStatisticsModel
    {
        public required MetricModel Metrics { get; set; }
        public required List<APITrafficModel> ApiTraffic { get; set; }
        public required List<MultiGroupedErrorModel> Errors { get; set; }
        public required List<EndpointCallModel> EndpointCalls { get; set; }
        public required List<MethodCallModel> MethodCalls { get; set; }
        public required List<StatusCallModel> StatusCalls { get; set; }
        public required List<ChangeCallModel> Changes { get; set; }
        public required List<LoginAttemptStatModel> LoginAttempts { get; set; }
        public required List<ServerHealthModel> ServerHealth { get; set; }
    }
}
