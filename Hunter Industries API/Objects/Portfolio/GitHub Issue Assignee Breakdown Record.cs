// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class GitHubIssueAssigneeBreakdownRecord
    {
        /// <summary>
        /// The name of the assignee.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The number of open issues assigned to the assignee.
        /// </summary>
        public int Issues { get; set; }
    }
}