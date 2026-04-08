using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistoryDetail
    {
        [Inject]
        private ExampleAPIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromQuery(Name = "fromPage")]
        public int? FromAuditPage { get; set; }

        private int FromPage = 1;

        private AuditHistoryRecord? Record;

        protected override void OnInitialized()
        {
            FromPage = FromAuditPage.HasValue && FromAuditPage.Value > 0 ? FromAuditPage.Value : 1;
            Record = APIService.GetAuditHistoryRecord(Id);
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
