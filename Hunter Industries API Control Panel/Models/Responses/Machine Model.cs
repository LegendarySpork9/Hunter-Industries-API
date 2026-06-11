// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the configuration machine api response.
    /// </summary>
    public class MachineModel
    {
        public required int Id { get; set; }
        public required string HostName { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
