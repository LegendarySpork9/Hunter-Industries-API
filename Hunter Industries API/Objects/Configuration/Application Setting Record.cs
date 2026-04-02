// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Configuration
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
        /// The data type of the setting.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Whether the setting is required.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}