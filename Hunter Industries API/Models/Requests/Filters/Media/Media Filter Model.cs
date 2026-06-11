// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters.Media
{
    /// <summary>
    /// </summary>
    public class MediaFilterModel
    {
        /// <summary>
        /// Whether to return deleted media.
        /// </summary>
        public bool IncludeDeleted { get; set; }
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