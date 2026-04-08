// Copyright © - Unpublished - Toby Hunter
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
                "/users" => "?isDeleted=false",
                _ => string.Empty
            };
        }
    }
}
