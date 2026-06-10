// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the user setting data for the update api request.
    /// </summary>
    public class UserSettingUpdateRequestModel
    {
        public required string Value { get; set; }
    }
}
