// Copyright © - 11/06/2026 - Toby Hunter
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
        public ServerInformationController(
            ILoggerService _logger,
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
        [SwaggerOperation("GetServers")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ServerInformationRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] bool isActive = false)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ServerInformationService _serverInformationService = new ServerInformationService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Server Information (Get) endpoint called with the following parameter \"{isActive}\".");

            List<ServerInformationRecord> servers = await _serverInformationService.GetServers(isActive);

            if (servers.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 204,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"IsActive: {isActive}"
                    },
                    null,
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = servers
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"IsActive: {isActive}"
                },
                null,
                ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Returns the server matching the id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /serverstatus/serverinformation/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the server.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information.Read")]
        [VersionedRoute("serverstatus/serverinformation/{id:int}", "2.0")]
        [SwaggerOperation("GetServerById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerInformationRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no server was found matching the given id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ServerInformationService _serverInformationService = new ServerInformationService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Server Information (Get) endpoint called with the following parameter \"{id}\".");

            ServerInformationRecord server = await _serverInformationService.GetServer(id);

            if (server == null)
            {
                response = new ResponseModel()
                {
                    StatusCode = 404,
                    Data = new
                    {
                        error = "No server exists with the given id."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NotFound"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    null,
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = server
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"Id: {id}"
                },
                null,
                ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Server Information (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
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
        ///         "time": "02:00:00",
        ///         "duration": 600
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the server information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information.Create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If a server matching the details already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerInformationRecord), Description = "If the server is successfully added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ServerInformationModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerInformationService _serverInformationService = new ServerInformationService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new ServerInformationModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Server Information (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(
                request,
                true,
                null,
                new string[]
                {
                    "Time",
                    "Duration"
                }))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    null,
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _serverInformationService.ServerExists(request.Name))
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A server with the details provided already exists."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("OK"),
                    username,
                    applicationName,
                    null,
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            (bool added, int serverId) = await _serverInformationService.ServerAdded(request);

            if (!added)
            {
                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    null,
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

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

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("POST"),
                AuditHistoryConverter.GetStatusId("Created"),
                username,
                applicationName,
                null,
                ResponseFunction.GetModelJSON(request),
                ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.Created,
                response.Data);
        }

        /// <summary>
        /// Updates the details of the server.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /serverstatus/serverinformation/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "isActive": "true"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the server.</param>
        /// <param name="request">An object containing the new details.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Information.Update")]
        [VersionedRoute("serverstatus/serverinformation/{id:int}", "2.0")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerInformationRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid or a server exists with the given name.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no server was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(
            int id,
            [FromBody, Required] ServerUpdateModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerInformationService _serverInformationService = new ServerInformationService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            ChangeService _changeService = new ChangeService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Server Information (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _serverInformationService.ServerExists(request.Name))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "A server with the name already exists."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _serverInformationService.ServerExists(id))
            {
                ServerInformationRecord serverRecord = await _serverInformationService.GetServer(id);

                if (await _serverInformationService.ServerUpdated(
                    id,
                    request))
                {
                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = serverRecord
                    };

                    (bool, int) audit = await _auditHistoryService.LogRequest(
                        IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                        AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                        AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                        AuditHistoryConverter.GetMethodId("PATCH"),
                        AuditHistoryConverter.GetStatusId("OK"),
                        username,
                        applicationName,
                        new string[]
                        {
                            $"Id: {id}"
                        },
                        ResponseFunction.GetModelJSON(request),
                        ResponseFunction.GetModelJSON(response.Data));

                    if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != serverRecord.Name)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Server Name",
                            serverRecord.Name,
                            request.Name);
                        serverRecord.Name = request.Name;
                    }

                    if (request.EventInterval != 0 && request.EventInterval != serverRecord.EventInterval)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Event Interval",
                            serverRecord.EventInterval.ToString(),
                            request.EventInterval.ToString());
                        serverRecord.EventInterval = request.EventInterval;
                    }

                    if (request.IsActive.HasValue && request.IsActive.Value != serverRecord.IsActive)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Server Active",
                            serverRecord.IsActive.ToString(),
                            request.IsActive.Value.ToString());
                        serverRecord.IsActive = request.IsActive.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(request.HostName) && request.HostName != serverRecord.HostName)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Server Host Name",
                            serverRecord.HostName,
                            request.HostName);
                        serverRecord.HostName = request.HostName;
                    }

                    if (!string.IsNullOrWhiteSpace(request.Game) && !string.IsNullOrWhiteSpace(request.GameVersion))
                    {
                        if (request.Game != serverRecord.Game)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Game",
                                serverRecord.Game,
                                request.Game);
                            serverRecord.Game = request.Game;
                        }

                        if (request.GameVersion != serverRecord.GameVersion)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Game Version",
                                serverRecord.GameVersion,
                                request.GameVersion);
                            serverRecord.GameVersion = request.GameVersion;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(request.IPAddress) && request.Port != 0)
                    {
                        if (request.IPAddress != serverRecord.Connection.IPAddress)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Ip Address",
                                serverRecord.Connection.IPAddress,
                                request.IPAddress);
                            serverRecord.Connection.IPAddress = request.IPAddress;
                        }

                        if (request.Port != serverRecord.Connection.Port)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Port",
                                serverRecord.Connection.Port.ToString(),
                                request.Port.ToString());
                            serverRecord.Connection.Port = request.Port;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(request.Time) && request.Duration != 0)
                    {
                        DowntimeRecord downtime = serverRecord.Downtime ?? new DowntimeRecord();

                        if (request.Time != downtime.Time)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Downtime",
                                downtime.Time ?? "null",
                                request.Time);
                            downtime.Time = request.Time;
                        }

                        if (request.Duration != downtime.Duration)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "Server Downtime Duration",
                                downtime.Duration == 0 ? "null" : downtime.Duration.ToString(),
                                request.Duration.ToString());
                            downtime.Duration = request.Duration;
                        }

                        serverRecord.Downtime = downtime;
                    }

                    if (request.ClearDowntime.HasValue && serverRecord.Downtime != null)
                    {
                        DowntimeRecord downtime = serverRecord.Downtime;

                        await _changeService.LogChange(
                            audit.Item2,
                            "Server Downtime",
                            downtime.Time,
                            "null");

                        await _changeService.LogChange(
                            audit.Item2,
                            "Server Downtime Duration",
                            downtime.Duration.ToString(),
                            "null");

                        serverRecord.Downtime = null;
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Server Information (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(
                        HttpStatusCode.OK,
                        response.Data);
                }

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an update statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    ResponseFunction.GetModelJSON(request),
                    ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Server Information (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    error = "No server exists with the given id."
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("serverstatus/serverinformation"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("PATCH"),
                AuditHistoryConverter.GetStatusId("NotFound"),
                username,
                applicationName,
                new string[]
                {
                    $"Id: {id}"
                },
                ResponseFunction.GetModelJSON(request),
                ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Server Information (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }
    }
}
