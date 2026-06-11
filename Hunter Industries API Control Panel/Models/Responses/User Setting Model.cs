// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the user setting api response.
    /// </summary>
    public class UserSettingModel
    {
        public required string Application { get; set; }
        public required List<SettingModel> Settings { get; set; }
    }
}
