// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Functions;
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
        [Inject]
        private APISettingsModel APISettings { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        private UserModel? User;
        private List<UserSettingModel> UserSettings = [];
        private List<ApplicationModel> Applications = [];

        private bool LoadingData = true;
        private List<string> AvailableScopes = [];
        private string EditUsername = string.Empty;
        private string EditPassword = string.Empty;
        private List<string> EditScopes = [];
        private List<UserSettingModel> UnchangedUserSettings = [];
        private string ControlPanelUsername = string.Empty;
        private bool ShowPassword;
        private bool IsLoading;
        private string ErrorMessage = string.Empty;
        private bool SaveUserSuccess;
        private bool ShowDeleteConfirm;
        private List<UserSettingRequestModel> NewSettings = [];
        private (string, bool) IsSettingLoading;
        private string? SavedSettingsApplication;
        private bool ShowValidationErrors;
        private List<string> ValidationErrors = [];
        private bool ShowSaveSettingErrors;
        private List<string> SaveSettingErrors = [];
        private string SavedSettingsMessage = "Saved changes successfully!";

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened User Page");

            User = await APIService.GetUser(Id);
            UserSettings = await APIService.GetUserSettings(Id);

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<ApplicationModel>? pagedApplications = await APIService.GetApplications(200, pageNumber);

                if (pagedApplications != null && pagedApplications.EntryCount > 0)
                {
                    Applications.AddRange(pagedApplications.Entries);

                    if (pageNumber < pagedApplications.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            Applications.RemoveAll(a => a.IsDeleted);

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

            UnchangedUserSettings = [.. UserSettings.Select(us => new UserSettingModel
            {
                Application = us.Application,
                Settings = [.. us.Settings.Select(s => new SettingModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value
                })]
            })];

            ControlPanelUsername = CredentialsFunction.GetCredentialsUsername(APISettings);

            LoadingData = false;
        }

        /// <summary>
        /// Changes whether the scope is enabled.
        /// </summary>
        private void ToggleScope(string scope,
            bool isChecked)
        {
            if (isChecked && !EditScopes.Contains(scope))
            {
                EditScopes.Add(scope);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Added Scope: {scope}");
            }

            else if (!isChecked)
            {
                EditScopes.Remove(scope);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Removed Scope: {scope}");
            }
        }

        /// <summary>
        /// Performs the user update steps.
        /// </summary>
        private async Task SaveUser()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Save Changes Clicked");

            IsLoading = true;

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

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Scopes: {string.Join(", ", User.Scopes)} -> {string.Join(", ", EditScopes)}");
                }

                (UserModel? user, ResponseModel? apiResponse) = await APIService.UpdateUser(User.Id,
                    userUpdate);
                
                if (user != null)
                {
                    SaveUserSuccess = true;
                    User = user;
                    EditUsername = User.Username;
                    EditPassword = User.Password;
                    EditScopes = [.. User.Scopes];
                }

                else
                {
                    SaveUserSuccess = false;
                    ErrorMessage = $"API returned {apiResponse?.StatusCode} ({apiResponse?.Message})";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }

                await InvokeAsync(StateHasChanged);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {SaveUserSuccess}");

                await Task.Delay(2000).ContinueWith(_ =>
                {
                    SaveUserSuccess = false;
                    ErrorMessage = string.Empty;

                    InvokeAsync(StateHasChanged);
                });
            }

            IsLoading = false;
        }

        /// <summary>
        /// Shows the delete confirmation modal.
        /// </summary>
        private void ConfirmDelete()
        {
            ShowDeleteConfirm = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Delete Confirmation Modal");
        }

        /// <summary>
        /// Performs the user deletion steps.
        /// </summary>
        private async Task DeleteUser()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Delete Clicked");

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

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Delete Confirmation Model");
        }

        /// <summary>
        /// Checks if the setting is being added.
        /// </summary>
        private bool IsAddingSetting(string application,
            string settingName)
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
        private string GetNewSettingValue(string application,
            string settingName)
        {
            UserSettingRequestModel? newSetting = NewSettings.FirstOrDefault(ns => ns.Application == application && ns.SettingName == settingName);

            return newSetting?.SettingValue ?? string.Empty;
        }

        /// <summary>
        /// Adds the new setting to the list.
        /// </summary>
        private void SetNewSettingValue(string application,
            string settingName,
            string value)
        {
            int index = NewSettings.FindIndex(us => us.Application == application && us.SettingName == settingName);
            NewSettings[index].SettingValue = value;
        }

        /// <summary>
        /// Starts adding the new setting.
        /// </summary>
        private void StartAddSetting(string application,
            string settingName)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Add Clicked");

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
        private void CancelAddSetting(string application,
            string settingName)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Cancel Clicked");

            int index = NewSettings.FindIndex(us => us.Application == application && us.SettingName == settingName);
            NewSettings.RemoveAt(index);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private async Task SaveSettings(string application,
            UserSettingModel? userSettings)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Save Settings Clicked");

            IsSettingLoading.Item1 = application;
            IsSettingLoading.Item2 = true;
            int settingsAddedOrUpdated = 0;

            ApplicationModel app = Applications.First(a => a.Name == application);
            List<UserSettingRequestModel> newSettings = [.. NewSettings.Where(ns => ns.Application == application)];

            ValidationErrors.AddRange(SettingValidatorFunction.ValidateApplicationSettings(app,
                userSettings,
                newSettings));

            List<SettingModel> settingsToUpdate = [];

            if (userSettings != null)
            {
                UserSettingModel unchangedUserSettings = UnchangedUserSettings.First(us => us.Application == application);

                foreach (SettingModel setting in userSettings.Settings)
                {
                    SettingModel unchangedSetting = unchangedUserSettings.Settings.First(s => s.Id == setting.Id);

                    if (setting.Value != unchangedSetting.Value)
                    {
                        settingsToUpdate.Add(setting);
                    }
                }
            }

            if (ValidationErrors.Count > 0)
            {
                ShowValidationErrors = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Validation Error Modal");

                await InvokeAsync(StateHasChanged);
            }

            else
            {
                List<UserSettingRequestModel> addedSettings = [];

                foreach (UserSettingRequestModel newSetting in newSettings)
                {
                    (UserSettingModel? newAPISetting, ResponseModel? apiResponse) = await APIService.CreateUserSetting(newSetting);

                    if (newAPISetting != null)
                    {
                        int index = UserSettings.FindIndex(us => us.Application == newAPISetting.Application);
                        SettingModel createdSetting = newAPISetting.Settings[0];
                        SettingModel baselineSetting = new()
                        {
                            Id = createdSetting.Id,
                            Name = createdSetting.Name,
                            Value = createdSetting.Value
                        };

                        if (index == -1)
                        {
                            UserSettings.Add(newAPISetting);
                            UnchangedUserSettings.Add(new()
                            {
                                Application = newAPISetting.Application,
                                Settings = [baselineSetting]
                            });
                            addedSettings.Add(newSetting);
                        }

                        else
                        {
                            UserSettings[index].Settings.Add(createdSetting);
                            UnchangedUserSettings.First(us => us.Application == newAPISetting.Application).Settings.Add(baselineSetting);
                            addedSettings.Add(newSetting);
                        }
                    }

                    else
                    {
                        SaveSettingErrors.Add($"API returned {apiResponse?.StatusCode} ({apiResponse?.Message}) when adding {newSetting.SettingName} ({newSetting.SettingValue}) to {application}.");
                    }
                }

                settingsAddedOrUpdated += addedSettings.Count;

                foreach (UserSettingRequestModel addedSetting in addedSettings)
                {
                    NewSettings.Remove(addedSetting);
                }

                List<SettingModel> updatedSettings = [];

                foreach (SettingModel setting in settingsToUpdate)
                {
                    UserSettingUpdateRequestModel updateSettingModel = new()
                    {
                        Value = setting.Value
                    };

                    (SettingModel? updatedSetting, ResponseModel? apiResponse) = await APIService.UpdateUserSetting(setting.Id,
                        updateSettingModel);

                    if (updatedSetting != null)
                    {
                        updatedSettings.Add(updatedSetting);
                    }

                    else
                    {
                        SaveSettingErrors.Add($"API returned {apiResponse?.StatusCode} ({apiResponse?.Message}) when updating {setting.Name} ({setting.Value}) for {application}.");
                    }
                }

                settingsAddedOrUpdated += updatedSettings.Count;

                if (settingsToUpdate.Count > 0)
                {
                    UserSettingModel unchangedSettings = UnchangedUserSettings.First(us => us.Application == application);

                    foreach (SettingModel updatedSetting in updatedSettings)
                    {
                        int index = unchangedSettings.Settings.FindIndex(s => s.Id == updatedSetting.Id);
                        unchangedSettings.Settings[index].Value = updatedSetting.Value;
                    }
                }

                if (SaveSettingErrors.Count > 0)
                {
                    SavedSettingsApplication = null;
                    ShowSaveSettingErrors = true;

                    if (settingsAddedOrUpdated > 0)
                    {
                        SavedSettingsApplication = application;
                        SavedSettingsMessage = $"Saved {settingsAddedOrUpdated} changes successfully!";
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Save Setting Error Modal");

                    await InvokeAsync(StateHasChanged);
                }

                else if (settingsAddedOrUpdated > 0 && SaveSettingErrors.Count == 0)
                {
                    SavedSettingsApplication = application;
                    SavedSettingsMessage = $"Saved {settingsAddedOrUpdated} changes successfully!";

                    await InvokeAsync(StateHasChanged);
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Save Success: {SavedSettingsApplication != null}");

                if (SavedSettingsApplication != null)
                {
                    await Task.Delay(2000).ContinueWith(_ =>
                    {
                        SavedSettingsApplication = null;

                        InvokeAsync(StateHasChanged);
                    });
                }
            }

            IsSettingLoading.Item1 = string.Empty;
            IsSettingLoading.Item2 = false;
        }

        /// <summary>
        /// Hides the validation errors modal.
        /// </summary>
        private void CloseValidationErrors()
        {
            ShowValidationErrors = false;
            ValidationErrors = [];

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Validation Errors Modal");
        }

        /// <summary>
        /// Hides the save setting errors modal.
        /// </summary>
        private void CloseSaveSettingErrors()
        {
            ShowSaveSettingErrors = false;
            SaveSettingErrors = [];

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Save Setting Errors Modal");
        }

        /// <summary>
        /// Returns the text for the save button.
        /// </summary>
        private string GetSaveButtonText(string application,
            UserSettingModel? userSettings)
        {
            int unsavedCount = NewSettings.Count(ns => ns.Application == application && !string.IsNullOrWhiteSpace(ns.SettingValue));

            if (userSettings != null)
            {
                UserSettingModel unchangedUserSettings = UnchangedUserSettings.First(us => us.Application == application);
                unsavedCount += userSettings.Settings.Count(s => s.Value != unchangedUserSettings.Settings.First(us => us.Id == s.Id).Value);
            }

            string buttonTextstring;

            if (unsavedCount > 0)
            {
                buttonTextstring = $"Save {unsavedCount} Setting(s)";
            }

            else
            {
                buttonTextstring = "Save Settings";
            }

            return buttonTextstring;
        }
    }
}
