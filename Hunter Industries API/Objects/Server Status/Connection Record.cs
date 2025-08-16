namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ConnectionRecord
    {
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