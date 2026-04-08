// Copyright © - 05/10/2025 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Models.Responses;
namespace HunterIndustriesAPIControlPanel.Services
{
    public class APIService
    {
        private readonly ILoggerService _Logger;
        private readonly IAPIClient _APIClient;
        
        public DateTime ExpiryTime { get; set; }

        // Sets the class's global variables.
        public APIService(
            ILoggerService _logger,
            IAPIClient _apiClient)
        {
            _Logger = _logger;
            _APIClient = _apiClient;
        }

        /// <summary>
        /// Gets a bearer token from the API.
        /// </summary>
        public async Task Authorise()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer token from API");

            try
            {
                AuthenticationModel? auth = await _APIClient.Authorise();

                if (auth != null)
                {
                    _APIClient.SetBearerToken(auth.Token);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {auth.Token}");

                    ExpiryTime = DateTime.SpecifyKind(auth.Expires, DateTimeKind.Utc);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {ExpiryTime}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer token from API");
        }

        /// <summary>
        /// Gets a list of the users from the API.
        /// </summary>
        /*public async Task<List<UserModel>> GetUsers()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<UserModel> users = [];

            try
            {
                (users, bool success) = await _APIClient.GetUsers();

                if (success)
                {
                    foreach (UserModel user in users)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Id: {user.Id}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Username: {user.Username}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Password: {user.Password}");
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched users from API");
                }

                else
                {
                    if (RetryCount != 4)
                    {
                        RetryCount++;

                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Retry {RetryCount} of 4");

                        await Authorise();
                        users = await GetUsers();
                    }

                    else
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
            }

            RetryCount = 0;
            return users;
        }*/
    }
}
