// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models;
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
        private APIService APIService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "user")]
        public int? QueryUserId { get; set; }
        [SupplyParameterFromQuery(Name = "application")]
        public int? QueryApplicationId { get; set; }

        private int? UserId;
        private int? ApplicationId;
        private string PageTitle = "Logs";

        private SharedStatisticsModel? Statistics;
        private List<AuditHistoryModel> AuditLogs = [];

        private DateTime? _FilterFromDate;
        private DateTime? _FilterToDate;
        private string _FilterEndpoint = string.Empty;
        private string _FilterIPAddress = string.Empty;
        private string _FilterMethod = "All";
        private string _FilterStatus = "All";
        private int _PageSize = 25;
        private int _PageNumber = 1;

        private string[] MethodColours = [];
        private string[] StatusColours = [];
        private string[] EndpointColours = [];
        private string[] FieldColours = [];

        private static readonly string[] DefaultPalette =
        [
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        ];



        [Inject] private ExampleAPIService ExampleAPIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private int? _userId;
        private int? _applicationId;
        private string _pageTitle = "Logs";
        private DashboardSummary? _summary;
        private PaginatedResponse<AuditHistoryRecord> _results = new();

        private DateTime? _filterFromDate;
        private DateTime? _filterToDate;
        private string _filterEndpoint = string.Empty;
        private string _filterIPAddress = string.Empty;
        private string _filterMethod = "All";
        private string _filterStatus = "All";
        private int _pageSize = 25;
        private int _pageNumber = 1;

        private string[] _methodColours = Array.Empty<string>();
        private string[] _statusColours = Array.Empty<string>();
        private string[] _endpointColours = Array.Empty<string>();
        private string[] _fieldColours = Array.Empty<string>();

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override void OnInitialized()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Logs Page");

            UserId = QueryUserId;
            ApplicationId = QueryApplicationId;

            if (UserId.HasValue)
            {
                var user = ExampleAPIService.GetUser(_userId.Value);
                _pageTitle = user != null ? $"Logs - {user.Username}" : "Logs - Unknown User";
            }

            else if (ApplicationId.HasValue)
            {
                var app = ExampleAPIService.GetConfigurationApplication(_applicationId.Value);
                _pageTitle = app != null ? $"Logs - {app.Name}" : "Logs - Unknown Application";
            }

            LoadSummary();
            LoadData();
        }

        private void LoadSummary()
        {
            _summary = ExampleAPIService.GetLogsSummary(_userId, _applicationId);

            _methodColours = _summary.CallsByMethod.Select(m => m.Label switch
            {
                "GET" => "#28a745",
                "POST" => "#ffc107",
                "PATCH" => "#fd7e14",
                "DELETE" => "#dc3545",
                _ => "#6c757d"
            }).ToArray();

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

            _endpointColours = _summary.CallsByEndpoint
                .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                .ToArray();

            _fieldColours = _summary.ChangesByField
                .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                .ToArray();
        }

        private void LoadData()
        {
            _results = ExampleAPIService.GetAuditHistory(
                _filterFromDate, _filterToDate,
                string.IsNullOrWhiteSpace(_filterEndpoint) ? null : _filterEndpoint,
                string.IsNullOrWhiteSpace(_filterIPAddress) ? null : _filterIPAddress,
                _filterMethod, _filterStatus,
                _pageSize, _pageNumber,
                _userId, _applicationId);
        }

        private void ApplyFilters()
        {
            _pageNumber = 1;
            LoadData();
        }

        private void ClearFilters()
        {
            _filterFromDate = null;
            _filterToDate = null;
            _filterEndpoint = string.Empty;
            _filterIPAddress = string.Empty;
            _filterMethod = "All";
            _filterStatus = "All";
            _pageNumber = 1;
            LoadData();
        }

        private void PreviousPage()
        {
            if (_pageNumber > 1)
            {
                _pageNumber--;
                LoadData();
            }
        }

        private void NextPage()
        {
            if (_pageNumber < _results.TotalPageCount)
            {
                _pageNumber++;
                LoadData();
            }
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
            if (status.StartsWith("500")) return "badge-status-500";
            return "bg-secondary";
        }
    }
}
