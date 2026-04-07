// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Statistics;
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using HunterIndustriesAPI.Objects.Statistics.Error;
using HunterIndustriesAPI.Objects.Statistics.Server;
using HunterIndustriesAPI.Objects.Statistics.Shared;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Statistic")]
    public class StatisticController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public StatisticController(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabase _database,
            IDatabaseOptions _options,
            IClock _clock)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Database = _database;
            _Options = _options;
            _Clock = _clock;
        }

        /// <summary>
        /// Returns the statistics for the dashboard.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /statistic/dashboard
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Statistic.Read")]
        [VersionedRoute("statistic/dashboard", "2.0")]
        [SwaggerOperation("GetStatisticDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardResponseModel), Description = "Returns the statistics for the dashboard.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> GetDashboard()
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            StatisticService _statisticService = new StatisticService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("statistic"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName);

            List<object> records = await _statisticService.GetDashboardStatistic("topBarStats");
            TopBarStatRecord topBarStatsRecord = records[0] as TopBarStatRecord;

            records = await _statisticService.GetDashboardStatistic("apiTraffic");
            List<APITrafficRecord> apiTrafficRecords = records.Cast<APITrafficRecord>().ToList();

            records = await _statisticService.GetDashboardStatistic("errors");
            List<IPAndSummaryErrorRecord> errorRecords = records.Cast<IPAndSummaryErrorRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("endpointCalls");
            List<EndpointCallRecord> endpointCallRecords = records.Cast<EndpointCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("methodCalls");
            List<MethodCallRecord> methodCallRecords = records.Cast<MethodCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("statusCalls");
            List<StatusCallRecord> statusCallRecords = records.Cast<StatusCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("changeCalls");
            List<ChangeCallRecord> changeRecords = records.Cast<ChangeCallRecord>().ToList();

            records = await _statisticService.GetDashboardStatistic("loginAttempts");
            List<LoginAttemptStatisticRecord> loginAttemptRecords = records.Cast<LoginAttemptStatisticRecord>().ToList();

            records = await _statisticService.GetDashboardStatistic("serverHealthOverview");
            List<ServerHealthOverviewRecord> serverHealthOverviewRecords = records.Cast<ServerHealthOverviewRecord>().ToList();

            records = await _statisticService.GetDashboardStatistic("serverHealthUptime");
            List<ServerHealthOverviewRecord> serverHealthUptimeRecords = records.Cast<ServerHealthOverviewRecord>().ToList();

            List<ServerHealthOverviewRecord> serverHealthRecords = serverHealthOverviewRecords.Join(serverHealthUptimeRecords, overview => overview.ServerId, uptime => uptime.ServerId, (overview, uptime) => new ServerHealthOverviewRecord
            {
                ServerId = overview.ServerId,
                Name = uptime.Name,
                Uptime = uptime.Uptime,
                Events = overview.Events,
                Alerts = overview.Alerts
            }).ToList();

            ResponseModel response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new DashboardResponseModel()
                {
                    Metric = topBarStatsRecord,
                    APITraffic = apiTrafficRecords,
                    Errors = errorRecords,
                    EndpointCalls = endpointCallRecords,
                    MethodCalls = methodCallRecords,
                    StatusCalls = statusCallRecords,
                    Changes = changeRecords,
                    LoginAttempts = loginAttemptRecords,
                    ServerHealth = serverHealthRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the statistics for the server page.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /statistic/server/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Statistic.Read")]
        [VersionedRoute("statistic/server/{id:int}", "2.0")]
        [SwaggerOperation("GetStatisticServer")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerResponseModel), Description = "Returns the statistics for the server page.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> GetServer(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            StatisticService _statisticService = new StatisticService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("statistic"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { id.ToString() });

            List<object> records = await _statisticService.GetServerStatistic("componentAlerts", id);
            List<AlertComponentRecord> alertComponentRecords = records.Cast<AlertComponentRecord>().ToList();

            records = await _statisticService.GetServerStatistic("statusAlerts", id);
            List<AlertStatusRecord> alertStatusRecords = records.Cast<AlertStatusRecord>().ToList();

            records = await _statisticService.GetServerStatistic("lastComponentEvents", id);
            List<EventComponentRecord> latestEventComponentRecords = records.Cast<EventComponentRecord>().ToList();

            records = await _statisticService.GetServerStatistic("recentAlerts", id);
            List<RecentAlertRecord> recentAlertRecords = records.Cast<RecentAlertRecord>().ToList();

            records = await _statisticService.GetServerStatistic("recentEvents", id);
            List<EventComponentRecord> recentEventRecords = records.Cast<EventComponentRecord>().ToList();

            ResponseModel response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ServerResponseModel()
                {
                    ComponentAlerts = alertComponentRecords,
                    StatusAlerts = alertStatusRecords,
                    LatestEvents = latestEventComponentRecords,
                    RecentAlerts = recentAlertRecords,
                    RecentEvents = recentEventRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the statistics for the error page.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /statistic/error
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Statistic.Read")]
        [VersionedRoute("statistic/error", "2.0")]
        [SwaggerOperation("GetStatisticError")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ErrorResponseModel), Description = "Returns the statistics for the dashboard.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> GetError()
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            StatisticService _statisticService = new StatisticService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("statistic"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName);

            List<object> records = await _statisticService.GetErrorStatistic("errorsOverTime");
            List<ErrorOverTimeRecord> errorOverTimeRecords = records.Cast<ErrorOverTimeRecord>().ToList();

            records = await _statisticService.GetErrorStatistic("ipErrors");
            List<IPErrorRecord> ipErrorRecords = records.Cast<IPErrorRecord>().ToList();

            records = await _statisticService.GetErrorStatistic("summaryErrors");
            List<SummaryErrorRecord> summaryErrorRecords = records.Cast<SummaryErrorRecord>().ToList();

            ResponseModel response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ErrorResponseModel()
                {
                    Errors = errorOverTimeRecords,
                    IPErrors = ipErrorRecords,
                    SummaryErrors = summaryErrorRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the statistics for the application page.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /statistic/application/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Statistic.Read")]
        [VersionedRoute("statistic/application/{id:int}", "2.0")]
        [SwaggerOperation("GetStatisticDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedStatResponseModel), Description = "Returns the statistics for the application page.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> GetApplication(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            StatisticService _statisticService = new StatisticService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("statistic"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { id.ToString() });

            List<object> records = await _statisticService.GetSharedStatistic("endpointCalls", "application", id);
            List<EndpointCallRecord> endpointCallRecords = records.Cast<EndpointCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("methodCalls", "application", id);
            List<MethodCallRecord> methodCallRecords = records.Cast<MethodCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("statusCalls", "application", id);
            List<StatusCallRecord> statusCallRecords = records.Cast<StatusCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("changeCalls", "application", id);
            List<ChangeCallRecord> changeRecords = records.Cast<ChangeCallRecord>().ToList();

            ResponseModel response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new SharedStatResponseModel()
                {
                    EndpointCalls = endpointCallRecords,
                    MethodCalls = methodCallRecords,
                    StatusCalls = statusCallRecords,
                    Changes = changeRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the statistics for the user page.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /statistic/user/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Statistic.Read")]
        [VersionedRoute("statistic/user/{id:int}", "2.0")]
        [SwaggerOperation("GetStatisticDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedStatResponseModel), Description = "Returns the statistics for the user page.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            StatisticService _statisticService = new StatisticService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("statistic"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { id.ToString() });

            List<object> records = await _statisticService.GetSharedStatistic("endpointCalls", "user", id);
            List<EndpointCallRecord> endpointCallRecords = records.Cast<EndpointCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("methodCalls", "user", id);
            List<MethodCallRecord> methodCallRecords = records.Cast<MethodCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("statusCalls", "user", id);
            List<StatusCallRecord> statusCallRecords = records.Cast<StatusCallRecord>().ToList();

            records = await _statisticService.GetSharedStatistic("changeCalls", "user", id);
            List<ChangeCallRecord> changeRecords = records.Cast<ChangeCallRecord>().ToList();

            ResponseModel response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new SharedStatResponseModel()
                {
                    EndpointCalls = endpointCallRecords,
                    MethodCalls = methodCallRecords,
                    StatusCalls = statusCallRecords,
                    Changes = changeRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Statistic (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }
    }
}
