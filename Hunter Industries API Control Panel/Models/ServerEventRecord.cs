namespace HunterIndustriesAPIControlPanel.Models
{
    public class ServerEventRecord
    {
        public string Component { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateOccured { get; set; }
        public RelatedServerRecord Server { get; set; } = new();
    }
}
