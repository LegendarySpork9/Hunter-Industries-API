namespace HunterIndustriesAPI.Objects.Assistant
{
    /// <summary>
    /// </summary>
    public class AssistantConfiguration
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
        /// <summary>
        /// Whether the assistant should be deleted.
        /// </summary>
        public bool Deletion { get; set; }
        /// <summary>
        /// The version of the assistant.
        /// </summary>
        public string Version { get; set; }
    }
}