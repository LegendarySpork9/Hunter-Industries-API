// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Assistant.Config")]
    [VersionedRoute("assistant/config", "1.0")]
    public class ConfigController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ConfigController(ILoggerService _logger,
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
        /// Returns a collection of assistant configurations.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /assistant/config?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Assistant.Config.Read")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ConfigResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] AssistantFilterModel filters)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ConfigService _configService = new ConfigService(_Logger, _FileSystem, _Options, _Database);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (filters == null)
            {
                filters = new AssistantFilterModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint called with the following parameters {_parameterFunction.FormatParameters(filters)}.");

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, filters));

            var result = await _configService.GetAssistantConfig(filters.AssistantName, filters.AssistantId);
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

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [RequiredPolicyAuthorisationAttributeFilter("Assistant.Config.Create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a configuration matching the name and id number already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(AssistantConfiguration), Description = "If the configuration is successfuly created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ConfigModel request)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ConfigService _configService = new ConfigService(_Logger, _FileSystem, _Options, _Database);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new ConfigModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _configService.AssistantExists(request.AssistantName, request.IdNumber))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A config with the name and/or ID already exists."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            if (!await _configService.AssistantConfigCreated(request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

            string version = await _configService.GetMostRecentVersion();

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

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.Created, response.Data);
        }
    }
}
