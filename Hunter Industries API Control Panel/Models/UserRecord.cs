namespace Hunter_Industries_API_Control_Panel.Models
{
    public class UserRecord
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Scopes { get; set; } = new();
        public bool IsDeleted { get; set; }
    }
}
