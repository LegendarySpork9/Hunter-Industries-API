using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistory
    {
        [Inject] private APIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "page")]
        public int? QueryPage { get; set; }

        private PaginatedResponse<AuditHistoryRecord> Results = new();
        private DateTime? FilterFromDate;
        private DateTime? FilterToDate;
        private string FilterEndpoint = string.Empty;
        private string FilterIPAddress = string.Empty;
        private string FilterMethod = string.Empty;
        private string FilterStatus = string.Empty;
        private int PageSize = 25;
        private int PageNumber = 1;

        protected override void OnInitialized()
        {
            if (QueryPage.HasValue && QueryPage.Value > 0)
            {
                PageNumber = QueryPage.Value;
            }

            LoadData();
        }

        private void LoadData()
        {
            Results = APIService.GetAuditHistory(
                FilterFromDate, FilterToDate,
                string.IsNullOrWhiteSpace(FilterEndpoint) ? null : FilterEndpoint,
                string.IsNullOrWhiteSpace(FilterIPAddress) ? null : FilterIPAddress,
                FilterMethod, FilterStatus,
                PageSize, PageNumber);

            UpdateUrl();
        }

        private void UpdateUrl()
        {
            Navigation.NavigateTo($"/audit-history?page={PageNumber}", replace: true);
        }

        private void ApplyFilters()
        {
            PageNumber = 1;
            LoadData();
        }

        private void ClearFilters()
        {
            FilterFromDate = null;
            FilterToDate = null;
            FilterEndpoint = string.Empty;
            FilterIPAddress = string.Empty;
            FilterMethod = string.Empty;
            FilterStatus = string.Empty;
            PageNumber = 1;
            LoadData();
        }

        private void PreviousPage()
        {
            if (PageNumber > 1)
            {
                PageNumber--;
                LoadData();
            }
        }

        private void NextPage()
        {
            if (PageNumber < Results.TotalPageCount)
            {
                PageNumber++;
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
