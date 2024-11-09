using HunterIndustriesAPI.Objects.Assistant;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    /// <summary>
    /// </summary>
    public class ConfigResponseModel
    {
        /// <summary>
        /// The latest assistant release version.
        /// </summary>
        public string LatestRelease { get; set; }
        /// <summary>
        /// The configuration records.
        /// </summary>
        public List<AssistantConfiguration> AssistantConfigurations { get; set; }
        /// <summary>
        /// The number of configurations returned.
        /// </summary>
        public int ConfigCount { get; set; }
        /// <summary>
        /// The total number of configurations.
        /// </summary>
        public int TotalCount { get; set; }
    }
}