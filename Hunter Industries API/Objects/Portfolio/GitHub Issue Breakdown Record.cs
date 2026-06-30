// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class GitHubIssueBreakdownRecord
    {
        /// <summary>
        /// The total number of open issues for an item.
        /// </summary>
        public int TotalIssues { get; set; }
        /// <summary>
        /// The number of open bug issues for an item.
        /// </summary>
        public int Bugs { get; set; }
        /// <summary>
        /// The number of open new feature issues for an item.
        /// </summary>
        public int NewFeatures { get; set; }
    }
}