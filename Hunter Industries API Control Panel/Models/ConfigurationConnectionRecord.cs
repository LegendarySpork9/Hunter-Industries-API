namespace HunterIndustriesAPIControlPanel.Models
{
    public class ConfigurationConnectionRecord
    {
        public int Id { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsDeleted { get; set; }
    }
}
