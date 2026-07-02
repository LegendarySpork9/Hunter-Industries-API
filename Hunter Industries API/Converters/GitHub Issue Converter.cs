// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters.Portfolio
{
    /// <summary>
    /// </summary>
    public static class GitHubIssueConverter
    {
        /// <summary>
        /// Returns the type in a friendly format.
        /// </summary>
        public static string GetIssueType(string type)
        {
            switch (type)
            {
                case "bug": return "Bug";
                case "new feature": return "New Feature";
                default: return type;
            };
        }
    }
}