// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Configuration
{
    /// <summary>
    /// </summary>
    public class DowntimeModel
    {
        /// <summary>
        /// The time that the server goes offline.
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// The amount of time, in seconds, that the server is offline.
        /// </summary>
        public int? Duration { get; set; }
    }
}