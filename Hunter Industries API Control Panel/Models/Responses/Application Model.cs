// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration application api response.
    /// </summary>
    public class ApplicationModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required AuthorisationModel Authorisation { get; set; }
        public required List<ApplicationSettingModel> Settings { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
