// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Media
{
    /// <summary>
    /// </summary>
    public class MediaModel
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of file.
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// The mime type of the file.
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// The size in bytes of the file.
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// The folder path to the file.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// The URL the file is served on.
        /// </summary>
        public string Domain { get; set; }
    }
}