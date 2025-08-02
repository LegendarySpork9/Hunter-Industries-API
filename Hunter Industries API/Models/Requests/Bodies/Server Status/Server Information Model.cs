namespace HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerInformationModel
    {
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
        /// The IPAddress of the game.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The Port the server runs on.
        /// </summary>
        public int Port { get; set; }
    }
}