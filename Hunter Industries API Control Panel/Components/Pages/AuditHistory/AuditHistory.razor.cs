// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory
{
    public partial class AuditHistory
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        private PagedAPIResponseModel<AuditHistoryModel>? AuditLogs;

        private bool IsLoading;

        private DateTime? FilterFromDate;
        private DateTime? FilterToDate;
        private string FilterEndpoint = string.Empty;
        private string FilterIPAddress = string.Empty;
        private int PageSize = 25;
        private int PageNumber = 1;

        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Audit History Page");

            await LoadData();
        }

        /// <summary>
        /// Loads and transforms the audit data.
        /// </summary>
        private async Task LoadData()
        {
            IsLoading = true;

            string? fromDate = FilterFromDate?.ToString("dd/MM/yyyy");
            string? toDate = FilterToDate?.ToString("dd/MM/yyyy");

            (AuditLogs) = await APIService.GetAuditHistories(
                fromDate,
                toDate,
                FilterIPAddress,
                FilterEndpoint,
                null,
                null,
                PageSize,
                PageNumber);

            if (AuditLogs == null)
            {
                AuditLogs = new()
                {
                    Entries = [],
                    EntryCount = 0,
                    PageNumber = 1,
                    PageSize = 25,
                    TotalPageCount = 0,
                    TotalCount = 0
                };
            }

            IsLoading = false;
        }

        /// <summary>
        /// Applys the set filters.
        /// </summary>
        private async Task ApplyFilters()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Apply Clicked");

            PageNumber = 1;
            await LoadData();
        }

        /// <summary>
        /// Clears the applied filters.
        /// </summary>
        private async Task ClearFilters()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Clear Clicked");

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
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "<< Prev Clicked");

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
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Next >> Clicked");

            if (PageNumber < AuditLogs?.TotalPageCount)
            {
                PageNumber++;
                await LoadData();
            }
        }
    }
}
