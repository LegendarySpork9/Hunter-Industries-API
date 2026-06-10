// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the user and application statistics api response.
    /// </summary>
    public class SharedStatisticsModel
    {
        public required List<EndpointCallModel> EndpointCalls { get; set; }
        public required List<MethodCallModel> MethodCalls { get; set; }
        public required List<StatusCallModel> StatusCalls { get; set; }
        public required List<ChangeCallModel> Changes { get; set; }
    }
}
