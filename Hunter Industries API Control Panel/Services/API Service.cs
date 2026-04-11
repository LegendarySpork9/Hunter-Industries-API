// Copyright © - 05/10/2025 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Models.Responses;
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
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtaining Bearer Token from API");

            try
            {
                AuthenticationModel? auth = await _APIClient.Authorise();

                if (auth != null)
                {
                    _APIClient.SetBearerToken(auth.Token);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {auth.Token}");

                    ExpiryTime = DateTime.SpecifyKind(auth.Info.Expires, DateTimeKind.Utc);

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Expiry Time: {ExpiryTime}");
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Obtained Bearer Token from API");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to Obtain Bearer Token from API");
            }
        }

        /// <summary>
        /// Gets a list of the users from the API.
        /// </summary>
        public async Task<List<UserModel>> GetUsers(bool includeDeleted)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching users from API");

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
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch users from API");
            }

            return users;
        }

        /// <summary>
        /// Gets the dashboard statistics from the API.
        /// </summary>
        public async Task<DashboardStatisticsModel?> GetDashboardStatistics()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching dashboard statistics from API");

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
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched dashboard statistics from API");
                }

                else
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch dashboard statistics from API");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch dashboard statistics from API");
            }

            return dashboardStatistics;
        }

        /// <summary>
        /// Gets the audit histories from the API matching the given parameters.
        /// </summary>
        public async Task<List<AuditHistoryModel>> GetAuditHistories(string? fromDate = null, string? toDate = null, string? ipAddress = null, string? endpoint = null, string? username = null, string? application = null, int pageSize = 25, int pageNumber = 1)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetching audit history records from API");

            if (ExpiryTime < _Clock.UtcNow)
            {
                await Authorise();
            }

            List<AuditHistoryModel> auditHistories = [];

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
                PagedAPIResponseModel<AuditHistoryModel>? pagedResponse = await _APIClient.GetPagedAuditHistory(queryParameters);

                if (pagedResponse != null)
                {
                    auditHistories = pagedResponse.Entries;

                    _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Audit Histories Returned: {auditHistories.Count}");

                    foreach (AuditHistoryModel auditHistory in auditHistories)
                    {
                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Specifying date times as UTC for audit history {auditHistory.Id}");

                        auditHistory.OccuredAt = DateTime.SpecifyKind(auditHistory.OccuredAt, DateTimeKind.Utc);

                        _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Specified date times as UTC for audit history {auditHistory.Id}");
                    }

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Fetched audit history records from API");
                }

                else
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch audit history records from API");
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Failed to fetch audit history records from API");
            }

            return auditHistories;
        }
    }
}
