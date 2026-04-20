// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;

namespace HunterIndustriesAPIControlPanel.Components.Pages.User
{
    public partial class UserDetail
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        private UserModel? User;
        private List<UserSettingModel> UserSettings = [];
        private List<ApplicationModel> Applications = [];

        private List<string> AvailableScopes = [];
        private string EditUsername = string.Empty;
        private string EditPassword = string.Empty;
        private List<string> EditScopes = [];
        private List<UserSettingModel> UnchangedUserSettings = [];
        private bool ShowPassword;
        private bool SaveUserSuccess;
        private bool ShowDeleteConfirm;
        private List<UserSettingRequestModel> NewSettings = [];
        private bool SaveSettingsSuccess;


        [Inject] private ExampleAPIService ExampleAPIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private UserRecord? _user;
        private List<string> _availableScopes = new();
        private List<string> _editScopes = new();
        private List<UserSettingRecord> _userSettings = new();
        private List<UserSettingRecord> _allAppSettings = new();
        private HashSet<string> _addingSettings = new();
        private Dictionary<string, string> _newSettingValues = new();
        private string _editUsername = string.Empty;
        private string _editPassword = string.Empty;
        private bool _showPassword;
        private bool _saveSuccess;
        private bool _showDeleteConfirm;

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened User Page");

            User = await APIService.GetUser(Id);
            UserSettings = await APIService.GetUserSettings(Id);
            Applications = await APIService.GetApplications();

            List<UserModel> users = await APIService.GetUsers(true);

            if (users.Count > 0)
            {
                List<string> scopes = [.. users.SelectMany(u => u.Scopes)];
                AvailableScopes = [.. scopes.Distinct()];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Available Scope(s): {AvailableScopes.Count}");
            }

            if (User != null)
            {
                EditUsername = User.Username;
                EditPassword = User.Password;
                EditScopes = [.. User.Scopes];
            }

            UnchangedUserSettings = [.. UserSettings];
        }

        /// <summary>
        /// Changes whether the scope is enabled.
        /// </summary>
        private void ToggleScope(string scope, bool isChecked)
        {
            if (isChecked && !EditScopes.Contains(scope))
            {
                EditScopes.Add(scope);
            }

            else if (!isChecked)
            {
                EditScopes.Remove(scope);
            }
        }

        /// <summary>
        /// Performs the user update steps.
        /// </summary>
        private async Task SaveUser()
        {
            if (User != null)
            {
                UserRequestModel userUpdate = new();

                if (!string.IsNullOrWhiteSpace(EditUsername) && EditUsername != User.Username)
                {
                    userUpdate.Username = EditUsername;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Username: {User.Username} -> {EditUsername}");
                }

                if (!string.IsNullOrWhiteSpace(EditPassword) && EditPassword != User.Password)
                {
                    userUpdate.Password = EditPassword;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Password: {User.Password} -> {EditPassword}");
                }

                if (EditScopes.Count > 0 && !EditScopes.ToHashSet().SetEquals(User.Scopes))
                {
                    userUpdate.Scopes = EditScopes;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Scopes: {string.Join(',', User.Scopes)} -> {string.Join(',', EditScopes)}");
                }

                User = await APIService.UpdateUser(User.Id, userUpdate);
                
                if (User != null)
                {
                    SaveUserSuccess = true;
                    EditUsername = User.Username;
                    EditPassword = User.Password;
                    EditScopes = [.. User.Scopes];

                    await InvokeAsync(StateHasChanged);
                }

                else
                {
                    SaveUserSuccess = false;
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {SaveUserSuccess}");

                if (SaveUserSuccess)
                {
                    await Task.Delay(2000).ContinueWith(_ =>
                    {
                        SaveUserSuccess = false;

                        InvokeAsync(StateHasChanged);
                    });
                }
            }
        }

        /// <summary>
        /// Shows the delete confirmation modal.
        /// </summary>
        private void ConfirmDelete()
        {
            ShowDeleteConfirm = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Delete Confirmation Modal");
        }

        /// <summary>
        /// Performs the user deletion steps.
        /// </summary>
        private async Task DeleteUser()
        {
            if (User != null)
            {
                bool deleted = await APIService.DeleteUser(User.Id);
                User.IsDeleted = deleted;
            }

            ShowDeleteConfirm = false;
        }

        /// <summary>
        /// Hides the delete confirmation modal.
        /// </summary>
        private void CancelDelete()
        {
            ShowDeleteConfirm = false;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Closed Delete Confirmation Model");
        }

        /// <summary>
        /// Checks if the setting is being added.
        /// </summary>
        private bool IsAddingSetting(string application, string settingName)
        {
            bool addingSetting;

            UserSettingRequestModel? newSetting = NewSettings.FirstOrDefault(ns => ns.Application == application && ns.SettingName == settingName);

            if (newSetting != null)
            {
                addingSetting = true;
            }

            else
            {
                addingSetting = false;
            }

            return addingSetting;
        }

        /// <summary>
        /// Gets the new setting's value.
        /// </summary>
        private string GetNewSettingValue(string application, string settingName)
        {
            string value = string.Empty;

            UserSettingRequestModel newSetting = NewSettings.First(ns => ns.Application == application && ns.SettingName == settingName);

            if (newSetting.SettingValue != string.Empty)
            {
                value = newSetting.SettingValue;
            }

            return value;
        }

        /// <summary>
        /// Adds the new setting to the list.
        /// </summary>
        private void SetNewSettingValue(string application, string settingName, string value)
        {
            int index = NewSettings.FindIndex(us => us.Application == application && us.SettingName == settingName);
            NewSettings[index].SettingValue = value;
        }

        /// <summary>
        /// Performs the user setting creation steps.
        /// </summary>
        private async Task ConfirmAddSetting(string application, string settingName)
        {
            UserSettingModel? newSetting = await APIService.CreateUserSetting(NewSettings.First(us => us.Application == application && us.SettingName == settingName));

            if (newSetting != null)
            {
                int index = UserSettings.FindIndex(us => us.Application == newSetting.Application);
                UserSettings[index].Settings.Add(newSetting.Settings[0]);

                index = NewSettings.FindIndex(us => us.Application == application && us.SettingName == settingName);
                NewSettings.RemoveAt(index);
            }
        }

        /// <summary>
        /// Starts adding the new setting.
        /// </summary>
        private void StartAddSetting(string application, string settingName)
        {
            if (User != null)
            {
                NewSettings.Add(new()
                {
                    UserId = User.Id,
                    Application = application,
                    SettingName = settingName,
                    SettingValue = string.Empty
                });
            }
        }

        /// <summary>
        /// Removes the setting from the new setting list.
        /// </summary>
        private void CancelAddSetting(string application, string settingName)
        {
            int index = NewSettings.FindIndex(us => us.Application == application && us.SettingName == settingName);
            NewSettings.RemoveAt(index);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private async Task SaveSettings(UserSettingModel userSettings)
        {
            List<bool> settingResults = [];

            UserSettingModel unchangedUserSettings = UnchangedUserSettings.First(us => us.Application == userSettings.Application);

            foreach (SettingModel setting in userSettings.Settings)
            {
                SettingModel unchangedSetting = unchangedUserSettings.Settings.First(s => s.Id == setting.Id);

                if (setting.Value != unchangedSetting.Value)
                {
                    SettingModel? updatedSetting = await APIService.UpdateUserSetting(setting.Id, setting.Value);

                    if (updatedSetting != null)
                    {
                        settingResults.Add(true);
                    }

                    else
                    {
                        settingResults.Add(false);
                    }
                }
            }

            List<bool> failedUpates = settingResults.FindAll(sr => sr == false);

            if (settingResults.Count> 0 && failedUpates.Count == 0)
            {
                SaveSettingsSuccess = true;

                await InvokeAsync(StateHasChanged);
            }

            else
            {
                SaveSettingsSuccess = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {SaveSettingsSuccess}");

            if (SaveSettingsSuccess)
            {
                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveSettingsSuccess = false;

                    InvokeAsync(StateHasChanged);
                });
            }
        }
    }
}
