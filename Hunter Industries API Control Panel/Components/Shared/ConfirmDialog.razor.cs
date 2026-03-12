using Microsoft.AspNetCore.Components;

namespace Hunter_Industries_API_Control_Panel.Components.Shared
{
    public partial class ConfirmDialog
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string Title { get; set; } = "Confirm";
        [Parameter] public string Message { get; set; } = "Are you sure?";
        [Parameter] public string ConfirmText { get; set; } = "Confirm";
        [Parameter] public EventCallback OnConfirm { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
    }
}
