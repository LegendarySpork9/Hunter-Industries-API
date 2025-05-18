namespace HunterIndustriesAPI.Models.Requests.Bodies.User
{
    /// <summary>
    /// </summary>
    public class UserSettingsModel
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Username { get; set; }
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