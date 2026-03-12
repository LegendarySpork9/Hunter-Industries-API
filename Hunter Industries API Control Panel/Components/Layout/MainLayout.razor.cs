using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Hunter_Industries_API_Control_Panel.Models;

namespace Hunter_Industries_API_Control_Panel.Components.Layout
{
    public partial class MainLayout
    {
        private bool _isInitialised;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    var usernameResult = await SessionStorage.GetAsync<string>("username");
                    var tokenResult = await SessionStorage.GetAsync<string>("token");

                    if (usernameResult.Success && tokenResult.Success &&
                        !string.IsNullOrEmpty(usernameResult.Value) && !string.IsNullOrEmpty(tokenResult.Value))
                    {
                        User.Username = usernameResult.Value;
                        User.Token = tokenResult.Value;
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
            User.Token = string.Empty;

            await SessionStorage.DeleteAsync("username");
            await SessionStorage.DeleteAsync("token");

            Navigation.NavigateTo("/login", forceLoad: true);
        }
    }
}
