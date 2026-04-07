namespace Hunter_Industries_API_Control_Panel.Models
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
