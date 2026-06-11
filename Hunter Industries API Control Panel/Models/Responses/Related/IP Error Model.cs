// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the ip error data.
    /// </summary>
    public class IPErrorModel
    {
        public required string IPAddress { get; set; }
        public required int Errors { get; set; }
    }
}
