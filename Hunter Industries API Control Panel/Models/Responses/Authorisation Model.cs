// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration authorisation api response.
    /// </summary>
    public class AuthorisationModel
    {
        public int Id { get; set; }
        public required string Phrase { get; set; }
        public bool IsDeleted { get; set; }
    }
}
