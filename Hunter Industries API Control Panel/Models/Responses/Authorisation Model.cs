// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration authorisation api response.
    /// </summary>
    public class AuthorisationModel
    {
        public required int Id { get; set; }
        public required string Phrase { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
