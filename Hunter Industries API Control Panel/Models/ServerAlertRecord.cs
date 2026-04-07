namespace HunterIndustriesAPIControlPanel.Models
{
    public class ServerAlertRecord
    {
        public int AlertId { get; set; }
        public string Reporter { get; set; } = string.Empty;
        public string Component { get; set; } = string.Empty;
        public string ComponentStatus { get; set; } = string.Empty;
        public string AlertStatus { get; set; } = string.Empty;
        public DateTime AlertDate { get; set; }
        public RelatedServerRecord Server { get; set; } = new();
    }
}
