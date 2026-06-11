// Copyright © 11/06/2026 Toby Hunter
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
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Createx application entity link sql.
        /// </summary>
        public static string GetSQLCreateApplicationEntityLink(string application)
        {
            switch (application)
            {
                default: return "Unknown";
            }
        }
    }
}