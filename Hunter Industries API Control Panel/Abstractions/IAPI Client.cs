// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Requests;
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
        Task<ApplicationModel?> GetApplication(int applicationId);
        Task<SharedStatisticsModel?> GetLogStatistics(string entity, int entityId);
        Task<List<UserSettingModel>> GetUserSettings(int userId);
        Task<PagedAPIResponseModel<ApplicationModel>?> GetPagedApplication(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<(UserModel?, ResponseModel?)> UpdateUser(int userId, UserRequestModel user);
        Task<(UserSettingModel?, ResponseModel?)> CreateUserSetting(UserSettingRequestModel userSetting);
        Task<(SettingModel?, ResponseModel?)> UpdateUserSetting(int userSettingId, UserSettingUpdateRequestModel updateUserSetting);
        Task<List<ServerInformationModel>> GetServers();
        Task<PagedAPIResponseModel<MachineModel>?> GetPagedMachine(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<PagedAPIResponseModel<GameModel>?> GetPagedGame(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<PagedAPIResponseModel<ConnectionModel>?> GetPagedConnection(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<PagedAPIResponseModel<DowntimeModel>?> GetPagedDowntime(List<KeyValuePair<string, object>>? queryParameters = null);
        Task<(ServerInformationModel?, ResponseModel?)> CreateServer(ServerRequestModel server);
        Task<(ServerInformationModel?, ResponseModel?)> UpdateServer(int serverId, ServerUpdateRequestModel server);
        Task<ServerInformationModel?> GetServer(int serverId);
        Task<ServerStatisticsModel?> GetServerStatistics(int serverId);
        Task<ConfigurationModel?> GetConfiguration();
    }
}
