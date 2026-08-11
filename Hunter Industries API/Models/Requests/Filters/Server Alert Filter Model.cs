// Copyright © Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters
{
    /// <summary>
    /// </summary>
    public class ServerAlertFilterModel
    {
        /// <summary>
        /// The name of the server to filter by.
        /// </summary>
        public string ServerName { get; set; }
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