namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ConfigurationAuthorisationRecord
    {
        public int Id { get; set; }
        public string Phrase { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
