// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters.Portfolio
{
    /// <summary>
    /// </summary>
    public static class MetricConverter
    {
        /// <summary>
        /// Returns the sql xUpdated SQL for the given metric.
        /// </summary>
        public static string GetUpdateSQL(string metric)
        {
            switch (metric)
            {
                case "summary": return "SummaryViewsUpdated.sql";
                case "full": return "FullDetailViewsUpdated.sql";
                default: return "Unknown.sql";
            };
        }

        /// <sumamry>
        /// Returns the friendly name for the given metric.
        /// </sumamry>
        public static string GetMetricName(string metric)
        {
            switch (metric)
            {
                case "summary": return "Summary Views";
                case "full": return "Full Detail Views";
                default: return metric;
            };
        }
    }
}