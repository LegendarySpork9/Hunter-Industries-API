// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class DomainMapper
    {
        /// <summary>
        /// Maps the domain model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this DomainModel domain)
        {
            return new()
            {
                Id = domain.Id,
                Name = domain.Host,
                IsDeleted = domain.IsDeleted
            };
        }
    }
}
