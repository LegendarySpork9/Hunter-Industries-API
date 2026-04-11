// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests.Filters
{
    /// <summary>
    /// </summary>
    public class ErrorLogFilterModel
    {
        /// <summary>
        /// The date from which to pull records.
        /// </summary>
        public string FromDate { get; set; } = "01/01/1900";
        /// <summary>
        /// The IP address the records were made from.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The summary of the records.
        /// </summary>
        public string Summary { get; set; }
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