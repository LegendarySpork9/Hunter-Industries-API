// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Configuration
{
    /// <summary>
    /// </summary>
    public class ApplicationSettingModel
    {
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
        public bool? Required { get; set; }
    }
}