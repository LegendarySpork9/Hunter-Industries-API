// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Components.Shared;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class PortfolioDashboard
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IClock _Clock { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        private PortfolioStatisticsModel? Statistics;

        private bool IsLoading;

        private List<ChartDataPointModel> ItemSummaryViews = [];
        private List<ChartDataPointModel> ItemFullDetailViews = [];
        private Dictionary<string, List<ChartDataPointModel>> LLMModelByCompany = [];
        private Dictionary<string, string> LLMModelColours = [];
        private string[] FrameworkColours = [];
        private string[] LanguageColours = [];
        private string[] EnvironmentColours = [];

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Portfolio Dashboard Page");

            IsLoading = true;

            Statistics = await APIService.GetPortfolioStatistics();

            if (Statistics != null)
            {
                ItemSummaryViews = [.. Statistics.TopFiveViewedItems.Select(i => new ChartDataPointModel
                {
                    Label = i.Name,
                    Value = i.SummaryViews
                })];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Item Summary Views: {ItemSummaryViews.Count}");

                ItemFullDetailViews = [.. Statistics.TopFiveViewedItems.Select(i => new ChartDataPointModel
                {
                    Label = i.Name,
                    Value = i.FullDetailViews
                })];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Item Full Detail Views: {ItemFullDetailViews.Count}");

                if (Statistics.LLMsUsed != null)
                {
                    string[] companies = [.. Statistics.LLMsUsed.Select(llm => llm.Company)
                        .Distinct()
                        .OrderBy(llm => llm)];
                    string[] models = [.. Statistics.LLMsUsed.Select(llm => llm.Model)
                        .Distinct()
                        .OrderBy(llm => llm)];
                    Dictionary<(string, string), int> llmLookup = Statistics.LLMsUsed.GroupBy(llm => new { llm.Model, llm.Company })
                        .ToDictionary(llm => (llm.Key.Model, llm.Key.Company), llm => llm.Sum(uses => uses.Uses));
                    LLMModelByCompany = models.ToDictionary(model => model, model => companies.Select(company => new ChartDataPointModel
                    {
                        Label = company,
                        Value = llmLookup.GetValueOrDefault((model, company), 0)
                    }).ToList());

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"LLMs used by Company: {LLMModelByCompany.Count}");

                    LLMModelColours = LLMModelByCompany.Keys.Select((key, llm) => (key, colour: Colours.DefaultPalette[llm % Colours.DefaultPalette.Length]))
                        .ToDictionary(llm => llm.key, llm => llm.colour);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"LLMs Used Colour(s): {LLMModelColours.Count}");
                }

                FrameworkColours = [.. Statistics.TopFiveFrameworks.Select((_, f) => Colours.DefaultPalette[f % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Framework Colour(s): {FrameworkColours.Length}");

                LanguageColours = [.. Statistics.TopFiveLanguages.Select((_, l) => Colours.DefaultPalette[l % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Language Colour(s): {LanguageColours.Length}");

                EnvironmentColours = [.. Statistics.TopFiveEnvironments.Select((_, e) => Colours.DefaultPalette[e % Colours.DefaultPalette.Length])];

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Environment Colour(s): {EnvironmentColours.Length}");
            }

            IsLoading = false;
        }
    }
}
