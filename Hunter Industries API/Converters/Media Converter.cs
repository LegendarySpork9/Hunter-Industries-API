// Copyright © - 11/06/2026 - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class MediaConverter
    {
        /// <summary>
        /// Returns the Getx application entity sql.
        /// </summary>
        public static string GetSQLGetApplicationEntity(string application)
        {
            switch (application)
            {
                case "Portfolio": return @"join PortfolioItemImage PII with (nolock) on Media.MediaId = PII.MediaId
where PortfolioItemId = @entityId";
                default: return "NoApplicationEntity";
            }
        }

        /// <summary>
        /// Returns whether the application has a link table.
        /// </summary>
        public static bool HasApplicationEntityLink(string application)
        {
            switch (application)
            {
                case "Portfolio": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns the Createx application entity link sql.
        /// </summary>
        public static string GetSQLCreateApplicationEntityLink(string application)
        {
            switch (application)
            {
                case "Portfolio": return "CreatePortfolioItemImage.sql";
                default: return "NoApplicationEntityLink";
            }
        }
    }
}