// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class DowntimeMapper
    {
        /// <summary>
        /// Maps the downtime model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this DowntimeModel downtime)
        {
            return new()
            {
                Id = downtime.Id,
                Name = $"{downtime.Time} ({downtime.Duration})",
                IsDeleted = downtime.IsDeleted
            };
        }
    }
}
