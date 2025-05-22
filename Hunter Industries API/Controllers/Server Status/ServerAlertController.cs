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
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.ServerStatus
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("ServerStatus")]
    [Route("api/serverstatus/serveralert")]
    public class ServerAlertController : ApiController
    {
        /// <summary>
        /// Returns a collection of the server alerts.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /serverstatus/serveralert
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerAlertResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Get([FromUri] int pageSize = 25, [FromUri] int pageNumber = 1)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ServerAlertService _serverAlertService = new ServerAlertService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (get) endpoint called with the following parameters {_parameterFunction.FormatParameters(new string[] { pageSize.ToString(), pageNumber.ToString() })}.");

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { pageSize.ToString(), pageNumber.ToString() });

            (List<ServerAlertRecord> serverAlerts, int totalAlerts) = _serverAlertService.GetServerAlerts(pageSize, pageNumber);

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

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        ///         "component": "PC Status",
        ///         "componentStatus": "Unknown",
        ///         "alertStatus": "Reported",
        ///         "hostName": "Test",
        ///         "game": "Minecraft",
        ///         "gameVersion": "1.7.10"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the alert.</param>
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ServerAlertRecord), Description = "If the server is successfuly added.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Post([FromBody, Required] ServerAlertModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerAlertService _serverAlertService = new ServerAlertService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new ServerAlertModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            (bool logged, int serverAlertId) = _serverAlertService.LogServerAlert(request);

            if (!logged)
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

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
                    AlertDate = DateTime.Now,
                    server = new RelatedServerRecord()
                    {
                        HostName = request.HostName,
                        Game = request.Game,
                        GameVersion = request.GameVersion
                    }
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [Route("api/serverstatus/serveralert/{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ServerAlertRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no server alert was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Patch(int id, [FromBody, Required] AlertUpdateModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ServerAlertService _serverAlertService = new ServerAlertService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();
            ChangeService _changeService = new ChangeService(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint called with the following parameters \"{id}\", {_parameterFunction.FormatParameters(null, request)}.");

            if (!_modelValidator.IsValid(request))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_serverAlertService.ServerAlertExists(id))
            {
                ServerAlertRecord alertRecord = _serverAlertService.GetServerAlert(id);

                if (_serverAlertService.ServerAlertUpdated(id, request.Status))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"),
                        new string[] { id.ToString(), request.Status });

                    if (!string.IsNullOrEmpty(request.Status) && request.Status != alertRecord.AlertStatus)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("usersettings"), auditID.Item2, "Alert Status", alertRecord.AlertStatus, request.Status);
                        alertRecord.AlertStatus = request.Status;
                    }

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = alertRecord
                    };

                    _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("PATCH"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { id.ToString(), request.Status });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("serverstatus/serveralert"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                new string[] { id.ToString(), request.Status });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No server alert exists with the given id."
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Server Alert (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
