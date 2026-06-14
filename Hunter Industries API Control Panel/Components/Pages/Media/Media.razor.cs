// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Media
{
    public partial class Media
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "application")]
        public string? QueryApplication { get; set; }

        private PagedAPIResponseModel<MediaModel>? MediaRecords;

        private bool IsLoading;

        private string FilterApplication = string.Empty;
        private int PageSize = 25;
        private int PageNumber = 1;

        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Media Page");

            FilterApplication = QueryApplication ?? string.Empty;

            MediaRecords = new()
            {
                Entries = [],
                EntryCount = 0,
                PageNumber = 1,
                PageSize = 25,
                TotalPageCount = 0,
                TotalCount = 0
            };

            if (!string.IsNullOrWhiteSpace(QueryApplication))
            {
                MediaRecords = await APIService.GetMediaRecords(
                    QueryApplication,
                    PageSize,
                    PageNumber);
            }
        }

        /// <summary>
        /// Loads and transforms the media data.
        /// </summary>
        private async Task LoadData()
        {
            IsLoading = true;

            MediaRecords = await APIService.GetMediaRecords(
                FilterApplication,
                PageSize,
                PageNumber);

            if (MediaRecords == null)
            {
                MediaRecords = new()
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

            FilterApplication = string.Empty;
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

            if (PageNumber < MediaRecords?.TotalPageCount)
            {
                PageNumber++;
                await LoadData();
            }
        }
    }
}
