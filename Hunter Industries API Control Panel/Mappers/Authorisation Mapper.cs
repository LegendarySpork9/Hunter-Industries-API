// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class AuthorisationMapper
    {
        /// <summary>
        /// Maps the authorisation model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this AuthorisationModel authorisation)
        {
            return new()
            {
                Id = authorisation.Id,
                Name = authorisation.Phrase,
                IsDeleted = authorisation.IsDeleted
            };
        }
    }
}
