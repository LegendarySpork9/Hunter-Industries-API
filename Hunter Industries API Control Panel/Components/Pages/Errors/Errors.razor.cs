// Copyright © 11/06/2026 Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Components.Shared;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Errors
{
    public partial class Errors
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IClock _Clock { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        private ErrorStatisticsModel? Statistics;
        private PagedAPIResponseModel<ErrorModel>? ErrorLog;

        private bool IsLoading;

        private DateTime? FilterFromDate;
        private DateTime? FilterToDate;
        private string FilterIPAddress = string.Empty;
        private string FilterSummary = string.Empty;
        private int PageSize = 25;
        private int PageNumber = 1;

        private string[] IPErrorColours = [];
        private string[] SummaryErrorColours = [];
        private string ErrorYearRange = string.Empty;

        /// <summary>
        /// Loads and transforms the summary data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Errors Page");

            IsLoading = true;

            Statistics = await APIService.GetErrorStatistics();

            if (Statistics != null)
            {
                IPErrorColours = [.. Statistics.IPErrors.Select((_, e) => Colours.DefaultPalette[e % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Ip Error Colour(s): {IPErrorColours.Length}");

                SummaryErrorColours = [.. Statistics.SummaryErrors.Select((_, e) => Colours.DefaultPalette[e % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Summary Error Colour(s): {SummaryErrorColours.Length}");

                DateTime now = _Clock.UtcNow;
                int startyear = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11).Year;
                int endYear = now.Year;

                ErrorYearRange = startyear == endYear ? $"{startyear}" : $"{startyear} → {endYear}";

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Error Year Range: {ErrorYearRange}");
            }

            await LoadData();
        }

        /// <summary>
        /// Loads and transforms the error data.
        /// </summary>
        private async Task LoadData()
        {
            IsLoading = true;

            string? fromDate = FilterFromDate?.ToString("dd/MM/yyyy");
            string? toDate = FilterToDate?.ToString("dd/MM/yyyy");

            (ErrorLog) = await APIService.GetErrors(
                fromDate,
                toDate,
                FilterIPAddress,
                FilterSummary,
                PageSize,
                PageNumber);

            if (ErrorLog == null)
            {
                ErrorLog = new()
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
            FilterIPAddress = string.Empty;
            FilterSummary = string.Empty;
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

            if (PageNumber < ErrorLog?.TotalPageCount)
            {
                PageNumber++;
                await LoadData();
            }
        }
    }
}
