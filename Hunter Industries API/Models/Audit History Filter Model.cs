// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models
{
    public class AuditHistoryFilterModel
    {
        public DateTime? FromDate { get; set; }
        public string? IPAddress { get; set; }
        public string? Endpoint { get; set; }
        public int PageSize { get; set; } = 25;
        public int PageNumber { get; set; } = 1;
    }
}
