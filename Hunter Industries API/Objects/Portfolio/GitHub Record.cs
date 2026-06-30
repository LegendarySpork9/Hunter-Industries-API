// Copyright © - Unpublished - Toby Hunter
using System.Collections.Generic;

namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class GitHubRecord
    {
        /// <summary>
        /// The url to the repository.
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// The collection of CI statuses.
        /// </summary>
        public Dictionary<string, string> CIStatus { get; set; }
        /// <summary>
        /// The breakdown of issues.
        /// </summary>
        public GitHubIssueBreakdownRecord IssueBreakdown { get; set; }
        /// <summary>
        /// The collection of assignee breakdowns.
        /// </summary>
        public List<GitHubIssueAssigneeBreakdownRecord> AssigneeBreakdown { get; set; }
        /// <summary>
        /// The collection of in progress issue breakdowns.
        /// </summary>
        public List<GitHubIssueInProgressBreakdownRecord> InProgressBreakdown { get; set; }
    }
}