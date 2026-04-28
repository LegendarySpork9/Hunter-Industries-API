// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the server information api response.
    /// </summary>
    public class ServerInformationModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string HostName { get; set; }
        public required string Game { get; set; }
        public required string GameVersion { get; set; }
        public required ServerConnectionModel Connection { get; set; }
        public ServerDowntimeModel? Downtime { get; set; }
        public required int EventInterval { get; set; }
        public required bool IsActive { get; set; }
    }
}
