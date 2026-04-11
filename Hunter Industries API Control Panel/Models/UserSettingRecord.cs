namespace HunterIndustriesAPIControlPanel.Models
{
    public class UserSettingRecord
    {
        public string Application { get; set; } = string.Empty;
        public List<SettingRecord> Settings { get; set; } = new();
    }
}
