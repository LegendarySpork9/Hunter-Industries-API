// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the connection data for the api request.
    /// </summary>
    public class ConnectionRequestModel
    {
        public required string IPAddress { get; set; }
        public required int Port { get; set; }
    }
}
