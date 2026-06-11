// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the server data for the api request.
    /// </summary>
    public class ServerRequestModel
    {
        public required string Name { get; set; }
        public required int EventInterval { get; set; }
        public required string HostName { get; set; }
        public required string Game { get; set; }
        public required string GameVersion { get; set; }
        public required string IPAddress { get; set; }
        public required int Port { get; set; }
        public string? Time { get; set; }
        public int? Duration { get; set; }
    }
}
