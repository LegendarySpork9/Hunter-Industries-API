// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the media type information.
    /// </summary>
    public class MediaTypeModel
    {
        public required string Extension { get; set; }
        public required string MimeType { get; set; }
    }
}
