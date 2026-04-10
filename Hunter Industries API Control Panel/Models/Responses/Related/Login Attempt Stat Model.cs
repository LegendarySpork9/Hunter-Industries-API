// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the login attempt numbers.
    /// </summary>
    public class LoginAttemptStatModel
    {
        public required string Username { get; set; }
        public required string Application { get; set; }
        public required int SuccessfulAttempts { get; set; }
        public required int UnsuccessfulAttempts { get; set; }
        public required int TotalAttempts { get; set; }
    }
}
