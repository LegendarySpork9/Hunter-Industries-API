// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the connection data for the api request.
    /// </summary>
    public class ConnectionUpdateRequestModel
    {
        public string? IPAddress { get; set; }
        public int? Port { get; set; }
    }
}
