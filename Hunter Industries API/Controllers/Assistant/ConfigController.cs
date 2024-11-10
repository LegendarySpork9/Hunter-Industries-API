using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Objects.Assistant;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Assistant;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("AIAccess")]
    [Route("api/assistant/config")]
    public class ConfigController : ApiController
    {
        /// <summary>
        /// Returns a collection of assistant configurations.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /assistant/config?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ConfigResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Get([FromUri] AssistantFilterModel filters)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ConfigService _configService = new ConfigService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (filters == null)
            {
                filters = new AssistantFilterModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint called with the following parameters {_parameterFunction.FormatParameters(filters)}.");

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, filters));

            var result = _configService.GetAssistantConfig(filters.AssistantName, filters.AssistantId);
            List<AssistantConfiguration> assistantConfigurations = result.Item1;

            if (assistantConfigurations.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ConfigResponseModel()
                {
                    LatestRelease = result.Item3,
                    AssistantConfigurations = assistantConfigurations,
                    ConfigCount = assistantConfigurations.Count,
                    TotalCount = result.Item2,
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Creates a new assistant configuration.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /assistant/config
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "AssistantName": "Test",
        ///         "IDNumber": "TST 1456-4",
        ///         "AssignedUser": "Tester",
        ///         "HostName": "PlaceHolder"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the basic assistant configuration information.</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a configuration matching the name and id number already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(AssistantConfiguration), Description = "If the configuration is successfuly created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Post([FromBody, Required] ConfigModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ConfigService _configService = new ConfigService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new ConfigModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_configService.AssistantExists(request.AssistantName, request.IdNumber))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A config with the name and/or ID already exists."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            if (!_configService.AssistantConfigCreated(request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

            string version = _configService.GetMostRecentVersion();

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new AssistantConfiguration()
                {
                    AssistantName = request.AssistantName,
                    IdNumber = request.IdNumber,
                    AssignedUser = request.AssignedUser,
                    HostName = request.HostName,
                    Deletion = false,
                    Version = version,
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.Created, response.Data);
        }
    }
}
