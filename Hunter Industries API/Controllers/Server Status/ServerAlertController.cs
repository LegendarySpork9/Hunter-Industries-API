// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.ServerStatus;
using Swashbuckle.Swagger.Annotations;
using System;
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
    [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Alert")]
    [VersionedRoute("serverstatus/serveralert", "1.1")]
    public class ServerAlertController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerAlertController(ILoggerService _logger,
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
        /// Returns a collection of the server alerts.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /serverstatus/serveralert
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Alert.Read")]
        [SwaggerOperation("GetServerAlert")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerAlertResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] int pageSize = 25, [FromUri] int pageNumber = 1)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ServerAlertService _serverAlertService = new ServerAlertService(_Logger, _FileSystem, _Options, _Database);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(new string[] { pageSize.ToString(), pageNumber.ToString() })}.");

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { pageSize.ToString(), pageNumber.ToString() });

            (List<ServerAlertRecord> serverAlerts, int totalAlerts) = await _serverAlertService.GetServerAlerts(pageSize, pageNumber);

            if (serverAlerts.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            int totalPages = (int)Math.Ceiling((decimal)totalAlerts / (decimal)pageSize);

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ServerAlertResponseModel()
                {
                    Entries = serverAlerts,
                    EntryCount = serverAlerts.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPageCount = totalPages,
                    TotalCount = totalAlerts
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the server alert matching the id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /serverstatus/serveralert
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the server alert.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Alert.Read")]
        [VersionedRoute("serverstatus/serveralert/{id:int}", "1.1")]
        [SwaggerOperation("GetServerAlertById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerAlertRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ServerAlertService _serverAlertService = new ServerAlertService(_Logger, _FileSystem, _Options, _Database);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint called with the following parameters \"{id}\".");

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { id.ToString() });

            ServerAlertRecord serverAlert = await _serverAlertService.GetServerAlert(id);

            if (serverAlert.AlertId == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = serverAlert
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Logs a new server alert.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /serverstatus/serveralert
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "reporter": "tester",
        ///         "component": "PC",
        ///         "componentStatus": "Unknown",
        ///         "alertStatus": "Reported",
        ///         "serverId": 1,
        ///         "name": "Test",
        ///         "hostName": "Test",
        ///         "game": "Minecraft",
        ///         "gameVersion": "1.7.10"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the alert.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Alert.Create")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerAlertRecord), Description = "If the server is successfuly added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ServerAlertModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerAlertService _serverAlertService = new ServerAlertService(_Logger, _FileSystem, _Options, _Database);

            ResponseModel response;

            if (request == null)
            {
                request = new ServerAlertModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            (bool logged, int serverAlertId) = await _serverAlertService.LogServerAlert(request);

            if (!logged)
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"), ParameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Created"), ParameterFunction.FormatParameters(null, request));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new ServerAlertRecord()
                {
                    AlertId = serverAlertId,
                    Reporter = request.Reporter,
                    Component = request.Component,
                    ComponentStatus = request.ComponentStatus,
                    AlertStatus = request.AlertStatus,
                    AlertDate = _Clock.UtcNow,
                    server = new RelatedServerRecord()
                    {
                        Id = request.ServerId,
                        Name = request.Name,
                        HostName = request.HostName,
                        Game = request.Game,
                        GameVersion = request.GameVersion
                    }
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.Created, response.Data);
        }

        /// <summary>
        /// Updates the status of the alert.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /serverstatus/serveralert/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "status": "Resolved"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the server alert.</param>
        /// <param name="request">An object containing the new status value.</param>
        [RequiredPolicyAuthorisationAttributeFilter("ServerStatus.Alert.Update")]
        [VersionedRoute("serverstatus/serveralert/{id:int}", "1.1")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerAlertRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no server alert was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(int id, [FromBody, Required] AlertUpdateModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerAlertService _serverAlertService = new ServerAlertService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(null, request)}.");

            if (!_modelValidator.IsValid(request))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _serverAlertService.ServerAlertExists(id))
            {
                ServerAlertRecord alertRecord = await _serverAlertService.GetServerAlert(id);

                if (await _serverAlertService.ServerAlertUpdated(id, request.Status))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                        new string[] { id.ToString(), request.Status });

                    if (!string.IsNullOrEmpty(request.Status) && request.Status != alertRecord.AlertStatus)
                    {
                        await _changeService.LogChange(AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), audit.Item2, "Alert Status", alertRecord.AlertStatus, request.Status);
                        alertRecord.AlertStatus = request.Status;
                    }

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = alertRecord
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("PATCH"),
                        AuditHistoryConverter.GetStatusID("InternalServerError"), new string[] { id.ToString(), request.Status });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("serverstatus/serveralert"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                new string[] { id.ToString(), request.Status });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No server alert exists with the given id."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
