// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses.Related
{
    /// <summary>
    /// Stores the server connection data.
    /// </summary>
    public class ServerConnectionModel
    {
        public required string IpAddress { get; set; }
        public required int Port { get; set; }
    }
}
