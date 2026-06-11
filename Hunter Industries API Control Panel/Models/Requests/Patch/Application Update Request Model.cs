// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the application data for the api request.
    /// </summary>
    public class ApplicationUpdateRequestModel
    {
        public string? Name { get; set; }
        public string? Phrase { get; set; }
    }
}
