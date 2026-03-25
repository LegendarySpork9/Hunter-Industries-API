// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class ApplicationSettingRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the setting.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Whether the setting is required.
        /// </summary>
        public bool Required { get; set; }
    }
}