// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Mappers
{
    public static class GameMapper
    {
        /// <summary>
        /// Maps the game model to the configuration object list model.
        /// </summary>
        public static ConfigurationListObjectModel ToListObject(this GameModel game)
        {
            return new()
            {
                Id = game.Id,
                Name = $"{game.Name} ({game.Version})",
                IsDeleted = game.IsDeleted
            };
        }
    }
}
