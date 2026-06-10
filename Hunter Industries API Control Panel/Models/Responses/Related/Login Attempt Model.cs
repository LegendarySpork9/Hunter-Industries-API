// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the login attempt data.
    /// </summary>
    public class LoginAttemptModel
    {
        public required int Id { get; set; }
        public string? Username { get; set; }
        public string? Phrase { get; set; }
        public required bool IsSuccessful { get; set; }
    }
}
