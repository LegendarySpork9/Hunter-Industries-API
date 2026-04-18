// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration application api response.
    /// </summary>
    public class ApplicationModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required AuthorisationModel Authorisation { get; set; }
        public required List<ApplicationSettingModel> Settings { get; set; }
        public bool IsDeleted { get; set; }
    }
}
