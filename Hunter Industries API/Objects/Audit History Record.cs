// Copyright © - unpublished - Toby Hunter

namespace HunterIndustriesAPI.Objects
{
    public class AuditHistoryRecord
    {
        public int? Id { get; set; }
        public string? IPAddress { get; set; }
        public string? Endpoint { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
        public DateTime? OccuredAt { get; set; }
        public string[]? Paramaters { get; set; }
    }
}
