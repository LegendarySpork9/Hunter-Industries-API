namespace Hunter_Industries_API_Control_Panel.Models
{
    public class AuditHistoryRecord
    {
        public int Id { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OccuredAt { get; set; }
        public string[]? Paramaters { get; set; }
        public LoginAttemptRecord? LoginAttempt { get; set; }
        public List<ChangeRecord>? Change { get; set; }
    }
}
