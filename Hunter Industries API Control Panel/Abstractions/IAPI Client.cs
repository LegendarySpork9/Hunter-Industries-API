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
        Task<UserModel?> CreateUser(UserRequestModel user);
        Task<bool> DeleteUser(int userId);
        Task<UserModel?> GetUser(int userId);
        Task<ApplicationModel?> GetApplication(int applicationId);
        Task<SharedStatisticsModel?> GetLogStatistics(string entity, int entityId);
        Task<List<UserSettingModel>> GetUserSettings(int userId);
        Task<PagedAPIResponseModel<ApplicationModel>?> GetPagedApplication();
        Task<UserModel?> UpdateUser(int userId, UserRequestModel user);
        Task<UserSettingModel?> CreateUserSetting(UserSettingRequestModel userSetting);
        Task<SettingModel?> UpdateUserSetting(int userSettingId, string value);
    }
}
