// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the user setting data for the api request.
    /// </summary>
    public class UserSettingRequestModel
    {
        public required int UserId { get; set; }
        public required string Application { get; set; }
        public required string SettingName { get; set; }
        public required string SettingValue { get; set; }
    }
}
