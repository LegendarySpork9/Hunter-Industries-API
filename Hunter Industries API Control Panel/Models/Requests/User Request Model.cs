// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests
{
    /// <summary>
    /// Stores the user data for the api request.
    /// </summary>
    public class UserRequestModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public List<string>? Scopes { get; set; }
    }
}
