// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Media
{
    /// <summary>
    /// </summary>
    public class MediaUpdateModel
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The size in bytes of the file.
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// The folder path to the file.
        /// </summary>
        public string Path { get; set; } 
    }
}