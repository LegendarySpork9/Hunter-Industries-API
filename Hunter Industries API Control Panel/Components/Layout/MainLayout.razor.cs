// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HunterIndustriesAPIControlPanel.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject]
        private UserModel User { get; set; } = default!;

        private bool IsInitialised;

        /// <summary>
        /// Checks if the user is logged in.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    ProtectedBrowserStorageResult<UserModel> userResult = await SessionStorage.GetAsync<UserModel>("loggedInUser");

                    if (userResult.Success && (userResult.Value != null && userResult.Value.IsLoggedIn))
                    {
                        if (User.Id == 0)
                        {
                            User = userResult.Value;
                        }

                        IsInitialised = true;
                        StateHasChanged();
                    }

                    else
                    {
                        Navigation.NavigateTo("/login",
                            forceLoad: true);
                    }
                }

                catch
                {
                    Navigation.NavigateTo("/login",
                        forceLoad: true);
                }
            }
        }

        /// <summary>
        /// Performs the logout steps.
        /// </summary>
        private async Task SignOut()
        {
            await SessionStorage.DeleteAsync("loggedInUser");

            User.IsLoggedIn = false;

            Navigation.NavigateTo("/login",
                forceLoad: true);
        }
    }
}
