// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the audit history api response.
    /// </summary>
    public class AuditHistoryModel
    {
        public required int Id { get; set; }
        public required string IpAddress { get; set; }
        public string? Username { get; set; }
        public string? Application { get; set; }
        public required string Endpoint { get; set; }
        public required string EndpointVersion { get; set; }
        public required string Method { get; set; }
        public required string Status { get; set; }
        public required DateTime OccuredAt { get; set; }
        public required string[] Parameters { get; set; }
        public LoginAttemptModel? LoginAttempt { get; set; }
        public List<ChangeModel>? Change { get; set; }
    }
}
