// Copyright © - 29/06/2026 - Toby Hunter
using System;

namespace HunterIndustriesAPI.Objects.Media
{
    /// <summary>
    /// </summary>
    public class MediaRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type information of the file.
        /// </summary>
        public MediaTypeRecord Type { get; set; }
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
        /// <summary>
        /// The URL to access the the image.
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// The application the file relates to.
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// The date and time the file was uploaded.
        /// </summary>
        public DateTime DateUploaded { get; set; }
        /// <summary>
        /// THe date and time the file was updated.
        /// </summary>
        public DateTime DateUpdated { get; set; }
        /// <summary>
        /// Whether the record is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}