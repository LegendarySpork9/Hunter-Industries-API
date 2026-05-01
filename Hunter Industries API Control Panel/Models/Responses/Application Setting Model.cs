// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the application setting api response.
    /// </summary>
    public class ApplicationSettingModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required bool Required { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
