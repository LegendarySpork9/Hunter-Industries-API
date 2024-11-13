namespace HunterIndustriesAPI.Models.Requests.Bodies.Assistant
{
    /// <summary>
    /// </summary>
    public class LocationModel
    {
        /// <summary>
        /// The name of the machine where the assistant is assigned.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// The IP Address of the machine the assistant is located on.
        /// </summary>
        public string IPAddress { get; set; }
    }
}