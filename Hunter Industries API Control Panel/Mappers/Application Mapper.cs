// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class ApplicationMapper
    {
        /// <summary>
        /// Maps the application model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this ApplicationModel application)
        {
            return new()
            {
                Id = application.Id,
                Name = application.Name,
                IsDeleted = application.IsDeleted
            };
        }
    }
}
