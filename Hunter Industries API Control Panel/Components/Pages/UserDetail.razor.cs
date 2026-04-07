using Microsoft.AspNetCore.Components;
using Hunter_Industries_API_Control_Panel.Models;
using Hunter_Industries_API_Control_Panel.Services;

namespace Hunter_Industries_API_Control_Panel.Components.Pages
{
    public partial class UserDetail
    {
        [Parameter] public int Id { get; set; }
        [Inject] private APIService APIService { get; set; } = default!;
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

        protected override void OnInitialized()
        {
            _user = APIService.GetUser(Id);
            _availableScopes = APIService.GetAvailableScopes();

            _allAppSettings = APIService.GetAllApplicationSettings();

            if (_user != null)
            {
                _editUsername = _user.Username;
                _editPassword = _user.Password;
                _editScopes = new List<string>(_user.Scopes);
                _userSettings = APIService.GetUserSettings(_user.Username);
            }
        }

        private void ToggleScope(string scope, bool isChecked)
        {
            if (isChecked && !_editScopes.Contains(scope))
                _editScopes.Add(scope);
            else if (!isChecked)
                _editScopes.Remove(scope);
        }

        private void SaveUser()
        {
            if (_user == null) return;

            APIService.UpdateUser(_user.Id, _editUsername, _editPassword, _editScopes);
            _user = APIService.GetUser(Id);
            _saveSuccess = true;

            Task.Delay(2000).ContinueWith(_ =>
            {
                _saveSuccess = false;
                InvokeAsync(StateHasChanged);
            });
        }

        private void SaveSettings(UserSettingRecord appSetting)
        {
            if (_user == null) return;

            foreach (var setting in appSetting.Settings)
            {
                APIService.UpdateUserSetting(_user.Username, appSetting.Application, setting.Id, setting.Value);
            }
        }

        private bool IsAddingSetting(string application, string settingName)
        {
            return _addingSettings.Contains($"{application}:{settingName}");
        }

        private string GetNewSettingValue(string application, string settingName)
        {
            return _newSettingValues.GetValueOrDefault($"{application}:{settingName}", string.Empty);
        }

        private void SetNewSettingValue(string application, string settingName, string value)
        {
            _newSettingValues[$"{application}:{settingName}"] = value;
        }

        private void StartAddSetting(string application, string settingName)
        {
            var key = $"{application}:{settingName}";
            _addingSettings.Add(key);
            _newSettingValues[key] = string.Empty;
        }

        private void CancelAddSetting(string application, string settingName)
        {
            var key = $"{application}:{settingName}";
            _addingSettings.Remove(key);
            _newSettingValues.Remove(key);
        }

        private void ConfirmDelete()
        {
            _showDeleteConfirm = true;
        }

        private void DeleteUser()
        {
            if (_user != null)
            {
                APIService.DeleteUser(_user.Id);
                Navigation.NavigateTo("/users");
            }
        }

        private void CancelDelete()
        {
            _showDeleteConfirm = false;
        }

        private void ConfirmAddSetting(string application, string settingName)
        {
            if (_user == null) return;

            var key = $"{application}:{settingName}";
            var value = _newSettingValues.GetValueOrDefault(key, string.Empty);

            APIService.AddUserSetting(_user.Username, application, settingName, value);
            _userSettings = APIService.GetUserSettings(_user.Username);

            _addingSettings.Remove(key);
            _newSettingValues.Remove(key);
        }
    }
}
