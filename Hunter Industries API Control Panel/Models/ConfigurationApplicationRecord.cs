namespace HunterIndustriesAPIControlPanel.Models
{
    public class ConfigurationApplicationRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phrase { get; set; } = string.Empty;
        public List<ConfigurationApplicationSettingRecord> Settings { get; set; } = new();
        public bool IsDeleted { get; set; }
    }
}
