// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Models.Requests
{
    public class AuditHistoryFilterModel
    {
        public string FromDate { get; set; } = "01/01/1900";
        public string? IPAddress { get; set; }
        public string? Endpoint { get; set; }
        public int PageSize { get; set; } = 25;
        public int PageNumber { get; set; } = 1;
    }
}
