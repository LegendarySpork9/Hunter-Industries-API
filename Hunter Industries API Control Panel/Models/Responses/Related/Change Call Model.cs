// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the status call numbers.
    /// </summary>
    public class StatusCallModel
    {
        public required string Status { get; set; }
        public required int Calls { get; set; }
    }
}
