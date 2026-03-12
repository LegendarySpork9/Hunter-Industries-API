namespace Hunter_Industries_API_Control_Panel.Models
{
    public class ErrorLogRecord
    {
        public int Id { get; set; }
        public DateTime DateOccured { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
