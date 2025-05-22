using HunterIndustriesAPI.Objects.ServerStatus;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Models.Responses.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerAlertResponseModel
    {
        /// <summary>
        /// The server alert records.
        /// </summary>
        public List<ServerAlertRecord> Entries { get; set; }
        /// <summary>
        /// The number of records in the page.
        /// </summary>
        public int EntryCount { get; set; }
        /// <summary>
        /// The current page number.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// The current page size.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// The total number of pages returned.
        /// </summary>
        public int TotalPageCount { get; set; }
        /// <summary>
        /// The total number of records returned.
        /// </summary>
        public int TotalCount { get; set; }
    }
}