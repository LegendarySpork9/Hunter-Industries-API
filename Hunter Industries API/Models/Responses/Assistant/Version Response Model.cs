namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    /// <summary>
    /// </summary>
    public class VersionResponseModel
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
        /// The version of the assistant.
        /// </summary>
        public string Version { get; set; }
    }
}