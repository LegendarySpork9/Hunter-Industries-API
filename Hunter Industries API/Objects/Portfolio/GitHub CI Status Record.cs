// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class GitHubCIStatusRecord
    {
        /// <summary>
        /// The name of the CI workflow.
        /// </summary>
        public string Workflow { get; set; }
        /// <summary>
        /// The status of the CI workflow.
        /// </summary>
        public string Status { get; set; }
    }
}