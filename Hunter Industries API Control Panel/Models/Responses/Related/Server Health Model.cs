// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server health data.
    /// </summary>
    public class ServerHealthModel
    {
        public required int ServerId { get; set; }
        public required string Name { get; set; }
        public required decimal Uptime { get; set; }
    }
}
