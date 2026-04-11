namespace HunterIndustriesAPIControlPanel.Models
{
    public class DashboardSummary
    {
        public int ApplicationCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int CallsThisMonth { get; set; }
        public int CallsLastMonth { get; set; }
        public int LoginAttemptsThisMonth { get; set; }
        public int LoginAttemptsLastMonth { get; set; }
        public int ChangesThisMonth { get; set; }
        public int ChangesLastMonth { get; set; }
        public int ErrorsThisMonth { get; set; }
        public int ErrorsLastMonth { get; set; }
        public List<TrafficDataPoint> DailyTraffic { get; set; } = new();
        public List<ChartDataItem> ErrorsByIPAndSummary { get; set; } = new();
        public List<ChartDataItem> CallsByEndpoint { get; set; } = new();
        public List<ChartDataItem> CallsByMethod { get; set; } = new();
        public List<ChartDataItem> CallsByStatusCode { get; set; } = new();
        public List<ChartDataItem> ChangesByField { get; set; } = new();
        public List<ChartDataItem> LoginAttemptsByUser { get; set; } = new();
        public List<ServerHealthDataPoint> ServerHealth { get; set; } = new();
    }

    public class ChartDataItem
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class TrafficDataPoint
    {
        public DateTime Date { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    public class ServerHealthDataPoint
    {
        public string ServerName { get; set; } = string.Empty;
        public double UptimePercent { get; set; }
        public int AlertCount { get; set; }
        public int EventCount { get; set; }
    }
}
