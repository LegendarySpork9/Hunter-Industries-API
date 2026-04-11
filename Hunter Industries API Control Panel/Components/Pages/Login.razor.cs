// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPICommon.Functions;
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Login
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private UserModel User { get; set; } = default!;

        private readonly LoginForm LoginInformation = new();

        private bool IsLoading;
        private string ErrorMessage = string.Empty;

        /// <summary>
        /// Captures the user IP for logging.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _Logger.ChangeIdentifier(IPAddressFunction.FetchIpAddress(HttpContextAccessor));
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Login Page");
            }
        }

        /// <summary>
        /// Performs the login steps.
        /// </summary>
        private async Task HandleLogin()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            StateHasChanged();

            try
            {
                await APIService.Authorise();

                List<UserModel> users = await APIService.GetUsers(false);
                UserModel? user = users.Find(u => u.Username == LoginInformation.Username && u.Password == HashFunction.HashString(LoginInformation.Password));

                if (user != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Login Successful.");
                    _Logger.ChangeIdentifier($"{user.Username} ({IPAddressFunction.FetchIpAddress(HttpContextAccessor)})");

                    User.Id = user.Id;
                    User.Username = user.Username;
                    User.Password = user.Password;
                    User.Scopes = user.Scopes;
                    User.IsLoggedIn = true;
                    
                    Navigation.NavigateTo("/");
                }

                else
                {
                    ErrorMessage = "Invalid credentials. Please check your username and password.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }
            }

            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during authentication. Please try again.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Stores the information used for logging in.
        /// </summary>
        private class LoginForm
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
