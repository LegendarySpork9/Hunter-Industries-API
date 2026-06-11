// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPIControlPanel.Converters
{
    public static class APIConverter
    {
        /// <summary>
        /// Returns the query parameters for a given endpoint.
        /// </summary>
        public static string GetQuery(string endpoint)
        {
            return endpoint switch
            {
                "/user" => "?includeDeleted=true",
                "/configuration/authorisation" => "?includeUsed=false",
                _ => string.Empty
            };
        }
    }
}
