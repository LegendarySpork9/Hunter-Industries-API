// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Configuration
{
    /// <summary>
    /// </summary>
    public class ConfigurationDowntimeRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The time that the server goes offline.
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// The amount of time, in seconds, that the server is offline.
        /// </summary>
        public int Duration { get; set; }
    }
}