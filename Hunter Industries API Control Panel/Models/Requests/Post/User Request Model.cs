// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the user data for the api request.
    /// </summary>
    public class UserRequestModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required List<string> Scopes { get; set; }
    }
}
