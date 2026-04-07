using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Errors
    {
        [Inject] private APIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<ErrorLogRecord> _allErrors = new();
        private List<ErrorLogRecord> _filteredErrors = new();
        private List<ErrorLogRecord> _pagedErrors = new();
        private List<ChartDataItem> _errorsOverTime = new();
        private List<ChartDataItem> _errorsByIP = new();
        private List<ChartDataItem> _errorsBySummary = new();
        private string[] _errorsByIPColours = Array.Empty<string>();
        private string _errorsYearRange = string.Empty;

        private static readonly string[] DefaultPalette = new[]
        {
            "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5",
            "#70AD47", "#264478", "#9B57A0", "#636363", "#EB7E30"
        };

        private int _pageSize = 25;
        private int _pageNumber = 1;
        private DateTime? _filterFromDate;
        private string _filterIPAddress = string.Empty;
        private string _filterSummary = string.Empty;

        private int TotalPages => _filteredErrors.Count == 0 ? 1 : (int)Math.Ceiling((double)_filteredErrors.Count / _pageSize);

        protected override void OnInitialized()
        {
            _allErrors = APIService.GetErrors();
            _filteredErrors = _allErrors;
            BuildCharts();
            UpdatePagedErrors();
        }

        private void BuildCharts()
        {
            var errorsByMonth = _allErrors
                .GroupBy(e => new { e.DateOccured.Year, e.DateOccured.Month })
                .ToDictionary(g => (g.Key.Year, g.Key.Month), g => g.Count());

            // Build a full 12-month range ending at the current month
            var now = DateTime.UtcNow;
            _errorsOverTime = Enumerable.Range(0, 12)
                .Select(i =>
                {
                    var date = new DateTime(now.Year, now.Month, 1).AddMonths(-11 + i);
                    return new ChartDataItem
                    {
                        Label = date.ToString("MMM"),
                        Value = errorsByMonth.GetValueOrDefault((date.Year, date.Month), 0),
                        Category = date.ToString("MMM yyyy")
                    };
                })
                .ToList();

            var startYear = new DateTime(now.Year, now.Month, 1).AddMonths(-11).Year;
            var endYear = now.Year;
            _errorsYearRange = startYear == endYear ? $"{startYear}" : $"{startYear} → {endYear}";

            _errorsByIP = _allErrors
                .GroupBy(e => e.IPAddress)
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList();

            _errorsByIPColours = _errorsByIP
                .Select((_, i) => DefaultPalette[i % DefaultPalette.Length])
                .ToArray();

            _errorsBySummary = _allErrors
                .GroupBy(e => ExtractClassMethod(e.Summary))
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .Where(c => c.Value > 0)
                .OrderByDescending(c => c.Value)
                .Take(6)
                .ToList();
        }

        private void UpdatePagedErrors()
        {
            _pagedErrors = _filteredErrors
                .Skip((_pageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
        }

        private void ApplyFilters()
        {
            _filteredErrors = APIService.GetErrors(
                _filterFromDate,
                string.IsNullOrWhiteSpace(_filterIPAddress) ? null : _filterIPAddress,
                string.IsNullOrWhiteSpace(_filterSummary) ? null : _filterSummary);
            _pageNumber = 1;
            UpdatePagedErrors();
        }

        private void ClearFilters()
        {
            _filterFromDate = null;
            _filterIPAddress = string.Empty;
            _filterSummary = string.Empty;
            _filteredErrors = _allErrors;
            _pageNumber = 1;
            UpdatePagedErrors();
        }

        private void PreviousPage()
        {
            if (_pageNumber > 1)
            {
                _pageNumber--;
                UpdatePagedErrors();
            }
        }

        private void NextPage()
        {
            if (_pageNumber < TotalPages)
            {
                _pageNumber++;
                UpdatePagedErrors();
            }
        }

        private void OnPageSizeChanged()
        {
            _pageNumber = 1;
            UpdatePagedErrors();
        }

        private static string ExtractClassMethod(string summary)
        {
            var words = summary.TrimEnd('.').Split(' ');

            for (int i = words.Length - 1; i >= 0; i--)
            {
                if (words[i].Contains('.'))
                {
                    var classMethod = words[i].TrimEnd('.');
                    var dotIndex = classMethod.LastIndexOf('.');

                    if (dotIndex >= 0)
                    {
                        return classMethod[(dotIndex + 1)..];
                    }

                    return classMethod;
                }
            }

            return summary.Length > 40 ? summary[..40] + "..." : summary;
        }
    }
}
