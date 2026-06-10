// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Patch
{
    /// <summary>
    /// Stores the game data for the api request.
    /// </summary>
    public class GameUpdateRequestModel
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
    }
}
