// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the paged api response.
    /// </summary>
    public class PagedAPIResponseModel<T>
    {
        public required List<T> Entries { get; set; }
        public required int EntryCount { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int TotalPageCount { get; set; }
        public required int TotalCount { get; set; }
    }
}
