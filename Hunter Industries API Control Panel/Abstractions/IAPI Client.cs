// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Requests;
using HunterIndustriesAPIControlPanel.Models.Requests.Patch;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;

namespace HunterIndustriesAPIControlPanel.Abstractions
{
    /// <summary>
    /// Interface for the API.
    /// </summary>
    public interface IAPIClient
    {
        public void SetBearerToken(string bearerToken);
        Task<AuthenticationModel?> Authorise();
        Task<List<UserModel>> GetUsers(bool includeDeleted);
        Task<DashboardStatisticsModel?> GetDashboardStatistics();
        Task<PagedAPIResponseModel<AuditHistoryModel>?> GetPagedAuditHistory(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<(UserModel?, ResponseModel?)> CreateUser(UserRequestModel user);
        Task<bool> DeleteUser(int userId);
        Task<UserModel?> GetUser(int userId);
        Task<T?> GetConfigurationEntity<T>(string entity, int entityId);
        Task<SharedStatisticsModel?> GetLogStatistics(string entity, int entityId);
        Task<List<UserSettingModel>> GetUserSettings(int userId);
        Task<T?> GetPagedConfiguration<T>(string entity, List<KeyValuePair<string, object>>? queryParameters = null, bool ignoreQuery = true);
        Task<(UserModel?, ResponseModel?)> UpdateUser(int userId, UserRequestModel user);
        Task<(UserSettingModel?, ResponseModel?)> CreateUserSetting(UserSettingRequestModel userSetting);
        Task<(SettingModel?, ResponseModel?)> UpdateUserSetting(int userSettingId, UserSettingUpdateRequestModel updateUserSetting);
        Task<List<ServerInformationModel>> GetServers();
        Task<(ServerInformationModel?, ResponseModel?)> CreateServer(ServerRequestModel server);
        Task<(ServerInformationModel?, ResponseModel?)> UpdateServer(int serverId, ServerUpdateRequestModel server);
        Task<ServerInformationModel?> GetServer(int serverId);
        Task<ServerStatisticsModel?> GetServerStatistics(int serverId);
        Task<ConfigurationModel?> GetConfiguration();
        Task<(T1?, ResponseModel?)> CreateConfigurationEntity<T1, T2>(string entity, T2 entityObject, List<KeyValuePair<string, object>>? queryParameters = null);
        Task<bool> DeleteConfigurationEntity(string entity, int entityId);
        Task<(T1?, ResponseModel?)> UpdateConfigurationEntity<T1, T2>(string entity, int entityId, T2 entityObject);
        Task<ErrorStatisticsModel?> GetErrorStatistics();
        Task<PagedAPIResponseModel<ErrorModel>?> GetPagedErrorLog(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<ErrorModel?> GetError(int errorId);
    }
}
