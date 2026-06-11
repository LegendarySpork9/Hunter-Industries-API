// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters
{
    /// <summary>
    /// </summary>
    public class ConfigurationFilterModel
    {
        /// <summary>
        /// Whether to include used records.
        /// </summary>
        public bool IncludeUsed { get; set; } = true;
        /// <summary>
        /// The number of records to pull per page.
        /// </summary>
        public int PageSize { get; set; } = 25;
        /// <summary>
        /// The number of the page to pull.
        /// </summary>
        public int PageNumber { get; set; } = 1;
    }
}