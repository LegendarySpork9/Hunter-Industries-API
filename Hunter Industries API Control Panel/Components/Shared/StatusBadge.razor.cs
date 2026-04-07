using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Shared
{
    public partial class StatusBadge
    {
        [Parameter] public string Text { get; set; } = string.Empty;
        [Parameter] public string Type { get; set; } = "default";
    }
}
