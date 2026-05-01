// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests
{
    /// <summary>
    /// Stores the authorisation data for the api request.
    /// </summary>
    public class AuthorisationRequestModel
    {
        public required string Phrase { get; set; }
    }
}
