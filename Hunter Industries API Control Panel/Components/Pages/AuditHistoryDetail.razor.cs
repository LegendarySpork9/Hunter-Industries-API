using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class AuditHistoryDetail
    {
        [Parameter] public int Id { get; set; }
        [SupplyParameterFromQuery(Name = "fromPage")]
        public int? FromPage { get; set; }
        [Inject] private APIService APIService { get; set; } = default!;

        private int _fromPage = 1;

        private AuditHistoryRecord? _record;

        protected override void OnInitialized()
        {
            _fromPage = FromPage.HasValue && FromPage.Value > 0 ? FromPage.Value : 1;
            _record = APIService.GetAuditHistoryRecord(Id);
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
