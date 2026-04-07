namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ConfigurationApplicationSettingRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Required { get; set; }
        public bool IsDeleted { get; set; }
    }
}
