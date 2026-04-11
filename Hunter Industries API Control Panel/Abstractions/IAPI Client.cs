// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;

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
    }
}
