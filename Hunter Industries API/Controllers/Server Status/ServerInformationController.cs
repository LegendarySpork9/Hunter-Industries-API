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
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.ServerStatus
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("ServerStatus")]
    [Route("api/serverstatus/serverinformation")]
    public class ServerInformationController : ApiController
    {
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ServerInformationRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Get([FromUri] bool isActive = false)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ServerInformationService _serverInformationService = new ServerInformationService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (get) endpoint called with the following parameters \"{isActive}\".");

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { isActive.ToString() });

            List<ServerInformationRecord> servers = _serverInformationService.GetServers(isActive);

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

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = servers
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        ///         "hostName": "Test",
        ///         "game": "Minecraft",
        ///         "gameVersion": "1.7.10",
        ///         "ipAddress": "127.0.0.1"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the server information.</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a server matching the details already exists for the application.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerInformationRecord), Description = "If the server is successfuly added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Post([FromBody, Required] ServerInformationModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerInformationService _serverInformationService = new ServerInformationService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new ServerInformationModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_serverInformationService.ServerExists(request.HostName, request.Game, request.GameVersion))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A server with the details provided already exists."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            (bool added, int serverId) = _serverInformationService.ServerAdded(request);

            if (!added)
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serverinformation"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new ServerInformationRecord()
                {
                    Id = serverId,
                    HostName = request.HostName,
                    Game = request.Game,
                    GameVersion = request.GameVersion,
                    IPAddress = request.IPAddress,
                    IsActive = false
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Information (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.Created, response.Data);
        }
    }
}
