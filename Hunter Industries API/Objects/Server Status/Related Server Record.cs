// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class RelatedServerRecord
    {
        /// <summary>
        /// The id number of the server.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the server machine.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public string Game { get; set; }
        /// <summary>
        /// The version of the game.
        /// </summary>
        public string GameVersion { get; set; }
    }
}