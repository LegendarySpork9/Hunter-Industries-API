using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistory
    {
        [Inject]
        private ExampleAPIService APIService { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

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

            else if (status.StartsWith("500"))
            {
                className = "badge-status-500";
            }

            return className;
        }
    }
}
