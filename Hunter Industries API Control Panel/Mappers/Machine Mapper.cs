// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class MachineMapper
    {
        /// <summary>
        /// Maps the machine model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this MachineModel machine)
        {
            return new()
            {
                Id = machine.Id,
                Name = machine.HostName,
                IsDeleted = machine.IsDeleted
            };
        }
    }
}
