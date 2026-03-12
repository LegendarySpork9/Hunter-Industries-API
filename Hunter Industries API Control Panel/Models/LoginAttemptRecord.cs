namespace Hunter_Industries_API_Control_Panel.Models
{
    public class LoginAttemptRecord
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Phrase { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
    }
}
