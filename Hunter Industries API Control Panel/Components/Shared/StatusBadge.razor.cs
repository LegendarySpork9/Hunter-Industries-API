using Microsoft.AspNetCore.Components;

namespace Hunter_Industries_API_Control_Panel.Components.Shared
{
    public partial class StatusBadge
    {
        [Parameter] public string Text { get; set; } = string.Empty;
        [Parameter] public string Type { get; set; } = "default";
    }
}
