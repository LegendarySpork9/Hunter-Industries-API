namespace HunterIndustriesAPIControlPanel.Models
{
    public class UserModel
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Token);
    }
}
