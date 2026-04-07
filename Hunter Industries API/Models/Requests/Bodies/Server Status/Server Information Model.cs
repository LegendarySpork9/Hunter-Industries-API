// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerInformationModel
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The time, in seconds, between server events.
        /// </summary>
        public int EventInterval { get; set; }
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
        /// <summary>
        /// The ip address of the game.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The port the server runs on.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The time that the server goes offline.
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// The amount of time, in seconds, that the server is offline.
        /// </summary>
        public int Duration { get; set; }
    }
}