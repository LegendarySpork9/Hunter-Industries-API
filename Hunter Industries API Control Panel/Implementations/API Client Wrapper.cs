// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Converters;
using HunterIndustriesAPIControlPanel.Models;
using HunterIndustriesAPIControlPanel.Models.Responses;
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
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    auth = JsonConvert.DeserializeObject<AuthenticationModel>(response.Content) ?? null;
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
                string url = BuildURL("/user", ignoreQuery: !includeDeleted);

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
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    users = JsonConvert.DeserializeObject<List<UserModel>>(response.Content) ?? [];

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Users Returned: {users.Count}");
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
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    dashboardStatistics = JsonConvert.DeserializeObject<DashboardStatisticsModel>(response.Content) ?? null;
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
                string url = BuildURL("/auditHistory", queryParameters: queryParameters);

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
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Response Message: {response.ErrorException?.Message ?? response.Content}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {
                    pagedAuditHistory = JsonConvert.DeserializeObject<PagedAPIResponseModel<AuditHistoryModel>>(response.Content) ?? null;
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
        /// Returns the API url.
        /// </summary>
        private string BuildURL(string endpoint, object? entityId = null, List<KeyValuePair<string, object>>? queryParameters = null, bool ignoreQuery = false)
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
