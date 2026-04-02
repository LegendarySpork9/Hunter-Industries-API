// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.Configuration
{
    /// <summary>
    /// </summary>
    public class ConnectionModel
    {
        /// <summary>
        /// The ip address of the game.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The port the server runs on.
        /// </summary>
        public int? Port { get; set; }
    }
}