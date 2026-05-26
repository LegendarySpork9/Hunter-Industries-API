// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Components.Shared;
using HunterIndustriesAPIControlPanel.Functions;
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
        private List<AuditHistoryModel>? RecentActivity;

        private bool IsLoading;

        private Dictionary<string, List<ChartDataPointModel>> ErrorsByIPGrouped = [];
        private Dictionary<string, List<ChartDataPointModel>> LoginAttemptsByApp = [];
        private HashSet<string> VisibleTrafficLabels = [];
        private const int TrafficLabelThreshold = 15;
        private const int TrafficLabelStep = 5;
        private Dictionary<string, string> ErrorColours = [];
        private Dictionary<string, string> LoginAttemptColours = [];
        private string[] ServerHealthColours = [];
        private string[] MethodColours = [];
        private string[] StatusColours = [];
        private string[] EndpointColours = [];
        private string[] FieldColours = [];

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Dashboard Page");

            IsLoading = true;

            Statistics = await APIService.GetDashboardStatistics();
            PagedAPIResponseModel<AuditHistoryModel>? auditLogs = await APIService.GetAuditHistories(pageSize: 10);

            if (Statistics != null)
            {
                int trafficStep = Statistics.ApiTraffic.Count > TrafficLabelThreshold ? TrafficLabelStep : 1;
                VisibleTrafficLabels = [.. Statistics.ApiTraffic.Where((_, i) => i % trafficStep == 0)
                    .Select(t => t.Day)];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Visible Traffic Label(s): {VisibleTrafficLabels.Count}");

                ServerHealthColours = [.. Statistics.ServerHealth.Select((_, e) => Colours.DefaultPalette[e % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Server Health Colour(s): {ServerHealthColours.Length}");

                List<string> ipAddresses = [.. Statistics.Errors.GroupBy(e => e.IpAddress)
                    .OrderByDescending(e => e.Sum(err => err.Errors))
                    .Select(e => e.Key)];
                ErrorsByIPGrouped = Statistics.Errors.GroupBy(e => ErrorFunction.ExtractClassMethod(e.Summary))
                    .ToDictionary(e => e.Key, e =>
                    {
                        Dictionary<string, int>? existing = e.GroupBy(err => err.IpAddress)
                            .ToDictionary(err => err.Key, err => err.Sum(ev => ev.Errors));

                        return ipAddresses.Select(ip => new ChartDataPointModel
                        {
                            Label = ip,
                            Value = existing.GetValueOrDefault(ip, 0)
                        }).ToList();
                    });

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Errors By IP: {ErrorsByIPGrouped.Count}");

                ErrorColours = ErrorsByIPGrouped.Keys.Select((key, e) => (key, colour: Colours.DefaultPalette[e % Colours.DefaultPalette.Length]))
                    .ToDictionary(err => err.key, err => err.colour);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Error Colour(s): {ErrorColours.Count}");

                MethodColours = [.. Statistics.MethodCalls.Select(m => m.Method switch
                {
                    "GET" => "#28a745",
                    "POST" => "#ffc107",
                    "PATCH" => "#fd7e14",
                    "DELETE" => "#dc3545",
                    _ => "#6c757d"
                })];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Method Colour(s): {MethodColours.Length}");

                StatusColours = [.. Statistics.StatusCalls.Select(s => s.Status switch 
                {
                    string status when status.StartsWith("200") => "#28a745",
                    string status when status.StartsWith("201") => "#17a2b8",
                    string status when status.StartsWith("400") => "#ffc107",
                    string status when status.StartsWith("401") => "#fd7e14",
                    string status when status.StartsWith("403") => "#e83e8c",
                    string status when status.StartsWith("404") => "#6c757d",
                    string status when status.StartsWith("500") => "#dc3545",
                    _ => "#6c757d"
                })];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Status Colour(s): {StatusColours.Length}");

                EndpointColours = [.. Statistics.EndpointCalls.Select((_, e) => Colours.DefaultPalette[e % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Endpoint Colour(s): {EndpointColours.Length}");

                FieldColours = [.. Statistics.Changes.Select((_, c) => Colours.DefaultPalette[c % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Field Colour(s): {FieldColours.Length}");

                string[] users = [.. Statistics.LoginAttempts.Select(u => u.Username)
                    .Distinct()
                    .OrderBy(u => u)];
                string[] applications = [.. Statistics.LoginAttempts.Select(a => a.Application)
                    .Distinct()
                    .OrderBy(a => a)];
                Dictionary<(string, string), int> loginLookup = Statistics.LoginAttempts.GroupBy(l => new { l.Application, l.Username })
                    .ToDictionary(ll => (ll.Key.Application, ll.Key.Username), ll => ll.Sum(login => login.TotalAttempts));
                LoginAttemptsByApp = applications.ToDictionary(app => app, app => users.Select(user => new ChartDataPointModel
                {
                    Label = user,
                    Value = loginLookup.GetValueOrDefault((app, user), 0)
                }).ToList());

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Login Attempts By App: {LoginAttemptsByApp.Count}");

                LoginAttemptColours = LoginAttemptsByApp.Keys.Select((key, l) => (key, colour: Colours.DefaultPalette[l % Colours.DefaultPalette.Length]))
                    .ToDictionary(la => la.key, la => la.colour);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Login Attempt Colour(s): {LoginAttemptColours.Count}");
            }

            if (auditLogs != null)
            {
                RecentActivity = auditLogs.Entries;
            }

            else
            {
                RecentActivity = [];
            }

            IsLoading = false;
        }

        /// <summary>
        /// Returns the trend based on two numbers.
        /// </summary>
        private static string GetTrend(
            int current,
            int previous)
        {
            string trend = string.Empty;

            if (previous == 0)
            {
                trend = current > 0 ? "+100%" : "+0%";
            }

            if (string.IsNullOrEmpty(trend))
            {
                double change = ((double)(current - previous) / previous) * 100;

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
        /// Returns the x ago value for the given time.
        /// </summary>
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
        /// Returns the traffic day label or blank if the axis is being thinned out.
        /// </summary>
        private string FormatTrafficDay(object value)
        {
            string label = string.Empty;

            if (value is string day && VisibleTrafficLabels.Contains(day))
            {
                label = day;
            }

            return label;
        }
    }
}
