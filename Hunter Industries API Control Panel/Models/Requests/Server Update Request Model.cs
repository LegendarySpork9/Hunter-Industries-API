// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests
{
    /// <summary>
    /// Stores the server data for the api request.
    /// </summary>
    public class ServerUpdateRequestModel
    {
        public string? Name { get; set; }
        public int? EventInterval { get; set; }
        public string? HostName { get; set; }
        public string? Game { get; set; }
        public string? GameVersion { get; set; }
        public string? IPAddress { get; set; }
        public int? Port { get; set; }
        public string? Time { get; set; }
        public int? Duration { get; set; }
        public bool? IsActive { get; set; }
    }
}
