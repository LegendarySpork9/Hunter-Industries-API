// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Converters;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using Newtonsoft.Json;
using RestSharp;
using UserModel = HunterIndustriesAPIControlPanel.Models.Responses.UserModel;

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

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", APISettings.Credentials);
                client.AddDefaultHeader("Accept", "application/json");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = _FileSystem.ReadAllText($@"{APISettings.PayloadLocation}\Authorise.json");

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    auth = JsonConvert.DeserializeObject<AuthenticationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
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
                string url = BuildURL("/user",
                    ignoreQuery: !includeDeleted);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    users = JsonConvert.DeserializeObject<List<UserModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {users.Count}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
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

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    dashboardStatistics = JsonConvert.DeserializeObject<DashboardStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
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
                string url = BuildURL("/auditHistory",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedAuditHistory = JsonConvert.DeserializeObject<PagedAPIResponseModel<AuditHistoryModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedAuditHistory;
        }

        /// <summary>
        /// Returns the new user from the API.
        /// </summary>
        public async Task<UserModel?> CreateUser(UserRequestModel user)
        {
            UserModel? createdUser = null;

            try
            {
                string url = BuildURL("/user",
                    ignoreQuery: true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(user);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdUser = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return createdUser;
        }

        /// <summary>
        /// Returns whether the user was deleted in the API.
        /// </summary>
        public async Task<bool> DeleteUser(int userId)
        {
            bool deleted = false;

            try
            {
                string url = BuildURL("/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Delete
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    deleted = true;
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return deleted;
        }

        /// <summary>
        /// Returns a the user from the API.
        /// </summary>
        public async Task<UserModel?> GetUser(int userId)
        {
            UserModel? user = null;

            try
            {
                string url = BuildURL("/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    user = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return user;
        }

        /// <summary>
        /// Returns a the application from the API.
        /// </summary>
        public async Task<ApplicationModel?> GetApplication(int applicationId)
        {
            ApplicationModel? application = null;

            try
            {
                string url = BuildURL("/configuration/application",
                    applicationId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    application = JsonConvert.DeserializeObject<ApplicationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return application;
        }

        /// <summary>
        /// Returns the statistics for the logs from the API.
        /// </summary>
        public async Task<SharedStatisticsModel?> GetLogStatistics(string entity,
            int entityId)
        {
            SharedStatisticsModel? sharedStatistics = null;

            try
            {
                string url = BuildURL($"/statistic/{entity}",
                    entityId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    sharedStatistics = JsonConvert.DeserializeObject<SharedStatisticsModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
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
                string url = BuildURL("/usersettings",
                    userId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    userSettings = JsonConvert.DeserializeObject<List<UserSettingModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"User Settings Returned: {userSettings.Select(us => us.Settings.Count).Sum()}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return userSettings;
        }

        /// <summary>
        /// Returns the paged application from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<ApplicationModel>?> GetPagedApplication(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<ApplicationModel>? pagedApplication = null;

            try
            {
                string url = BuildURL("/configuration/application",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedApplication = JsonConvert.DeserializeObject<PagedAPIResponseModel<ApplicationModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedApplication;
        }

        /// <summary>
        /// Returns updated user from the API.
        /// </summary>
        public async Task<UserModel?> UpdateUser(int userId,
            UserRequestModel user)
        {
            UserModel? updatedUser = null;

            try
            {
                string url = BuildURL("/user",
                    userId,
                    ignoreQuery: true);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(user);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedUser = JsonConvert.DeserializeObject<UserModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return updatedUser;
        }

        /// <summary>
        /// Returns the new user setting from the API.
        /// </summary>
        public async Task<UserSettingModel?> CreateUserSetting(UserSettingRequestModel userSetting)
        {
            UserSettingModel? createdUserSetting = null;

            try
            {
                string url = BuildURL("/usersettings");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(userSetting);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdUserSetting = JsonConvert.DeserializeObject<UserSettingModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return createdUserSetting;
        }

        /// <summary>
        /// Returns updated user setting from the API.
        /// </summary>
        public async Task<SettingModel?> UpdateUserSetting(int userSettingId,
            string value)
        {
            SettingModel? updatedUserSetting = null;

            try
            {
                string url = BuildURL("/usersettings",
                    userSettingId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(value);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedUserSetting = JsonConvert.DeserializeObject<SettingModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return updatedUserSetting;
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

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    servers = JsonConvert.DeserializeObject<List<ServerInformationModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Servers Returned: {servers.Count}");
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return servers;
        }

        /// <summary>
        /// Returns the paged machine from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<MachineModel>?> GetPagedMachine(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<MachineModel>? pagedMachine = null;

            try
            {
                string url = BuildURL("/configuration/machine",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedMachine = JsonConvert.DeserializeObject<PagedAPIResponseModel<MachineModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedMachine;
        }

        /// <summary>
        /// Returns the paged game from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<GameModel>?> GetPagedGame(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<GameModel>? pagedGame = null;

            try
            {
                string url = BuildURL("/configuration/game",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedGame = JsonConvert.DeserializeObject<PagedAPIResponseModel<GameModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedGame;
        }

        /// <summary>
        /// Returns the paged connection from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<ConnectionModel>?> GetPagedConnection(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<ConnectionModel>? pagedConnection = null;

            try
            {
                string url = BuildURL("/configuration/connection",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedConnection = JsonConvert.DeserializeObject<PagedAPIResponseModel<ConnectionModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedConnection;
        }

        /// <summary>
        /// Returns the paged downtime from the API.
        /// </summary>
        public async Task<PagedAPIResponseModel<DowntimeModel>?> GetPagedDowntime(List<KeyValuePair<string, object>>? queryParameters = null)
        {
            PagedAPIResponseModel<DowntimeModel>? pagedDowntime = null;

            try
            {
                string url = BuildURL("/configuration/downtime",
                    queryParameters: queryParameters);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                RestRequest request = new()
                {
                    Method = Method.Get
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedDowntime = JsonConvert.DeserializeObject<PagedAPIResponseModel<DowntimeModel>>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return pagedDowntime;
        }

        /// <summary>
        /// Returns the new server from the API.
        /// </summary>
        public async Task<ServerInformationModel?> CreateServer(ServerRequestModel server)
        {
            ServerInformationModel? createdServer = null;

            try
            {
                string url = BuildURL("/serverstatus/serverinformation");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(server);

                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.Created && response.Content != null)
                {
                    createdServer = JsonConvert.DeserializeObject<ServerInformationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return createdServer;
        }

        /// <summary>
        /// Returns the updated server from the API.
        /// </summary>
        public async Task<ServerInformationModel?> UpdateServer(int serverId,
            ServerUpdateRequestModel server)
        {
            ServerInformationModel? updatedServer = null;

            try
            {
                string url = BuildURL("/serverstatus/serverinformation",
                    serverId);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"URL: {url}");

                RestClient client = new(url);
                client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Client");

                string body = JsonConvert.SerializeObject(server);

                RestRequest request = new()
                {
                    Method = Method.Patch
                };
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Request Body: {body}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Configured Rest Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                RestResponse response = await client.ExecuteAsync(request);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Code: {response.StatusCode}");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.Content ?? "No Response Content"}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    updatedServer = JsonConvert.DeserializeObject<ServerInformationModel>(response.Content);
                }

                if (response.ErrorException != null)
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Error: {response.ErrorException.Message}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Response Stack Trace: {response.ErrorException.StackTrace}");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return updatedServer;
        }

        /// <summary>
        /// Returns the API url.
        /// </summary>
        private string BuildURL(string endpoint,
            object? entityId = null,
            List<KeyValuePair<string, object>>? queryParameters = null,
            bool ignoreQuery = false)
        {
            string url = $"{APISettings.BaseURL}/{APISettings.Version}{endpoint}";
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
