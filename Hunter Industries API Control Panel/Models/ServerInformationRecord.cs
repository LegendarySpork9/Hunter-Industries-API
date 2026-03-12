namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ServerInformationRecord
    {
        public int Id { get; set; }
        public string HostName { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public string GameVersion { get; set; } = string.Empty;
        public ConnectionRecord Connection { get; set; } = new();
        public DowntimeRecord? Downtime { get; set; }
        public bool IsActive { get; set; }
    }
}
