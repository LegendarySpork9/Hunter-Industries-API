namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ConfigurationGameRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
