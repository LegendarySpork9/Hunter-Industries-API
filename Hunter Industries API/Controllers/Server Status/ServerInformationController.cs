// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.ServerStatus;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.ServerStatus
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information")]
    [VersionedRoute("serverstatus/serverinformation", "1.1")]
    public class ServerInformationController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerInformationController(ILoggerService _logger,
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
        /// Returns the collection of servers.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /serverstatus/serverinformation?isActive=true
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="isActive">Whether the servers are active or not.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information.Read")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ServerInformationRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] bool isActive = false)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ServerInformationService _serverInformationService = new ServerInformationService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Get) endpoint called with the following parameters \"{isActive}\".");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { isActive.ToString() });

            List<ServerInformationRecord> servers = await _serverInformationService.GetServers(isActive);

            if (servers.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = servers
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Adds a new server.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /serverstatus/serverinformation
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Test",
        ///         "hostName": "Test",
        ///         "game": "Minecraft",
        ///         "gameVersion": "1.7.10",
        ///         "ipAddress": "127.0.0.1",
        ///         "port": 25565,
        ///         "time": "02:00:00"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the server information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information.Create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a server matching the details already exists for the application.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerInformationRecord), Description = "If the server is successfuly added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ServerInformationModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerInformationService _serverInformationService = new ServerInformationService(_Logger, _FileSystem, _Options, _Database);

            ResponseModel response;

            if (request == null)
            {
                request = new ServerInformationModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true, null, new string[] { "Time" }))
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    username, applicationName, null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _serverInformationService.ServerExists(request.Name))
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("OK"), username, applicationName, ParameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A server with the details provided already exists."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            (bool added, int serverId) = await _serverInformationService.ServerAdded(request);

            if (!added)
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, ParameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Created"), username, applicationName, ParameterFunction.FormatParameters(null, request));

            DowntimeRecord downtime = null;

            if (!string.IsNullOrWhiteSpace(request.Time))
            {
                downtime = new DowntimeRecord()
                {
                    Time = request.Time,
                    Duration = request.Duration
                };
            }

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new ServerInformationRecord()
                {
                    Id = serverId,
                    Name = request.Name,
                    HostName = request.HostName,
                    Game = request.Game,
                    GameVersion = request.GameVersion,
                    Connection = new ConnectionRecord()
                    {
                        IPAddress = request.IPAddress,
                        Port = request.Port,
                    },
                    Downtime = downtime,
                    IsActive = false
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.Created, response.Data);
        }
    }
}
