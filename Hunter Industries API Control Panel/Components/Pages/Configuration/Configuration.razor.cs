using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.Configuration
{
    public partial class Configuration
    {
        [Inject]
        private ExampleAPIService APIService { get; set; } = default!;

        private readonly Dictionary<string, (string DisplayName, string Icon)> ConfigTypes = [];

        protected override void OnInitialized()
        {
            List<string> types = APIService.GetConfigurationTypes();
            Dictionary<string, (string, string)> iconMap = new()
            {
                ["application"] = ("Application", "&#x1F4E6;"),
                ["authorisation"] = ("Authorisation", "&#x1F512;"),
                ["component"] = ("Component", "&#x1F9E9;"),
                ["connection"] = ("Connection", "&#x1F310;"),
                ["downtime"] = ("Downtime", "&#x23F0;"),
                ["game"] = ("Game", "&#x1F3AE;"),
                ["machine"] = ("Machine", "&#x1F5A5;")
            };

            foreach (string type in types)
            {
                if (iconMap.TryGetValue(type, out (string, string) info))
                {
                    ConfigTypes[type] = info;
                }
            }
        }
    }
}
