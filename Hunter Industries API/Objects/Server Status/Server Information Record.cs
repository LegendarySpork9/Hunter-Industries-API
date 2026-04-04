// Copyright © - Unpublished - Toby Hunter
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
        /// <summary>
        /// The connection details of the server.
        /// </summary>
        public ConnectionRecord Connection { get; set; }
        /// <summary>
        /// The expected daily downtime of the server.
        /// </summary>
        public DowntimeRecord Downtime { get; set; }
        /// <summary>
        /// The time, in seconds, between server events.
        /// </summary>
        public int EventInterval { get; set; }
        /// <summary>
        /// Whether the server is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}