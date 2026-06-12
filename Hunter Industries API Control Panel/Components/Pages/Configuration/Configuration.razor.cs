// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Configuration
{
    public partial class Configuration
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        private ConfigurationModel? ConfigurationObjects { get; set; }

        private bool IsLoading;

        private readonly Dictionary<string, (string DisplayName, string Icon)> ConfigTypes = [];

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Opened Configuration Page");

            IsLoading = true;

            ConfigurationObjects = await APIService.GetConfiguration();

            if (ConfigurationObjects != null)
            {
                Dictionary<string, (string, string)> iconMap = new()
                {
                    ["application"] = ("Application", "&#x1F4E6;"),
                    ["authorisation"] = ("Authorisation", "&#x1F512;"),
                    ["component"] = ("Component", "&#x1F9E9;"),
                    ["connection"] = ("Connection", "&#x1F310;"),
                    ["domain"] = ("Domain", "&#x1F310;"),
                    ["downtime"] = ("Downtime", "&#x23F0;"),
                    ["game"] = ("Game", "&#x1F3AE;"),
                    ["machine"] = ("Machine", "&#x1F5A5;")
                };

                foreach (string type in ConfigurationObjects.ConfigurationObjects)
                {
                    if (iconMap.TryGetValue(type, out (string, string) info))
                    {
                        ConfigTypes[type] = info;
                    }
                }
            }

            IsLoading = false;
        }
    }
}
