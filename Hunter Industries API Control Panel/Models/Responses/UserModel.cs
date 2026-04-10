namespace HunterIndustriesAPIControlPanel.Models.Responses
{
    /// <summary>
    /// Stores the user api response.
    /// </summary>
    public class UserModel
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required List<string> Scopes { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
