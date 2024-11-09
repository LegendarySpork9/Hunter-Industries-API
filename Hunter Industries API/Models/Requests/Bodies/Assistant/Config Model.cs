namespace HunterIndustriesAPI.Models.Requests.Bodies.Assistant
{
    /// <summary>
    /// </summary>
    public class ConfigModel
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
        /// The user that the assistant is assigned.
        /// </summary>
        public string AssignedUser { get; set; }
        /// <summary>
        /// The name of the machine where the assistant is assigned.
        /// </summary>
        public string HostName { get; set; }
    }
}