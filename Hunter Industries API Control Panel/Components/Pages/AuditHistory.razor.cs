using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class AuditHistory
    {
        [Inject] private APIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "page")]
        public int? QueryPage { get; set; }

        private PaginatedResponse<AuditHistoryRecord> _results = new();
        private DateTime? _filterFromDate;
        private DateTime? _filterToDate;
        private string _filterEndpoint = string.Empty;
        private string _filterIPAddress = string.Empty;
        private string _filterMethod = "All";
        private string _filterStatus = "All";
        private int _pageSize = 25;
        private int _pageNumber = 1;

        protected override void OnInitialized()
        {
            if (QueryPage.HasValue && QueryPage.Value > 0)
            {
                _pageNumber = QueryPage.Value;
            }

            LoadData();
        }

        private void LoadData()
        {
            _results = APIService.GetAuditHistory(
                _filterFromDate, _filterToDate,
                string.IsNullOrWhiteSpace(_filterEndpoint) ? null : _filterEndpoint,
                string.IsNullOrWhiteSpace(_filterIPAddress) ? null : _filterIPAddress,
                _filterMethod, _filterStatus,
                _pageSize, _pageNumber);

            UpdateUrl();
        }

        private void UpdateUrl()
        {
            Navigation.NavigateTo($"/audit-history?page={_pageNumber}", replace: true);
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
