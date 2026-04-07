namespace HunterIndustriesAPIControlPanel.Models
{
    public class ChangeRecord
    {
        public int Id { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
    }
}
