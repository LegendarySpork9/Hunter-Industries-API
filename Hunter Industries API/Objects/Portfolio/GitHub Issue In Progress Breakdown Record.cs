// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class GitHubIssueInProgressBreakdownRecord
    {
        /// <summary>
        /// The id number of the issue.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the assignee.
        /// </summary>
        public string Assignee { get; set; }
        /// <summary>
        /// The title of the issue.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The type of issue.
        /// </summary>
        public string Type { get; set; }
    }
}