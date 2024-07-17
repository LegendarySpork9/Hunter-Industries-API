// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Objects;

namespace HunterIndustriesAPI.Models.Responses
{
    public class AuditHistoryResponseModel
    {
        public List<AuditHistoryRecord>? Entries { get; set; }
        public int? EntryCount { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPageCount { get; set; }
        public int? TotalCount { get; set; }
    }
}
