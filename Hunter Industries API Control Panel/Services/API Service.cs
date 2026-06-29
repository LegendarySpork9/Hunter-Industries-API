// Copyright © - 05/10/2025 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Components.Pages.AuditHistory;
using HunterIndustriesAPIControlPanel.Models.Requests.Patch;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Services
{
    public class APIService
    {
        private readonly IConfigurableLoggerService _Logger;
        private readonly IAPIClient _APIClient;
        private readonly IClock _Clock;
        
        public DateTime ExpiryTime { get; set; }

        // Sets the class's global variables.
        public APIService(
            IConfigurableLoggerService _logger,
            IAPIClient _apiClient,
            IClock _clock)
        {
            _Logger = _logger;
            _APIClient = _apiClient;
            _Clock = _clock;
        }

        /// <summary>
        /// Gets a bearer token from the API.
        /// </summary>
        public async Task Authorise()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Obtaining Bearer Token from API");

            try
            {
                AuthenticationModel? auth = await _APIClient.Authorise();

                if (auth != null)
                {
                    _APIClient.SetBearerToken(auth.Token);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Bearer Token: {auth.Token}");

                    ExpiryTime = DateTime.SpecifyKind(
                        auth.Info.Expires,
                        DateTimeKind.Utc);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Expiry Time: {ExpiryTime}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Obtained Bearer Token from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to Obtain Bearer Token from API");
            }
        }

        /// <summary>
        /// Gets a list of the users from the API.
        /// </summary>
        public async Task<List<UserModel>> GetUsers(bool includeDeleted)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching users from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<UserModel> users = [];

            try
            {
                users = await _APIClient.GetUsers(includeDeleted);

                if (users.Count > 0)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched users from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch users from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch users from API");
            }

            return users;
        }

        /// <summary>
        /// Gets the dashboard statistics from the API.
        /// </summary>
        public async Task<DashboardStatisticsModel?> GetDashboardStatistics()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching dashboard statistics from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            DashboardStatisticsModel? dashboardStatistics = null;

            try
            {
                dashboardStatistics = await _APIClient.GetDashboardStatistics();

                if (dashboardStatistics != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched dashboard statistics from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch dashboard statistics from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch dashboard statistics from API");
            }

            return dashboardStatistics;
        }

        /// <summary>
        /// Gets the recent audit histories from the API.
        /// </summary>
        public async Task<List<AuditHistoryModel>> GetRecentAuditLogs()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching audit history records from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<AuditHistoryModel> auditHistories = [];

            List<KeyValuePair<string, object>> queryParameters =
            [
                new("pageSize", 10)
            ];

            try
            {
                PagedAPIResponseModel<AuditHistoryModel>? pagedResponse = await _APIClient.GetPagedAuditHistory(queryParameters);

                if (pagedResponse != null && pagedResponse.Entries != null)
                {
                    auditHistories = pagedResponse.Entries;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Audit Histories Returned: {auditHistories.Count}");

                    foreach (AuditHistoryModel auditHistory in auditHistories)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specifying date times as UTC for audit history {auditHistory.Id}");

                        auditHistory.OccuredAt = DateTime.SpecifyKind(
                            auditHistory.OccuredAt,
                            DateTimeKind.Utc);

                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specified date times as UTC for audit history {auditHistory.Id}");
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched audit history records from API");
                }

                else
                {
                    pagedResponse = null;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch audit history records from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch audit history records from API");
            }

            return auditHistories;
        }

        /// <summary>
        /// Creates a user in the API.
        /// </summary>
        public async Task<(UserModel?, ResponseModel?)> CreateUser(UserRequestModel user)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Creating user, {user.Username}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            UserModel? createdUser = null;
            ResponseModel? apiResponse = null;

            try
            {
                (createdUser, apiResponse) = await _APIClient.CreateUser(user);

                if (createdUser != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Id: {createdUser.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Username: {createdUser.Username}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Password: {createdUser.Password}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Scopes: {string.Join(
                            ", ",
                            createdUser.Scopes)}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Created user, {user.Username}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to create user, {user.Username}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to create user, {user.Username}, in API");
            }

            return (
                createdUser,
                apiResponse
            );
        }

        /// <summary>
        /// Deletes a user in the API.
        /// </summary>
        public async Task<bool> DeleteUser(int userId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Deleting user, {userId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool deleted = false;

            try
            {
                deleted = await _APIClient.DeleteUser(userId);

                if (deleted)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Deleted user, {userId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to delete user, {userId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to delete user, {userId}, in API");
            }

            return deleted;
        }

        /// <summary>
        /// Gets the user from the API.
        /// </summary>
        public async Task<UserModel?> GetUser(int userId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching user, {userId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            UserModel? user = null;

            try
            {
                user = await _APIClient.GetUser(userId);

                if (user != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched user, {userId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch user, {userId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch user, {userId}, from API");
            }

            return user;
        }

        /// <summary>
        /// Gets the configuration entity from the API.
        /// </summary>
        public async Task<T?> GetConfigurationEntity<T>(
            string entity,
            int entityId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching {entity}, {entityId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            T? entityObject = default;

            try
            {
                entityObject = await _APIClient.GetConfigurationEntity<T>(
                    entity,
                    entityId);

                if (entityObject != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched {entity}, {entityId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch {entity}, {entityId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch {entity}, {entityId}, from API");
            }

            return entityObject;
        }

        /// <summary>
        /// Gets the logs statistics from the API.
        /// </summary>
        public async Task<SharedStatisticsModel?> GetLogStatistics(
            string entity,
            int entityId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching log statistics from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            SharedStatisticsModel? sharedStatistics = null;

            try
            {
                sharedStatistics = await _APIClient.GetLogStatistics(
                    entity,
                    entityId);

                if (sharedStatistics != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched log statistics from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch log statistics from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch log statistics from API");
            }

            return sharedStatistics;
        }

        /// <summary>
        /// Gets the audit histories from the API matching the given parameters.
        /// </summary>
        public async Task<PagedAPIResponseModel<AuditHistoryModel>?> GetAuditHistories(
            string? fromDate = null,
            string? toDate = null,
            string? ipAddress = null,
            string? endpoint = null,
            string? username = null,
            string? application = null,
            int pageSize = 25,
            int pageNumber = 1)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching audit history records from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            PagedAPIResponseModel<AuditHistoryModel>? pagedResponse = null;

            List<KeyValuePair<string, object>> queryParameters = [];

            if (!string.IsNullOrWhiteSpace(fromDate))
            {
                queryParameters.Add(new("fromDate", fromDate));
            }

            if (!string.IsNullOrWhiteSpace(toDate))
            {
                queryParameters.Add(new("toDate", toDate));
            }

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                queryParameters.Add(new("ipAddress", ipAddress));
            }

            if (!string.IsNullOrWhiteSpace(endpoint))
            {
                queryParameters.Add(new("endpoint", endpoint));
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                queryParameters.Add(new("username", username));
            }

            if (!string.IsNullOrWhiteSpace(application))
            {
                queryParameters.Add(new("application", application));
            }

            if (pageSize != 25)
            {
                queryParameters.Add(new("pageSize", pageSize));
            }

            if (pageNumber != 1)
            {
                queryParameters.Add(new("pageNumber", pageNumber));
            }

            try
            {
                pagedResponse = await _APIClient.GetPagedAuditHistory(queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Audit Histories Returned: {pagedResponse?.EntryCount ?? 0}");

                if (pagedResponse != null && pagedResponse.Entries != null)
                {
                    List<AuditHistoryModel> auditHistories = pagedResponse.Entries;

                    foreach (AuditHistoryModel auditHistory in auditHistories)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specifying date times as UTC for audit history {auditHistory.Id}");

                        auditHistory.OccuredAt = DateTime.SpecifyKind(
                            auditHistory.OccuredAt,
                            DateTimeKind.Utc);

                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specified date times as UTC for audit history {auditHistory.Id}");
                    }

                    pagedResponse.Entries = auditHistories;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched audit history records from API");
                }

                else
                {
                    pagedResponse = null;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch audit history records from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch audit history records from API");
            }

            return pagedResponse;
        }

        /// <summary>
        /// Gets a list of the user settings from the API for the user.
        /// </summary>
        public async Task<List<UserSettingModel>> GetUserSettings(int userId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching user settings from API for user, {userId}");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<UserSettingModel> userSettings = [];

            try
            {
                userSettings = await _APIClient.GetUserSettings(userId);

                if (userSettings.Count > 0)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched user settings from API for user, {userId}");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch user settings from API for user, {userId}");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch user settings from API for user, {userId}");
            }

            return userSettings;
        }

        /// <summary>
        /// Gets the configuration objects from the API.
        /// </summary>
        public async Task<T?> GetPagedConfiguration<T>(
            string entity,
            int pageSize = 25,
            int pageNumber = 1,
            bool ignoreQuery = true)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching {entity}s from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            T? configurationObjects = default;

            List<KeyValuePair<string, object>> queryParameters = [];

            if (pageSize != 25)
            {
                queryParameters.Add(new("pageSize", pageSize));
            }

            if (pageNumber != 1)
            {
                queryParameters.Add(new("pageNumber", pageNumber));
            }

            try
            {
                configurationObjects = await _APIClient.GetPagedConfiguration<T>(
                    entity,
                    queryParameters,
                    ignoreQuery);

                if (configurationObjects != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched {entity}s from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch {entity}s from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch {entity}s from API");
            }

            return configurationObjects;
        }

        /// <summary>
        /// Updates a user in the API.
        /// </summary>
        public async Task<(UserModel?, ResponseModel?)> UpdateUser(
            int userId,
            UserUpdateRequestModel user)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Updating user, {userId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            UserModel? updatedUser = null;
            ResponseModel? apiResponse = null;

            try
            {
                (updatedUser, apiResponse) = await _APIClient.UpdateUser(
                    userId,
                    user);

                if (updatedUser != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Id: {updatedUser.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Username: {updatedUser.Username}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Password: {updatedUser.Password}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Scopes: {string.Join(
                            ", ",
                            updatedUser.Scopes)}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Updated user, {userId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to update user, {userId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to update user, {userId}, in API");
            }

            return (
                updatedUser,
                apiResponse
            );
        }

        /// <summary>
        /// Creates a user in the API.
        /// </summary>
        public async Task<(UserSettingModel?, ResponseModel?)> CreateUserSetting(UserSettingRequestModel userSetting)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Creating user setting, {userSetting.SettingName}, in API for user, {userSetting.UserId}");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            UserSettingModel? createdUserSetting = null;
            ResponseModel? apiResponse = null;

            try
            {
                (createdUserSetting, apiResponse) = await _APIClient.CreateUserSetting(userSetting);

                if (createdUserSetting != null)
                {
                    SettingModel newSetting = createdUserSetting.Settings[0];

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Id: {newSetting.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Application: {createdUserSetting.Application}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Name: {newSetting.Name}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Value: {newSetting.Value}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Created user setting, {userSetting.SettingName}, in API for user, {userSetting.UserId}");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to create user setting, {userSetting.SettingName}, in API for user, {userSetting.UserId}");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to create user setting, {userSetting.SettingName}, in API for user, {userSetting.UserId}");
            }

            return (
                createdUserSetting,
                apiResponse
            );
        }

        /// <summary>
        /// Updates a user setting in the API.
        /// </summary>
        public async Task<(SettingModel?, ResponseModel?)> UpdateUserSetting(
            int userSettingId,
            UserSettingUpdateRequestModel updateUserSetting)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Updating user setting, {userSettingId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            SettingModel? updatedUserSetting = null;
            ResponseModel? apiResponse = null;

            try
            {
                (updatedUserSetting, apiResponse) = await _APIClient.UpdateUserSetting(
                    userSettingId,
                    updateUserSetting);

                if (updatedUserSetting != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Id: {updatedUserSetting.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Name: {updatedUserSetting.Name}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"User Setting Value: {updatedUserSetting.Value}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Updated user setting, {userSettingId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to update user setting, {userSettingId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to update user setting, {userSettingId}, in API");
            }

            return (
                updatedUserSetting,
                apiResponse
            );
        }

        /// <summary>
        /// Gets a list of the servers from the API.
        /// </summary>
        public async Task<List<ServerInformationModel>> GetServers()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching servers from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<ServerInformationModel> servers = [];

            try
            {
                servers = await _APIClient.GetServers();

                if (servers.Count > 0)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched users from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch servers from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch servers from API");
            }

            return servers;
        }

        /// <summary>
        /// Creates a server in the API.
        /// </summary>
        public async Task<(ServerInformationModel?, ResponseModel?)> CreateServer(ServerRequestModel server)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Creating server, {server.Name}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ServerInformationModel? createdServer = null;
            ResponseModel? apiResponse = null;

            try
            {
                (createdServer, apiResponse) = await _APIClient.CreateServer(server);

                if (createdServer != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Id: {createdServer.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Name: {createdServer.Name}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Event Interval: {createdServer.EventInterval}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Host Name: {createdServer.HostName}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Game: {createdServer.Game}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Game Version: {createdServer.GameVersion}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        "Server Ip Address: {createdServer.Connection.IpAddress}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Port: {createdServer.Connection.Port}");

                    if (createdServer.Downtime != null)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Debug,
                            $"Server Downtime: {createdServer.Downtime.Time}");
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Debug,
                            $"Server Downtime Duration: {createdServer.Downtime.Duration}");
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Created server, {server.Name}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to create server, {server.Name}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to create server, {server.Name}, in API");
            }

            return (
                createdServer,
                apiResponse
            );
        }

        /// <summary>
        /// Updates a server in the API.
        /// </summary>
        public async Task<(ServerInformationModel?, ResponseModel?)> UpdateServer(
            int serverId,
            ServerUpdateRequestModel server)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Updating server, {serverId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ServerInformationModel? updatedServer = null;
            ResponseModel? apiResponse = null;

            try
            {
                (updatedServer, apiResponse) = await _APIClient.UpdateServer(
                    serverId,
                    server);

                if (updatedServer != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Id: {updatedServer.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Name: {updatedServer.Name}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Event Interval: {updatedServer.EventInterval}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Host Name: {updatedServer.HostName}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Game: {updatedServer.Game}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Game Version: {updatedServer.GameVersion}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Ip Address: {updatedServer.Connection.IpAddress}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Port: {updatedServer.Connection.Port}");

                    if (updatedServer.Downtime != null)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Debug,
                            $"Server Downtime: {updatedServer.Downtime.Time}");
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Debug,
                            $"Server Downtime Duration: {updatedServer.Downtime.Duration}");
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Debug,
                        $"Server Active: {updatedServer.IsActive}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Updated server, {serverId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to update server, {serverId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to update server, {serverId}, in API");
            }

            return (
                updatedServer,
                apiResponse
            );
        }

        /// <summary>
        /// Gets the server from the API.
        /// </summary>
        public async Task<ServerInformationModel?> GetServer(int serverId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching server, {serverId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ServerInformationModel? server = null;

            try
            {
                server = await _APIClient.GetServer(serverId);

                if (server != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched server, {serverId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch server, {serverId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch server, {serverId}, from API");
            }

            return server;
        }

        /// <summary>
        /// Gets the server statistics from the API.
        /// </summary>
        public async Task<ServerStatisticsModel?> GetServerStatistics(int serverId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching server statistics from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ServerStatisticsModel? serverStatistics = null;

            try
            {
                serverStatistics = await _APIClient.GetServerStatistics(serverId);

                if (serverStatistics != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specifying date times as UTC for latest events");

                    foreach (ServerEventModel latestEvent in serverStatistics.LatestEvents)
                    {
                        latestEvent.DateOccured = DateTime.SpecifyKind(
                            latestEvent.DateOccured,
                            DateTimeKind.Utc);
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specified date times as UTC for latest events");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specifying date times as UTC for recent alerts");

                    foreach (RecentAlertModel recentAlert in serverStatistics.RecentAlerts)
                    {
                        recentAlert.AlertDate = DateTime.SpecifyKind(
                            recentAlert.AlertDate,
                            DateTimeKind.Utc);
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specified date times as UTC for recent alerts");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specifying date times as UTC for recent events");

                    foreach (ServerEventModel recentEvent in serverStatistics.RecentEvents)
                    {
                        recentEvent.DateOccured = DateTime.SpecifyKind(
                            recentEvent.DateOccured,
                            DateTimeKind.Utc);
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Specified date times as UTC for recent events");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched server statistics from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch server statistics from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch server statistics from API");
            }

            return serverStatistics;
        }

        /// <summary>
        /// Gets the configuration from the API.
        /// </summary>
        public async Task<ConfigurationModel?> GetConfiguration()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching configuration from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ConfigurationModel? configuration = null;

            try
            {
                configuration = await _APIClient.GetConfiguration();

                if (configuration != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched configuration from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch configuration from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch configuration from API");
            }

            return configuration;
        }

        /// <summary>
        /// Creates a configuration entity in the API.
        /// </summary>
        public async Task<(T1?, ResponseModel?)> CreateConfigurationEntity<T1, T2>(
            string entity,
            string newEntityValue,
            T2 entityObject,
            int? parentEntityId = null)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Creating {entity}, {newEntityValue}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            T1? newEntityObject = default;
            ResponseModel? apiResponse = null;

            List<KeyValuePair<string, object>> queryParameters = [];

            if (parentEntityId.HasValue)
            {
                queryParameters.Add(new("entityId", parentEntityId.Value));
            }

            try
            {
                (newEntityObject, apiResponse) = await _APIClient.CreateConfigurationEntity<T1, T2>(
                    entity,
                    entityObject,
                    queryParameters);

                if (newEntityObject != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Created {entity}, {newEntityValue}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to create {entity}, {newEntityValue}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to create {entity}, {newEntityValue}, in API");
            }

            return (
                newEntityObject,
                apiResponse
            );
        }

        /// <summary>
        /// Deletes an entity in the API.
        /// </summary>
        public async Task<bool> DeleteConfigurationEntity(
            string entity,
            int entityId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Deleting {entity}, {entityId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            bool deleted = false;

            try
            {
                deleted = await _APIClient.DeleteConfigurationEntity(
                    entity,
                    entityId);

                if (deleted)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Deleted {entity}, {entityId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to delete {entity}, {entityId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to delete {entity}, {entityId}, in API");
            }

            return deleted;
        }

        /// <summary>
        /// Updates a configuration entity in the API.
        /// </summary>
        public async Task<(T1?, ResponseModel?)> UpdateConfigurationEntity<T1, T2>(
            string entity,
            int entityId,
            T2 entityObject)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Updating {entity}, {entityId}, in API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            T1? updatedEntityObject = default;
            ResponseModel? apiResponse = null;

            try
            {
                (updatedEntityObject, apiResponse) = await _APIClient.UpdateConfigurationEntity<T1, T2>(
                    entity,
                    entityId,
                    entityObject);

                if (updatedEntityObject != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Updated {entity}, {entityId}, in API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to update {entity}, {entityId}, in API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to update {entity}, {entityId}, in API");
            }

            return (
                updatedEntityObject,
                apiResponse
            );
        }

        /// <summary>
        /// Gets the error statistics from the API.
        /// </summary>
        public async Task<ErrorStatisticsModel?> GetErrorStatistics()
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching error statistics from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ErrorStatisticsModel? errorStatistics = null;

            try
            {
                errorStatistics = await _APIClient.GetErrorStatistics();

                if (errorStatistics != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched error statistics from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch error statistics from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch error statistics from API");
            }

            return errorStatistics;
        }

        /// <summary>
        /// Gets the error logs from the API matching the given parameters.
        /// </summary>
        public async Task<PagedAPIResponseModel<ErrorModel>?> GetErrors(
            string? fromDate = null,
            string? toDate = null,
            string? ipAddress = null,
            string? summary = null,
            int pageSize = 25,
            int pageNumber = 1)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching error records from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            PagedAPIResponseModel<ErrorModel>? pagedResponse = null;

            List<KeyValuePair<string, object>> queryParameters = [];

            if (!string.IsNullOrWhiteSpace(fromDate))
            {
                queryParameters.Add(new("fromDate", fromDate));
            }

            if (!string.IsNullOrWhiteSpace(toDate))
            {
                queryParameters.Add(new("toDate", toDate));
            }

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                queryParameters.Add(new("ipAddress", ipAddress));
            }

            if (!string.IsNullOrWhiteSpace(summary))
            {
                queryParameters.Add(new("summary", summary));
            }

            if (pageSize != 25)
            {
                queryParameters.Add(new("pageSize", pageSize));
            }

            if (pageNumber != 1)
            {
                queryParameters.Add(new("pageNumber", pageNumber));
            }

            try
            {
                pagedResponse = await _APIClient.GetPagedErrorLog(queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Errors Returned: {pagedResponse?.EntryCount ?? 0}");

                if (pagedResponse != null && pagedResponse.Entries != null)
                {
                    List<ErrorModel> errors = pagedResponse.Entries;

                    foreach (ErrorModel error in errors)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specifying date times as UTC for error {error.Id}");

                        error.DateOccured = DateTime.SpecifyKind(
                            error.DateOccured,
                            DateTimeKind.Utc);

                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specified date times as UTC for error {error.Id}");
                    }

                    pagedResponse.Entries = errors;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched error records from API");
                }

                else
                {
                    pagedResponse = null;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch error records from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch audit history records from API");
            }

            return pagedResponse;
        }

        /// <summary>
        /// Gets the error from the API.
        /// </summary>
        public async Task<ErrorModel?> GetError(int errorId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching error, {errorId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            ErrorModel? error = null;

            try
            {
                error = await _APIClient.GetError(errorId);

                if (error != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Specifying date times as UTC for error {error.Id}");

                    error.DateOccured = DateTime.SpecifyKind(
                        error.DateOccured,
                        DateTimeKind.Utc);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Specified date times as UTC for error {error.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched error, {errorId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch error, {errorId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch error, {errorId}, from API");
            }

            return error;
        }

        /// <summary>
        /// Gets the audit History from the API.
        /// </summary>
        public async Task<AuditHistoryModel?> GetAuditHistory(int auditId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching audit history, {auditId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            AuditHistoryModel? auditHistory = null;

            try
            {
                auditHistory = await _APIClient.GetAuditHistory(auditId);

                if (auditHistory != null)
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Specifying date times as UTC for audit history {auditHistory.Id}");

                    auditHistory.OccuredAt = DateTime.SpecifyKind(
                        auditHistory.OccuredAt,
                        DateTimeKind.Utc);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Specified date times as UTC for audit history {auditHistory.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched audit history, {auditId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch audit history, {auditId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch audit history, {auditId}, from API");
            }

            return auditHistory;
        }

        /// <summary>
        /// Gets the media records from the API matching the given parameters.
        /// </summary>
        public async Task<PagedAPIResponseModel<MediaModel>?> GetMediaRecords(
            string application,
            int pageSize = 25,
            int pageNumber = 1)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                "Fetching media records from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            PagedAPIResponseModel<MediaModel>? pagedResponse = null;

            List<KeyValuePair<string, object>> queryParameters = [];

            if (pageSize != 25)
            {
                queryParameters.Add(new("pageSize", pageSize));
            }

            if (pageNumber != 1)
            {
                queryParameters.Add(new("pageNumber", pageNumber));
            }

            try
            {
                pagedResponse = await _APIClient.GetPagedMedia(
                    application,
                    queryParameters);

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"Media Returned: {pagedResponse?.EntryCount ?? 0}");

                if (pagedResponse != null && pagedResponse.Entries != null)
                {
                    List<MediaModel> mediaRecords = pagedResponse.Entries;

                    foreach (MediaModel mediaRecord in mediaRecords)
                    {
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specifying date times as UTC for media record {mediaRecord.Id}");

                        mediaRecord.DateUploaded = DateTime.SpecifyKind(
                            mediaRecord.DateUploaded,
                            DateTimeKind.Utc);
                        mediaRecord.DateUpdated = DateTime.SpecifyKind(
                            mediaRecord.DateUpdated,
                            DateTimeKind.Utc);

                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specified date times as UTC for media record {mediaRecord.Id}");
                    }

                    pagedResponse.Entries = mediaRecords;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Fetched media records from API");
                }

                else
                {
                    pagedResponse = null;

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        "Failed to fetch media records from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    "Failed to fetch media records from API");
            }

            return pagedResponse;
        }

        /// <summary>
        /// Gets the media record from the API.
        /// </summary>
        public async Task<MediaModel?> GetMedia(int mediaId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Fetching media record, {mediaId}, from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            MediaModel? media = null;

            try
            {
                media = await _APIClient.GetMedia(mediaId);

                if (media != null)
                {
                    _Logger.LogMessage(
                            StandardValues.LoggerValues.Info,
                            $"Specifying date times as UTC for media record {media.Id}");

                    media.DateUploaded = DateTime.SpecifyKind(
                        media.DateUploaded,
                        DateTimeKind.Utc);
                    media.DateUpdated = DateTime.SpecifyKind(
                        media.DateUpdated,
                        DateTimeKind.Utc);

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Specified date times as UTC for media record {media.Id}");
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Fetched media record, {mediaId}, from API");
                }

                else
                {
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Failed to fetch media record, {mediaId}, from API");
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Failed to fetch media record, {mediaId}, from API");
            }

            return media;
        }
    }
}
