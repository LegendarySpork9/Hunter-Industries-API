// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the application data for the api request.
    /// </summary>
    public class ApplicationRequestModel
    {
        public required string Name { get; set; }
        public required string Phrase { get; set; }
    }
}
