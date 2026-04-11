using HunterIndustriesAPIControlPanel.Models.Responses;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
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
                    if (User.IsLoggedIn)
                    {
                        IsInitialised = true;
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

        /// <summary>
        /// Performs the logout steps.
        /// </summary>
        private async Task SignOut()
        {
            User.IsLoggedIn = false;

            Navigation.NavigateTo("/login", forceLoad: true);
        }
    }
}
