// Copyright © Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters
{
    /// <summary>
    /// </summary>
    public class ServerInformationFilterModel
    {
        /// <summary>
        /// Whether the servers are active or not.
        /// </summary>
        public bool IsActive { get; set; }
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
