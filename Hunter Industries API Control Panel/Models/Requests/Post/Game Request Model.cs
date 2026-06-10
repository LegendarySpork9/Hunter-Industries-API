// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Requests.Post
{
    /// <summary>
    /// Stores the game data for the api request.
    /// </summary>
    public class GameRequestModel
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
    }
}
