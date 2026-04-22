using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Shared
{
    public partial class AlertDialog
    {
        [Parameter]
        public bool IsVisible { get; set; }
        [Parameter]
        public string Title { get; set; } = "Alert";
        [Parameter]
        public List<string> Messages { get; set; } = [];
        [Parameter]
        public string CloseText { get; set; } = "OK";
        [Parameter]
        public EventCallback OnClose { get; set; }
    }
}
