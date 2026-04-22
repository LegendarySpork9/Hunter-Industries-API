// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Logs
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "user")]
        public int? QueryUserId { get; set; }
        [SupplyParameterFromQuery(Name = "application")]
        public int? QueryApplicationId { get; set; }

        private UserModel? User;
        private ApplicationModel? Application;
        private string PageTitle = "Logs";
        private string Entity = string.Empty;
        private int EntityId = 0;

        private SharedStatisticsModel? Statistics;
        private PagedAPIResponseModel<AuditHistoryModel>? AuditLogs;
        private int TotalPageCount = 0;

        private DateTime? FilterFromDate;
        private DateTime? FilterToDate;
        private string FilterEndpoint = string.Empty;
        private string FilterIPAddress = string.Empty;
        private int PageSize = 25;
        private int PageNumber = 1;

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
        /// Sets the page title.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Logs Page");

            if (QueryUserId.HasValue)
            {
                UserModel? user = await APIService.GetUser(QueryUserId.Value);

                if (user != null)
                {
                    User = user;
                }

                PageTitle = user != null ? $"Logs - {user.Username}" : "Logs - Unknown User";
                Entity = "user";
                EntityId = QueryUserId.Value;
            }

            else if (QueryApplicationId.HasValue)
            {
                ApplicationModel? application = await APIService.GetApplication(QueryApplicationId.Value);

                if (application != null)
                {
                    Application = application;
                }

                PageTitle = application != null ? $"Logs - {application.Name}" : "Logs - Unknown User";
                Entity = "application";
                EntityId = QueryApplicationId.Value;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Page Title: {PageTitle}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Entity: {Entity}");
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Entity Id: {EntityId}");

            await LoadSummary();
            await LoadData();
        }

        /// <summary>
        /// Loads and transforms the summary data.
        /// </summary>
        private async Task LoadSummary()
        {
            Statistics = await APIService.GetLogStatistics(Entity, EntityId);

            if (Statistics != null)
            {
                MethodColours = [.. Statistics.MethodCalls.Select(m => m.Method switch
                {
                    "GET" => "#28a745",
                    "POST" => "#ffc107",
                    "PATCH" => "#fd7e14",
                    "DELETE" => "#dc3545",
                    _ => "#6c757d"
                })];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Method Colour(s): {MethodColours.Length}");

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

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Status Colour(s): {StatusColours.Length}");

                EndpointColours = [.. Statistics.EndpointCalls.Select((_, e) => DefaultPalette[e % DefaultPalette.Length])];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Endpoint Colour(s): {EndpointColours.Length}");

                FieldColours = [.. Statistics.Changes.Select((_, c) => DefaultPalette[c % DefaultPalette.Length])];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Field Colour(s): {FieldColours.Length}");
            }
        }

        /// <summary>
        /// Loads and transforms the audit data.
        /// </summary>
        private async Task LoadData()
        {
            string? fromDate = FilterFromDate?.ToString("dd/MM/yyyy");
            string? toDate = FilterToDate?.ToString("dd/MM/yyyy");
            string? username = User?.Username ?? null;
            string? application = Application?.Name ?? null;

            AuditLogs = await APIService.GetAuditHistories(fromDate, toDate, FilterIPAddress, FilterEndpoint, username, application, PageSize, PageNumber);

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Audit Entries: {AuditLogs?.EntryCount ?? 0}");
        }

        /// <summary>
        /// Applys the set filters.
        /// </summary>
        private async Task ApplyFilters()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Apply Clicked");

            PageNumber = 1;
            await LoadData();
        }

        /// <summary>
        /// Clears the applied filters.
        /// </summary>
        private async Task ClearFilters()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Clear Clicked");

            FilterFromDate = null;
            FilterToDate = null;
            FilterEndpoint = string.Empty;
            FilterIPAddress = string.Empty;
            PageNumber = 1;
            await LoadData();
        }

        /// <summary>
        /// Loads the last page of audit logs.
        /// </summary>
        private async Task PreviousPage()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "<< Prev Clicked");

            if (PageNumber > 1)
            {
                PageNumber--;
                await LoadData();
            }
        }

        /// <summary>
        /// Loads the next page of audit logs.
        /// </summary>
        private async Task NextPage()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Next >> Clicked");

            if (PageNumber < TotalPageCount)
            {
                PageNumber++;
                await LoadData();
            }
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
    }
}
