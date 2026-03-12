using Microsoft.AspNetCore.Components;

namespace Hunter_Industries_API_Control_Panel.Components.Shared
{
    public partial class KPICard
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public string Value { get; set; } = "0";
        [Parameter] public string Icon { get; set; } = string.Empty;
        [Parameter] public string IconColour { get; set; } = "#D7D7D7";
        [Parameter] public string Trend { get; set; } = string.Empty;
        [Parameter] public bool IsError { get; set; }
    }
}
