// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the multi-grouped error data.
    /// </summary>
    public class MultiGroupedErrorModel
    {
        public required string IpAddress { get; set; }
        public required string Summary { get; set; }
        public required int Errors { get; set; }
    }
}
