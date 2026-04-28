// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration connection api response.
    /// </summary>
    public class ConnectionModel
    {
        public required int Id { get; set; }
        public required string IPAddress { get; set; }
        public required int Port { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
