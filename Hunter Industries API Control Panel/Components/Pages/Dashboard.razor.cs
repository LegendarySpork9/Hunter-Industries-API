// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Dashboard
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IClock _Clock { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        private DashboardStatisticsModel? Statistics;
        private List<AuditHistoryModel> RecentActivity = [];

        private Dictionary<string, List<ChartDataItem>> ErrorsByIPGrouped = [];
        private Dictionary<string, List<ChartDataItem>> LoginAttemptsByApp = [];
        private Dictionary<string, string> ErrorColours = [];
        private Dictionary<string, string> LoginAttemptColours = [];
        private string[] ServerHealthColours = [];
        private string[] MethodColours = [];
        private string[] StatusColours = [];
        private string[] EndpointColours = [];
        private string[] FieldColours = [];

        private static readonly string[] DefaultPalette =
        [
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        ];

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            Statistics = await APIService.GetDashboardStatistics();
            RecentActivity = await APIService.GetAuditHistories(pageSize: 10);

            if (Statistics != null)
            {
                ServerHealthColours = [.. Statistics.ServerHealth.Select((_, e) => DefaultPalette[e % DefaultPalette.Length])];

                List<string> ipAddresses = [.. Statistics.Errors.GroupBy(e => e.IpAddress).OrderByDescending(e => e.Sum(err => err.Errors)).Select(e => e.Key)];
                ErrorsByIPGrouped = Statistics.Errors.GroupBy(e => ExtractClassMethod(e.Summary)).ToDictionary(e => e.Key, e =>
                {
                    Dictionary<string, int>? existing = e.GroupBy(err => err.IpAddress).ToDictionary(err => err.Key, err => err.Sum(ev => ev.Errors));

                    return ipAddresses.Select(ip => new ChartDataItem
                    {
                        Label = ip,
                        Value = existing.GetValueOrDefault(ip, 0)
                    }).ToList();
                });

                ErrorColours = ErrorsByIPGrouped.Keys.Select((key, e) => (key, colour: DefaultPalette[e % DefaultPalette.Length])).ToDictionary(err => err.key, err => err.colour);

                MethodColours = [.. Statistics.MethodCalls.Select(m => m.Method switch
                {
                    "GET" => "#28a745",
                    "POST" => "#ffc107",
                    "PATCH" => "#fd7e14",
                    "DELETE" => "#dc3545",
                    _ => "#6c757d"
                })];

                StatusColours = [.. Statistics.StatusCalls.Select(s => s.Status switch {
                    string status when status.StartsWith("200") => "#28a745",
                    string status when status.StartsWith("201") => "#17a2b8",
                    string status when status.StartsWith("400") => "#ffc107",
                    string status when status.StartsWith("401") => "#fd7e14",
                    string status when status.StartsWith("403") => "#e83e8c",
                    string status when status.StartsWith("404") => "#6c757d",
                    string status when status.StartsWith("500") => "#dc3545",
                    _ => "#6c757d"
                })];

                EndpointColours = [.. Statistics.EndpointCalls.Select((_, e) => DefaultPalette[e % DefaultPalette.Length])];

                FieldColours = [.. Statistics.Changes.Select((_, c) => DefaultPalette[c % DefaultPalette.Length])];

                string[] users = [.. Statistics.LoginAttempts.Select(u => u.Username).Distinct().OrderBy(u => u)];
                string[] applications = [.. Statistics.LoginAttempts.Select(a => a.Application).Distinct().OrderBy(a => a)];
                Dictionary<(string, string), int> loginLookup = Statistics.LoginAttempts.GroupBy(l => new { l.Application, l.Username }).ToDictionary(ll => (ll.Key.Application, ll.Key.Username), ll => ll.Sum(login => login.TotalAttempts));
                LoginAttemptsByApp = applications.ToDictionary(app => app, app => users.Select(user => new ChartDataItem
                {
                    Label = user,
                    Value = loginLookup.GetValueOrDefault((app, user), 0)
                }).ToList());

                LoginAttemptColours = LoginAttemptsByApp.Keys.Select((key, l) => (key, colour: DefaultPalette[l % DefaultPalette.Length])).ToDictionary(la => la.key, la => la.colour);
            }
        }

        /// <summary>
        /// Returns the trend based on two numbers.
        /// </summary>
        private static string GetTrend(int current, int previous)
        {
            string trend = string.Empty;

            if (previous == 0 && current > 0)
            {
                trend = "+100%";
            }

            if (string.IsNullOrEmpty(trend))
            {
                double change = ((current - previous) / previous) * 100;

                if (change >= 0)
                {
                    trend = $"+{change:F0}%";
                }

                else
                {
                    trend = $"{change:F0}%";
                }
            }

            return trend;
        }

        /// <summary>
        /// Returns the badge class for the given method.
        /// </summary>
        private static string GetMethodBadgeClass(string method)
        {
            return method switch
            {
                "GET" => "badge-method-get",
                "POST" => "badge-method-post",
                "PATCH" => "badge-method-patch",
                "DELETE" => "badge-method-delete",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Returns the badge class for the given status.
        /// </summary>
        private static string GetStatusBadgeClass(string status)
        {
            string className = "bg-secondary";

            if (status.StartsWith("200"))
            {
                className = "badge-status-200";
            }

            else if (status.StartsWith("201"))
            {
                className = "badge-status-201";
            }

            else if (status.StartsWith("400"))
            {
                className = "badge-status-400";
            }

            else if (status.StartsWith("401"))
            {
                className = "badge-status-401";
            }

            else if (status.StartsWith("403"))
            {
                className = "badge-status-403";
            }

            else if (status.StartsWith("404"))
            {
                className = "badge-status-404";
            }

            else if (status.StartsWith("500"))
            {
                className = "badge-status-500";
            }

            return className;
        }

        private string GetRelativeTime(DateTime dateTime)
        {
            string relativeTime = dateTime.ToString("dd MMM yyyy");

            TimeSpan span = _Clock.UtcNow - dateTime;

            if (span.TotalMinutes < 1)
            {
                relativeTime = "Just now";
            }

            else if (span.TotalMinutes < 60)
            {
                relativeTime = $"{(int)span.TotalMinutes}m ago";
            }

            else if (span.TotalHours < 24)
            {
                relativeTime = $"{(int)span.TotalHours}h ago";
            }

            else if (span.TotalDays < 30)
            {
                relativeTime = $"{(int)span.TotalDays}d ago";
            }

            return relativeTime;
        }

        /// <summary>
        /// Returns the class name from the given string.
        /// </summary>
        private static string ExtractClassMethod(string summary)
        {
            string className = string.Empty;

            string[] words = summary.TrimEnd('.').Split(' ');

            for (int i = words.Length - 1; i >= 0; i--)
            {
                if (words[i].Contains('.'))
                {
                    string classMethod = words[i].TrimEnd('.');
                    int dotIndex = classMethod.LastIndexOf('.');

                    if (dotIndex >= 0)
                    {
                        className = classMethod[(dotIndex + 1)..];
                    }

                    else
                    {
                        className = classMethod;
                    }
                }
            }

            if (string.IsNullOrEmpty(className))
            {
                if (summary.Length > 40)
                {
                    className = $"{summary[..40]}...";
                }

                else
                {
                    className = summary;
                }
            }

            return className;
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
