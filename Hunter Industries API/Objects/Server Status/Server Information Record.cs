namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerInformationRecord
    {
        /// <summary>
        /// The id number of the server.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the server.
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
        /// <summary>
        /// The IPAddress of the game
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Whether the server is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}