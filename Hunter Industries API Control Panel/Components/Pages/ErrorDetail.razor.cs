using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class ErrorDetail
    {
        [Parameter] public int Id { get; set; }
        [Inject] private APIService APIService { get; set; } = default!;

        private ErrorLogRecord? _error;

        protected override void OnInitialized()
        {
            _error = APIService.GetError(Id);
        }
    }
}
