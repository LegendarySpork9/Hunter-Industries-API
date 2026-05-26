// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistoryDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        private AuditHistoryModel? AuditHistory;

        private bool IsLoading;

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Audit History Detail Page");

            IsLoading = true;

            AuditHistory = await APIService.GetAuditHistory(Id);

            IsLoading = false;
        }
    }
}
