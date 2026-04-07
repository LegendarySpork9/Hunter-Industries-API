namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ConfigurationDowntimeRecord
    {
        public int Id { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Duration { get; set; }
        public bool IsDeleted { get; set; }
    }
}
