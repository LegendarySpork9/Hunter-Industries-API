// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Converters;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Requests.Patch;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Newtonsoft.Json;
using RestSharp;

namespace HunterIndustriesAPIControlPanel.Implementations
{
    public class APIClientWrapper : IAPIClient
    {
        private readonly IConfigurableLoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly APISettingsModel APISettings;

        private string? BearerToken;

        // Sets the class's global variables.
        public APIClientWrapper(
            IConfigurableLoggerService _logger,
            IFileSystem _fileSystem,
            APISettingsModel apiSettings)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            APISettings = apiSettings;
        }

        /// <summary>
        /// Sets the bearer token to the given value.
        /// </summary>
        public void SetBearerToken(string bearerToken) => BearerToken = bearerToken;

        /// <summary>
        /// Returns the authentication from the API.
        /// </summary>
        public async Task<AuthenticationModel?> Authorise()
        {
            AuthenticationModel? auth = null;

            try
            {
                string url = BuildURL("/auth/token");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    APISettings.Credentials);
                client.AddDefaultHeader(
                    "Accept",
                    "application/json");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = _FileSystem.ReadAllText(APISettings.AuthPayloadLocation);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    auth = JsonConvert.DeserializeObject<AuthenticationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return auth;
        }

        /// <summary>
        /// Returns a list of users from the API.
        /// </summary>
        public async Task<List<UserModel>> GetUsers(bool includeDeleted)
        {
            List<UserModel> users = [];

            try
            {
                string url = BuildURL(
                    "/user",
                    ignoreQuery: !includeDeleted);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    users = JsonConvert.DeserializeObject<List<UserModel>>(response.Content) ?? [];

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Users Returned: {users.Count}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return users;
        }

        /// <summary>
        /// Returns the statistics for the dashboard from the API.
        /// </summary>
        public async Task<DashboardStatisticsModel?> GetDashboardStatistics()
        {
            DashboardStatisticsModel? dashboardStatistics = null;

            try
            {
                string url = BuildURL("/statistic/dashboard");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    dashboardStatistics = JsonConvert.DeserializeObject<DashboardStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return dashboardStatistics;
        }

        /// <summary>
        /// Returns the paged audit history from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<AuditHistoryModel>?> GetPagedAuditHistory(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<AuditHistoryModel>? pagedAuditHistory = null;

            try
            {
                string url = BuildURL(
                    "/auditHistory",
                    queryParameters: queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedAuditHistory = JsonConvert.DeserializeObject<PagedAPIResponseModel<AuditHistoryModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return pagedAuditHistory;
        }

        /// <summary>
        /// Returns the new user from the API.
        /// </summary>
        public async Task<(UserModel?, ResponseModel?)> CreateUser(UserRequestModel user)
        {
            UserModel? createdUser = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    "/user",
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(user);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdUser = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                createdUser,
                apiResponse
            );
        }

        /// <summary>
        /// Returns whether the user was deleted in the API.
        /// </summary>
        public async Task<bool> DeleteUser(int userId)
        {
            bool deleted = false;

            try
            {
                string url = BuildURL(
                    "/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Delete
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    deleted = true;
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return deleted;
        }

        /// <summary>
        /// Returns the user from the API.
        /// </summary>
        public async Task<UserModel?> GetUser(int userId)
        {
            UserModel? user = null;

            try
            {
                string url = BuildURL(
                    "/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    user = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return user;
        }

        /// <summary>
        /// Returns the configuration entity from the API.
        /// </summary>
        public async Task<T?> GetConfigurationEntity<T>(
            string entity,
            int entityId)
        {
            T? entityObject = default;

            try
            {
                string url = BuildURL(
                    $"/configuration/{entity}",
                    entityId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    entityObject = JsonConvert.DeserializeObject<T>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        "Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return entityObject;
        }

        /// <summary>
        /// Returns the statistics for the logs from the API.
        /// </summary>
        public async Task<SharedStatisticsModel?> GetLogStatistics(
            string entity,
            int entityId)
        {
            SharedStatisticsModel? sharedStatistics = null;

            try
            {
                string url = BuildURL(
                    $"/statistic/{entity}",
                    entityId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    sharedStatistics = JsonConvert.DeserializeObject<SharedStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return sharedStatistics;
        }

        /// <summary>
        /// Returns a list of user settings from the API for the user.
        /// </summary>
        public async Task<List<UserSettingModel>> GetUserSettings(int userId)
        {
            List<UserSettingModel> userSettings = [];

            try
            {
                string url = BuildURL(
                    "/usersettings",
                    userId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    userSettings = JsonConvert.DeserializeObject<List<UserSettingModel>>(response.Content) ?? [];

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Settings Returned: {userSettings.Select(us => us.Settings.Count).Sum()}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return userSettings;
        }

        /// <summary>
        /// Returns the paged configuration object from the API.
        /// </summary>
        public async Task<T?> GetPagedConfiguration<T>(
            string entity,
            List<KeyValuePair<string, object>>? queryParameters = null,
            bool ignoreQuery = true)
        {
            T? pagedObject = default;

            try
            {
                string url = BuildURL(
                    $"/configuration/{entity}",
                    queryParameters: queryParameters,
                    ignoreQuery: ignoreQuery);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedObject = JsonConvert.DeserializeObject<T>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return pagedObject;
        }

        /// <summary>
        /// Returns updated user from the API.
        /// </summary>
        public async Task<(UserModel?, ResponseModel?)> UpdateUser(
            int userId,
            UserUpdateRequestModel user)
        {
            UserModel? updatedUser = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    "/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(user);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedUser = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                updatedUser,
                apiResponse
            );
        }

        /// <summary>
        /// Returns the new user setting from the API.
        /// </summary>
        public async Task<(UserSettingModel?, ResponseModel?)> CreateUserSetting(UserSettingRequestModel userSetting)
        {
            UserSettingModel? createdUserSetting = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL("/usersettings");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(userSetting);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdUserSetting = JsonConvert.DeserializeObject<UserSettingModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                createdUserSetting,
                apiResponse
            );
        }

        /// <summary>
        /// Returns updated user setting from the API.
        /// </summary>
        public async Task<(SettingModel?, ResponseModel?)> UpdateUserSetting(
            int userSettingId,
            UserSettingUpdateRequestModel updateUserSetting)
        {
            SettingModel? updatedUserSetting = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    "/usersettings",
                    userSettingId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(updateUserSetting);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedUserSetting = JsonConvert.DeserializeObject<SettingModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                updatedUserSetting,
                apiResponse
            );
        }

        /// <summary>
        /// Returns a list of servers from the API.
        /// </summary>
        public async Task<List<ServerInformationModel>> GetServers()
        {
            List<ServerInformationModel> servers = [];

            try
            {
                string url = BuildURL("/serverstatus/serverinformation");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    servers = JsonConvert.DeserializeObject<List<ServerInformationModel>>(response.Content) ?? [];

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Servers Returned: {servers.Count}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return servers;
        }

        /// <summary>
        /// Returns the new server from the API.
        /// </summary>
        public async Task<(ServerInformationModel?, ResponseModel?)> CreateServer(ServerRequestModel server)
        {
            ServerInformationModel? createdServer = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL("/serverstatus/serverinformation");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(server);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdServer = JsonConvert.DeserializeObject<ServerInformationModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                createdServer,
                apiResponse
            );
        }

        /// <summary>
        /// Returns the updated server from the API.
        /// </summary>
        public async Task<(ServerInformationModel?, ResponseModel?)> UpdateServer(
            int serverId,
            ServerUpdateRequestModel server)
        {
            ServerInformationModel? updatedServer = null;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    "/serverstatus/serverinformation",
                    serverId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(server);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedServer = JsonConvert.DeserializeObject<ServerInformationModel>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                updatedServer,
                apiResponse
            );
        }

        /// <summary>
        /// Returns the server from the API.
        /// </summary>
        public async Task<ServerInformationModel?> GetServer(int serverId)
        {
            ServerInformationModel? server = null;

            try
            {
                string url = BuildURL(
                    "/serverstatus/serverinformation",
                    serverId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    server = JsonConvert.DeserializeObject<ServerInformationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return server;
        }

        /// <summary>
        /// Returns the statistics for the server from the API.
        /// </summary>
        public async Task<ServerStatisticsModel?> GetServerStatistics(int serverId)
        {
            ServerStatisticsModel? serverStatistics = null;

            try
            {
                string url = BuildURL(
                    "/statistic/server",
                    serverId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    serverStatistics = JsonConvert.DeserializeObject<ServerStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return serverStatistics;
        }

        /// <summary>
        /// Returns the configuration objects from the API.
        /// </summary>
        public async Task<ConfigurationModel?> GetConfiguration()
        {
            ConfigurationModel? configuration = null;

            try
            {
                string url = BuildURL("/configuration");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    configuration = JsonConvert.DeserializeObject<ConfigurationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return configuration;
        }

        /// <summary>
        /// Returns the new configuration entity from the API.
        /// </summary>
        public async Task<(T1?, ResponseModel?)> CreateConfigurationEntity<T1, T2>(
            string entity,
            T2 entityObject,
            List<KeyValuePair<string, object>>? queryParameters = null)
        {
            T1? createdEntity = default;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    $"/configuration/{entity}",
                    queryParameters: queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(entityObject);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdEntity = JsonConvert.DeserializeObject<T1>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                createdEntity,
                apiResponse
            );
        }

        /// <summary>
        /// Returns whether the enityt was deleted in the API.
        /// </summary>
        public async Task<bool> DeleteConfigurationEntity(
            string entity,
            int entityId)
        {
            bool deleted = false;

            try
            {
                string url = BuildURL(
                    $"/configuration/{entity}",
                    entityId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Delete
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    deleted = true;
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return deleted;
        }

        /// <summary>
        /// Returns the updated configuration entity from the API.
        /// </summary>
        public async Task<(T1?, ResponseModel?)> UpdateConfigurationEntity<T1, T2>(
            string entity,
            int entityId,
            T2 entityObject)
        {
            T1? updatedEntity = default;
            ResponseModel? apiResponse = null;

            try
            {
                string url = BuildURL(
                    $"/configuration/{entity}",
                    entityId);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                string body = JsonConvert.SerializeObject(entityObject);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter(
                    "application/json",
                    body,
                    ParameterType.RequestBody);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Request Body: {body}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedEntity = JsonConvert.DeserializeObject<T1>(response.Content);
                }

                else if (response.Content != null)
                {
                    APIMessageModel? apiMessage = JsonConvert.DeserializeObject<APIMessageModel>(response.Content);
                    apiResponse = new()
                    {
                        StatusCode = response.StatusCode,
                        Message = apiMessage?.Error ?? apiMessage?.Information ?? "No message returned by the API."
                    };
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return (
                updatedEntity,
                apiResponse
            );
        }

        /// <summary>
        /// Returns the statistics for the errors from the API.
        /// </summary>
        public async Task<ErrorStatisticsModel?> GetErrorStatistics()
        {
            ErrorStatisticsModel? errorStatistics = null;

            try
            {
                string url = BuildURL("/statistic/error");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    errorStatistics = JsonConvert.DeserializeObject<ErrorStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return errorStatistics;
        }

        /// <summary>
        /// Returns the paged error log from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<ErrorModel>?> GetPagedErrorLog(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<ErrorModel>? pagedErrorLog = null;

            try
            {
                string url = BuildURL(
                    "/errorlog",
                    queryParameters: queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedErrorLog = JsonConvert.DeserializeObject<PagedAPIResponseModel<ErrorModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return pagedErrorLog;
        }

        /// <summary>
        /// Returns the error from the API.
        /// </summary>
        public async Task<ErrorModel?> GetError(int errorId)
        {
            ErrorModel? error = null;

            try
            {
                string url = BuildURL(
                    "/errorlog",
                    errorId,
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    error = JsonConvert.DeserializeObject<ErrorModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return error;
        }

        /// <summary>
        /// Returns the audit history from the API.
        /// </summary>
        public async Task<AuditHistoryModel?> GetAuditHistory(int auditId)
        {
            AuditHistoryModel? auditHistory = null;

            try
            {
                string url = BuildURL(
                    "/audithistory",
                    auditId,
                    ignoreQuery: true);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader(
                    "Authorization",
                    $"Bearer {BearerToken}");

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Configured Rest Request");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    auditHistory = JsonConvert.DeserializeObject<AuditHistoryModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    ex.Message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString());
            }

            return auditHistory;
        }

        /// <summary>
        /// Returns the API url.
        /// </summary>
        private string BuildURL(
            string endpoint,
            object? entityId = null,
            List<KeyValuePair<string, object>>? queryParameters = null,
            bool ignoreQuery = false)
        {
            string url = $"{APISettings.BaseURL}{endpoint}";
            string query = APIConverter.GetQuery(endpoint);

            if (entityId != null)
            {
                url += $"/{entityId}";
            }

            if (queryParameters != null && queryParameters.Count > 0)
            {
                if (string.IsNullOrEmpty(query))
                {
                    query = "?";

                    for (int x = 0; x < queryParameters.Count; x++)
                    {
                        KeyValuePair<string, object> queryParameter = queryParameters[x];

                        query += $"{queryParameter.Key}={queryParameter.Value}";

                        if (x != (queryParameters.Count - 1))
                        {
                            query += "&";
                        }
                    }
                }

                else
                {
                    foreach (KeyValuePair<string, object> queryParameter in queryParameters)
                    {
                        query = query.Replace($"{queryParameter.Key}", $"{queryParameter.Value}");
                    }
                }
            }

            if (!ignoreQuery)
            {
                url += query;
            }

            return url;
        }
    }
}
