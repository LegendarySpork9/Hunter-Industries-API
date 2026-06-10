// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the api traffic data.
    /// </summary>
    public class APITrafficModel
    {
        public required string Day { get; set; }
        public required int SuccessfulCalls { get; set; }
        public required int UnsuccessfulCalls { get; set; }
    }
}
