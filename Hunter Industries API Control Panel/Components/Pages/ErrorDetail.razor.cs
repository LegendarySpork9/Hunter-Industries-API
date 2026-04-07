using Microsoft.AspNetCore.Components;
using Hunter_Industries_API_Control_Panel.Models;
using Hunter_Industries_API_Control_Panel.Services;

namespace Hunter_Industries_API_Control_Panel.Components.Pages
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
