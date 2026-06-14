// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration domain api response.
    /// </summary>
    public class DomainModel
    {
        public required int Id { get; set; }
        public required string Host { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
