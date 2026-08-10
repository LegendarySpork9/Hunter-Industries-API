// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPICommon.Functions;
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HunterIndustriesAPIControlPanel.Components.Pages
{
    public partial class Login
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private ProtectedSessionStorage SessionStorage { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private UserModel User { get; set; } = default!;
        [Inject]
        private TimezoneService TimezoneService { get; set; } = default!;
        [Inject]
        private APISettingsModel APISettings { get; set; } = default!;

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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Opened Login Page");
            }
        }

        /// <summary>
        /// Performs the login steps.
        /// </summary>
        private async Task HandleLogin()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                "Login Clicked");

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                await APIService.Authorise();

                List<UserModel> users = await APIService.GetUsers(false);
                UserModel? user = users.Find(u => u.Username == LoginInformation.Username && u.Password == HashFunction.HashString(LoginInformation.Password));

                if (user != null && user.Scopes.Contains("Control Panel API"))
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Login Successful");
                    _Logger.ChangeIdentifier($"{user.Username} ({IPAddressFunction.FetchIpAddress(HttpContextAccessor)})");

                    User.Id = user.Id;
                    User.Username = user.Username;
                    User.Password = user.Password;
                    User.Scopes = user.Scopes;
                    User.IsLoggedIn = true;

                    List<UserSettingModel> userSettings = await APIService.GetUserSettings(user.Id);
                    UserSettingModel? cpSettings = userSettings.FirstOrDefault(s => s.Application == APISettings.ApplicationName);

                    if (cpSettings != null)
                    {
                        SettingModel? enabledSetting = cpSettings.Settings.FirstOrDefault(s => s.Name == "Timezone Conversion Enabled");
                        SettingModel? offsetSetting = cpSettings.Settings.FirstOrDefault(s => s.Name == "Timezone Offset");

                        if (enabledSetting != null && bool.TryParse(enabledSetting.Value, out bool enabled))
                        {
                            TimezoneService.ConversionEnabled = enabled;
                        }

                        if (offsetSetting != null && double.TryParse(offsetSetting.Value, out double offset))
                        {
                            TimezoneService.OffsetHours = offset;
                        }
                    }

                    await SessionStorage.SetAsync(
                        "loggedInUser",
                        User);

                    await SessionStorage.SetAsync(
                        "timezonePreference",
                        new TimezonePreferenceModel
                        {
                            ConversionEnabled = TimezoneService.ConversionEnabled,
                            OffsetHours = TimezoneService.OffsetHours
                        });

                    Navigation.NavigateTo("/");
                }

                else
                {
                    ErrorMessage = "Invalid credentials. Please check your username, password and ensure you have the \"Control Panel API\" scope.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        ErrorMessage);
                }
            }

            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during authentication. Please try again.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            IsLoading = false;
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
