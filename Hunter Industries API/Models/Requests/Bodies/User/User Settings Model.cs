// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.User
{
    /// <summary>
    /// </summary>
    public class UserSettingsModel
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The application the settings relate to.
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// The name of the setting.
        /// </summary>
        public string SettingName { get; set; }
        /// <summary>
        /// The value of the setting.
        /// </summary>
        public string SettingValue { get; set; }
    }
}