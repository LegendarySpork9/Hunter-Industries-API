namespace HunterIndustriesAPI.Models.Responses.Assistant
{
    /// <summary>
    /// </summary>
    public class DeletionResponseModel
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
        /// Whether the assistant should be deleted.
        /// </summary>
        public bool Deletion { get; set; }
    }
}