// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the media api response.
    /// </summary>
    public class MediaModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required MediaTypeModel Type { get; set; }
        public required long Size { get; set; }
        public string? Path { get; set; }
        public required string Domain { get; set; }
        public required string URL { get; set; }
        public required string Application { get; set; }
        public required DateTime DateUploaded { get; set; }
        public required DateTime DateUpdated { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
