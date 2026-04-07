namespace Hunter_Industries_API_Control_Panel.Models
{
    public class UserSettingRecord
    {
        public string Application { get; set; } = string.Empty;
        public List<SettingRecord> Settings { get; set; } = new();
    }
}
