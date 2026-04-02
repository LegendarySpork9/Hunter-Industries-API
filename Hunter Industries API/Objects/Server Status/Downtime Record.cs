// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class DowntimeRecord
    {
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