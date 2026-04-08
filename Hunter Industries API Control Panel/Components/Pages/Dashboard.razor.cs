using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Dashboard
    {
        [Inject] private ExampleAPIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private DashboardSummary? _summary;
        private List<AuditHistoryRecord> _recentActivity = new();
        private Dictionary<string, List<ChartDataItem>> _errorsByIPGrouped = new();
        private Dictionary<string, List<ChartDataItem>> _loginAttemptsByApp = new();
        private string[] _methodColours = Array.Empty<string>();
        private string[] _statusColours = Array.Empty<string>();
        private string[] _endpointColours = Array.Empty<string>();
        private string[] _fieldColours = Array.Empty<string>();

        private static readonly string[] DefaultPalette = new[]
        {
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        };

        protected override void OnInitialized()
        {
            _summary = APIService.GetDashboardSummary();
            _recentActivity = APIService.GetRecentAuditHistory(10);

            // Build errors by IP - ensure every series has an entry for every IP, sorted by most errors
            var allIPs = _summary.ErrorsByIPAndSummary
                .GroupBy(e => e.Label)
                .OrderByDescending(g => g.Sum(x => x.Value))
                .Select(g => g.Key)
                .ToList();
            _errorsByIPGrouped = _summary.ErrorsByIPAndSummary
                .GroupBy(e => ExtractClassMethod(e.Category))
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var existing = g.GroupBy(x => x.Label).ToDictionary(x => x.Key, x => x.Sum(v => v.Value));
                        return allIPs.Select(ip => new ChartDataItem
                        {
                            Label = ip,
                            Value = existing.GetValueOrDefault(ip, 0)
                        }).ToList();
                    });

            // Build method donut colours (order must match _summary.CallsByMethod)
            _methodColours = _summary.CallsByMethod.Select(m => m.Label switch
            {
                "GET" => "#28a745",
                "POST" => "#ffc107",
                "PATCH" => "#fd7e14",
                "DELETE" => "#dc3545",
                _ => "#6c757d"
            }).ToArray();

            // Build status code donut colours (order must match _summary.CallsByStatusCode)
            _statusColours = _summary.CallsByStatusCode.Select(s => s.Label switch
            {
                var l when l.StartsWith("200") => "#28a745",
                var l when l.StartsWith("201") => "#17a2b8",
                var l when l.StartsWith("400") => "#ffc107",
                var l when l.StartsWith("401") => "#fd7e14",
                var l when l.StartsWith("403") => "#e83e8c",
                var l when l.StartsWith("404") => "#6c757d",
                var l when l.StartsWith("500") => "#dc3545",
                _ => "#6c757d"
            }).ToArray();

            // Build endpoint donut colours
            _endpointColours = _summary.CallsByEndpoint
                .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                .ToArray();

            // Build field donut colours
            _fieldColours = _summary.ChangesByField
                .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                .ToArray();

            // Build login attempts - ensure every series has an entry for every user (0 if missing)
            var allUsers = _summary.LoginAttemptsByUser.Select(l => l.Label).Distinct().OrderBy(u => u).ToList();
            var allApps = _summary.LoginAttemptsByUser.Select(l => l.Category).Distinct().OrderBy(a => a).ToList();
            var loginLookup = _summary.LoginAttemptsByUser
                .GroupBy(l => new { l.Category, l.Label })
                .ToDictionary(g => (g.Key.Category, g.Key.Label), g => g.Sum(x => x.Value));
            _loginAttemptsByApp = allApps.ToDictionary(
                app => app,
                app => allUsers.Select(user => new ChartDataItem
                {
                    Label = user,
                    Value = loginLookup.GetValueOrDefault((app, user), 0)
                }).ToList());
        }

        private string GetTrend(int current, int previous)
        {
            if (previous == 0) return current > 0 ? "+100%" : "";
            var change = ((double)(current - previous) / previous) * 100;
            return change >= 0 ? $"+{change:F0}%" : $"{change:F0}%";
        }

        private string GetMethodBadgeClass(string method) => method switch
        {
            "GET" => "badge-method-get",
            "POST" => "badge-method-post",
            "PATCH" => "badge-method-patch",
            "DELETE" => "badge-method-delete",
            _ => "bg-secondary"
        };

        private string GetStatusBadgeClass(string status)
        {
            if (status.StartsWith("200")) return "badge-status-200";
            if (status.StartsWith("201")) return "badge-status-201";
            if (status.StartsWith("400")) return "badge-status-400";
            if (status.StartsWith("401")) return "badge-status-401";
            if (status.StartsWith("403")) return "badge-status-403";
            if (status.StartsWith("404")) return "badge-status-404";
            if (status.StartsWith("500")) return "badge-status-500";
            return "bg-secondary";
        }

        private string GetRelativeTime(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 30) return $"{(int)span.TotalDays}d ago";
            return dateTime.ToString("dd MMM yyyy");
        }

        private static string ExtractClassMethod(string summary)
        {
            var words = summary.TrimEnd('.').Split(' ');

            for (int i = words.Length - 1; i >= 0; i--)
            {
                if (words[i].Contains('.'))
                {
                    var classMethod = words[i].TrimEnd('.');
                    var dotIndex = classMethod.LastIndexOf('.');

                    if (dotIndex >= 0)
                    {
                        return classMethod[(dotIndex + 1)..];
                    }

                    return classMethod;
                }
            }

            return summary.Length > 40 ? summary[..40] + "..." : summary;
        }
    }
}
