// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the authorisation data for the api request.
    /// </summary>
    public class AuthorisationRequestModel
    {
        public required string Phrase { get; set; }
    }
}
