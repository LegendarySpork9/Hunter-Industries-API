using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Shared
{
    public partial class StatusBadge
    {
        [Parameter]
        public string Text { get; set; } = string.Empty;
        [Parameter]
        public string Type { get; set; } = "default";

        /// <summary>
        /// Returns the class name to change the style depending on the type.
        /// </summary>
        private string GetClass()
        {
            return Type switch
            {
                "active" => "badge-active",
                "inactive" => "badge-inactive",
                "deleted" => "badge-deleted",
                "success" => "badge-active",
                "warning" => "badge bg-warning text-dark",
                "danger" => "badge bg-danger",
                "info" => "badge bg-info text-dark",
                _ => "badge bg-secondary"
            };
        }
    }
}
