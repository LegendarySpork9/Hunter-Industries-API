// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class ConnectionMapper
    {
        /// <summary>
        /// Maps the connection model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this ConnectionModel connection)
        {
            return new()
            {
                Id = connection.Id,
                Name = $"{connection.IPAddress}:{connection.Port}",
                IsDeleted = connection.IsDeleted
            };
        }
    }
}
