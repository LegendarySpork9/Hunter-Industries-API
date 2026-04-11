// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerAlertModel
    {
        /// <summary>
        /// The name of the reporter.
        /// </summary>
        public string Reporter { get; set; }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// The status of the component.
        /// </summary>
        public string ComponentStatus { get; set; }
        /// <summary>
        /// The status of the alert.
        /// </summary>
        public string AlertStatus { get; set; }
        /// <summary>
        /// The id number of the server.
        /// </summary>
        public int ServerId { get; set; }
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