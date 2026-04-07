using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Hunter_Industries_API_Control_Panel.Models;
using Hunter_Industries_API_Control_Panel.Services;

namespace Hunter_Industries_API_Control_Panel.Components.Pages
{
    public partial class Login
    {
        [Inject] private APIService APIService { get; set; } = default!;
        [Inject] private UserModel User { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private ProtectedSessionStorage SessionStorage { get; set; } = default!;

        private LoginModel _loginModel = new();
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        private async Task HandleLogin()
        {
            _isLoading = true;
            _errorMessage = string.Empty;
            StateHasChanged();

            try
            {
                var (success, token) = APIService.Authenticate(
                    _loginModel.Username,
                    _loginModel.Password,
                    _loginModel.Phrase);

                if (success)
                {
                    User.Username = _loginModel.Username;
                    User.Token = token;
                    User.TokenExpiry = DateTime.UtcNow.AddMinutes(15);

                    await SessionStorage.SetAsync("username", User.Username);
                    await SessionStorage.SetAsync("token", User.Token);

                    Navigation.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    _errorMessage = "Invalid credentials. Please check your username, password and phrase.";
                }
            }
            catch (Exception)
            {
                _errorMessage = "An error occurred during authentication. Please try again.";
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private class LoginModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Phrase { get; set; } = string.Empty;
        }
    }
}
