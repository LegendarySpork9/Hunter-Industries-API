namespace HunterIndustriesAPIControlPanel.Models
{
    public class ConfigurationGameRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
