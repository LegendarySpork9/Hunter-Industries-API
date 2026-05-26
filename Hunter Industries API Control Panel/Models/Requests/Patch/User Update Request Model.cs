// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the user data for the update api request.
    /// </summary>
    public class UserUpdateRequestModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public List<string>? Scopes { get; set; }
    }
}
