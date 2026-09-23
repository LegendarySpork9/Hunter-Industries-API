// Copyright © Unpublished - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the paged user api response with available scopes.
    /// </summary>
    public class PagedUserResponseModel
    {
        public required List<UserModel> Entries { get; set; }
        public required int EntryCount { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int TotalPageCount { get; set; }
        public required int TotalCount { get; set; }
        public List<string> AvailableScopes { get; set; } = [];
    }
}
