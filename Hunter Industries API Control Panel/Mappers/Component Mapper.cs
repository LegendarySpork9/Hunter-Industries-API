// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class ComponentMapper
    {
        /// <summary>
        /// Maps the component model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this ComponentModel component)
        {
            return new()
            {
                Id = component.Id,
                Name = component.Name,
                IsDeleted = component.IsDeleted
            };
        }
    }
}
