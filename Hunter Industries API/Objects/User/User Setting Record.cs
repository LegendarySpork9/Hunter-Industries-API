using System.Collections.Generic;

namespace HunterIndustriesAPI.Objects.User
{
    /// <summary>
    /// </summary>
    public class UserSettingRecord
    {
        /// <summary>
        /// The application the settings relate to.
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// The settings applied to the user.
        /// </summary>
        public List<SettingRecord> Settings { get; set; } = new List<SettingRecord>();
    }
}