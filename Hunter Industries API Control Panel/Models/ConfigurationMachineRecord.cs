namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ConfigurationMachineRecord
    {
        public int Id { get; set; }
        public string HostName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
