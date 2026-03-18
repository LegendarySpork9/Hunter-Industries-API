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
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.ServerStatus
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Event")]
    [VersionedRoute("serverstatus/serverevent", "1.1")]
    public class ServerEventController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerEventController(ILoggerService _logger,
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
        /// Returns the server events for the given component.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /serverstatus/serverevent?Component=PC Status
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="component">Which component you want the status for.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Event.Read")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ServerEventRecord>), Description = "Returns the item matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] string component)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ServerInformationService _serverInformationService = new ServerInformationService(_Logger, _FileSystem, _Options, _Database);
            ServerEventService _serverEventService = new ServerEventService(_Logger, _FileSystem, _Options, _Database, _serverInformationService);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (get) endpoint called with the following parameters {_parameterFunction.FormatParameters(component)}.");

            if (string.IsNullOrWhiteSpace(component))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverevent"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("BadRequest"), new string[] { component });

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (get) returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverevent"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { component });

            List<ServerEventRecord> serverEvents = await _serverEventService.GetServerEvents(component);

            if (serverEvents.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = serverEvents
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Logs a new server event.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /serverstatus/serverevent
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "component": "PC Status",
        ///         "status": "Online",
        ///         "hostName": "Test",
        ///         "game": "Minecraft",
        ///         "gameVersion": "1.7.10"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the event.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Event.Create")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerEventRecord), Description = "If the server is successfuly added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ServerEventModel request)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerInformationService _serverInformationService = new ServerInformationService(_Logger, _FileSystem, _Options, _Database);
            ServerEventService _serverEventService = new ServerEventService(_Logger, _FileSystem, _Options, _Database, _serverInformationService);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new ServerEventModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverevent"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            (bool logged, int serverEventId) = await _serverEventService.LogServerEvent(request);

            if (!logged)
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverevent"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverevent"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new ServerEventRecord()
                {
                    Component = request.Component,
                    Status = request.Status,
                    DateOccured = _Clock.UtcNow,
                    Server = new RelatedServerRecord()
                    {
                        HostName = request.HostName,
                        Game = request.Game,
                        GameVersion = request.GameVersion
                    }
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Event (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.Created, response.Data);
        }
    }
}
