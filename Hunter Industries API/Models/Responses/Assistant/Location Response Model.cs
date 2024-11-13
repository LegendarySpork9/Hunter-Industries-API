namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    /// <summary>
    /// </summary>
    public class LocationResponseModel
    {
        /// <summary>
        /// The name of the assistant.
        /// </summary>
        public string AssistantName { get; set; }
        /// <summary>
        /// The id of the assistant.
        /// </summary>
        public string IdNumber { get; set; }
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