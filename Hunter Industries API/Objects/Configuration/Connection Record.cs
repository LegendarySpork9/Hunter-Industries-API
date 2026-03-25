// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class ConfigurationConnectionRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The ip address of the game.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The port the server runs on.
        /// </summary>
        public int Port { get; set; }
    }
}