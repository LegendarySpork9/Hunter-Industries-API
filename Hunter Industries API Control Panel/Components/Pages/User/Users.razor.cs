using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace HunterIndustriesAPIControlPanel.Components.Pages.User
{
    public partial class Users
    {
        [Inject]
        private IConfigurableLoggerService _Logger { get; set; } = default!;
        [Inject]
        private APIService APIService { get; set; } = default!;
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private RadzenDataGrid<UserModel> UserGrid = new();
        private List<UserModel> UserRecords = [];

        private List<string> AvailableScopes = [];
        private bool ShowCreateModel;
        private bool ShowDeleteConfirm;
        private string ErrorMessage = string.Empty;
        private UserRequestModel? UserToCreate;
        private UserModel? UserToDelete;

        /// <summary>
        /// Loads and transforms the data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Opened Users Page");

            UserRecords = await APIService.GetUsers(true);

            if (UserRecords.Count > 0)
            {
                List<string> scopes = [.. UserRecords.SelectMany(u => u.Scopes)];
                AvailableScopes = [.. scopes.Distinct()];

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Available Scope(s): {AvailableScopes.Count}");
            }
        }

        /// <summary>
        /// Shows the create user modal.
        /// </summary>
        private void OpenCreateModal()
        {
            UserToCreate = new()
            {
                Scopes = []
            };
            ErrorMessage = string.Empty;
            ShowCreateModel = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened New User Modal");
        }

        /// <summary>
        /// Hides the create user model.
        /// </summary>
        private void CloseCreateModal()
        {
            ShowCreateModel = false;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed New User Modal");
        }

        /// <summary>
        /// Changes whether the scope is enabled.
        /// </summary>
        private void ToggleScope(string scope, bool isChecked)
        {
            if (UserToCreate != null && UserToCreate.Scopes != null)
            {
                if (isChecked && !UserToCreate.Scopes.Contains(scope))
                {
                    UserToCreate.Scopes.Add(scope);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Added Scope: {scope}");
                }

                else if (!isChecked)
                {
                    UserToCreate.Scopes.Remove(scope);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Removed Scope: {scope}");
                }
            }
        }

        /// <summary>
        /// Performs the user creation steps.
        /// </summary>
        private async Task CreateUser()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Create User Clicked");

            if (UserToCreate != null)
            {
                if (string.IsNullOrWhiteSpace(UserToCreate.Username) || string.IsNullOrWhiteSpace(UserToCreate.Password))
                {
                    ErrorMessage = "Username and password are required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    return;
                }

                if (UserToCreate.Scopes != null && UserToCreate.Scopes.Count == 0)
                {
                    ErrorMessage = "At least one valid scope is required.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    return;
                }

                UserModel? existingUser = UserRecords.Find(u => u.Username == UserToCreate.Username && u.IsDeleted == false);

                if (existingUser == null)
                {
                    UserModel? newUser = await APIService.CreateUser(UserToCreate);

                    if (newUser != null)
                    {
                        UserRecords.Add(newUser);
                        ShowCreateModel = false;

                        await UserGrid.Reload();
                        await InvokeAsync(StateHasChanged);
                    }

                    else
                    {
                        ErrorMessage = "Something went wrong. Please check logs for details.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                    }
                }

                else
                {
                    ErrorMessage = "A user with that username already exists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
                }
            }

            else
            {
                ErrorMessage = "The UserToCreate model is null.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ErrorMessage);
            }
        }

        /// <summary>
        /// Directs the user to the edit page.
        /// </summary>
        private void NavigateToEdit(int id) => Navigation.NavigateTo($"/users/{id}");

        /// <summary>
        /// Directs the user to the logs page.
        /// </summary>
        private void NavigateToLogs(int id) => Navigation.NavigateTo($"/logs?user={id}");

        /// <summary>
        /// Shows the delete confirmation modal.
        /// </summary>
        private void ConfirmDelete(UserModel user)
        {
            UserToDelete = user;
            ShowDeleteConfirm = true;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened Delete Confirmation Modal");
        }

        /// <summary>
        /// Performs the user deletion steps.
        /// </summary>
        private async Task DeleteUser()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Delete User Clicked");

            if (UserToDelete != null)
            {
                bool deleted = await APIService.DeleteUser(UserToDelete.Id);

                if (deleted)
                {
                    int index = UserRecords.IndexOf(UserToDelete);
                    UserRecords[index].IsDeleted = true;
                }
            }

            ShowDeleteConfirm = false;
            UserToDelete = null;
        }

        /// <summary>
        /// Hides the delete confirmation modal.
        /// </summary>
        private void CancelDelete()
        {
            ShowDeleteConfirm = false;
            UserToDelete = null;

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Closed Delete Confirmation Model");
        }
    }
}
