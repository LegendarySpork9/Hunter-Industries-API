namespace HunterIndustriesAPIControlPanel.Models
{
    public class ConfigurationMachineRecord
    {
        public int Id { get; set; }
        public string HostName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
