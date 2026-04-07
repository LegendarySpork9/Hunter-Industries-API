using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Configuration
    {
        [Inject] private APIService APIService { get; set; } = default!;

        private Dictionary<string, (string DisplayName, string Icon)> _configTypes = new();

        protected override void OnInitialized()
        {
            var types = APIService.GetConfigurationTypes();
            var iconMap = new Dictionary<string, (string, string)>
            {
                ["application"] = ("Application", "&#x1F4E6;"),
                ["authorisation"] = ("Authorisation", "&#x1F512;"),
                ["component"] = ("Component", "&#x1F9E9;"),
                ["connection"] = ("Connection", "&#x1F310;"),
                ["downtime"] = ("Downtime", "&#x23F0;"),
                ["game"] = ("Game", "&#x1F3AE;"),
                ["machine"] = ("Machine", "&#x1F5A5;")
            };

            foreach (var type in types)
            {
                if (iconMap.TryGetValue(type, out var info))
                {
                    _configTypes[type] = info;
                }
            }
        }
    }
}
