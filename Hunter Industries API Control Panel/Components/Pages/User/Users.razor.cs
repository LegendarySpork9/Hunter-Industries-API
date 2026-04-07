using Microsoft.AspNetCore.Components;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Components.Pages.User
{
    public partial class Users
    {
        [Inject] private APIService APIService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<UserRecord> _users = new();
        private List<string> _availableScopes = new();
        private List<string> _selectedScopes = new();
        private bool _showCreateModal;
        private bool _showDeleteConfirm;
        private string _newUsername = string.Empty;
        private string _newPassword = string.Empty;
        private string _createError = string.Empty;
        private UserRecord? _userToDelete;

        protected override void OnInitialized()
        {
            _users = APIService.GetUsers();
            _availableScopes = APIService.GetAvailableScopes();
        }

        private void ShowCreateModal()
        {
            _newUsername = string.Empty;
            _newPassword = string.Empty;
            _selectedScopes = new List<string>();
            _createError = string.Empty;
            _showCreateModal = true;
        }

        private void CloseCreateModal() => _showCreateModal = false;

        private void ToggleScope(string scope, bool isChecked)
        {
            if (isChecked && !_selectedScopes.Contains(scope))
                _selectedScopes.Add(scope);
            else if (!isChecked)
                _selectedScopes.Remove(scope);
        }

        private void CreateUser()
        {
            if (string.IsNullOrWhiteSpace(_newUsername) || string.IsNullOrWhiteSpace(_newPassword))
            {
                _createError = "Username and password are required.";
                return;
            }

            var success = APIService.CreateUser(_newUsername, _newPassword, _selectedScopes);

            if (success)
            {
                _users = APIService.GetUsers();
                _showCreateModal = false;
            }
            else
            {
                _createError = "A user with that username already exists.";
            }
        }

        private void NavigateToEdit(int id) => Navigation.NavigateTo($"/users/{id}");

        private void NavigateToLogs(int id) => Navigation.NavigateTo($"/logs?user={id}");

        private void ConfirmDelete(UserRecord user)
        {
            _userToDelete = user;
            _showDeleteConfirm = true;
        }

        private void DeleteUser()
        {
            if (_userToDelete != null)
            {
                APIService.DeleteUser(_userToDelete.Id);
                _users = APIService.GetUsers();
            }
            _showDeleteConfirm = false;
            _userToDelete = null;
        }

        private void CancelDelete()
        {
            _showDeleteConfirm = false;
            _userToDelete = null;
        }
    }
}
