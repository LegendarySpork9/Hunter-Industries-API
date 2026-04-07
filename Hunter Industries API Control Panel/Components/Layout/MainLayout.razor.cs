using HunterIndustriesAPIControlPanel.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HunterIndustriesAPIControlPanel.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        private UserModel User { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private ProtectedSessionStorage SessionStorage { get; set; } = default!;

        private bool _isInitialised;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    ProtectedBrowserStorageResult<string> usernameResult = await SessionStorage.GetAsync<string>("username");

                    if (usernameResult.Success && !string.IsNullOrEmpty(usernameResult.Value))
                    {
                        User.Username = usernameResult.Value;
                        _isInitialised = true;
                        StateHasChanged();
                    }
                    else
                    {
                        Navigation.NavigateTo("/login", forceLoad: true);
                    }
                }
                catch
                {
                    Navigation.NavigateTo("/login", forceLoad: true);
                }
            }
        }

        private async Task SignOut()
        {
            User.Username = string.Empty;

            await SessionStorage.DeleteAsync("username");

            Navigation.NavigateTo("/login", forceLoad: true);
        }
    }
}
