// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the endpoint call numbers.
    /// </summary>
    public class EndpointCallModel
    {
        public required string Endpoint { get; set; }
        public required int Calls { get; set; }
    }
}
