// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class ApplicationSettingMapper
    {
        /// <summary>
        /// Maps the application setting model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this ApplicationSettingModel applicationSetting)
        {
            return new()
            {
                Id = applicationSetting.Id,
                Name = applicationSetting.Name,
                IsDeleted = applicationSetting.IsDeleted
            };
        }
    }
}
