// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Filters.Operation;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Assistant;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Assistant.Location")]
    [VersionedRoute("assistant/location", "1.0")]
    public class LocationController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public LocationController(ILoggerService _logger,
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
        /// Returns the location details of an assistants.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /assistant/location?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Assistant.Location.Read")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LocationResponseModel), Description = "Returns the item matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            LocationService _locationService = new LocationService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunctions.GetUsername(principal);
            string applicationName = ClaimFunctions.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new AssistantFilterModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters, true))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"), username, applicationName, ParameterFunction.FormatParameters(null, filters));

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"), username, applicationName, ParameterFunction.FormatParameters(null, filters));

            LocationResponseModel locationResponse = await _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantId);

            if (locationResponse == new LocationResponseModel())
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = locationResponse
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Updates the location details for an assistant.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /assistant/location?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "HostName": "Test"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the location data.</param>
        /// <param name="filters">An object containing the fitler to apply the value to.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Assistant.Location.Update")]
        [MakeFiltersRequired]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LocationResponseModel), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no configuration was found using the filters.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch([FromBody] LocationModel request, [FromUri] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ConfigService _configService = new ConfigService(_Logger, _FileSystem, _Options, _Database);
            LocationService _locationService = new LocationService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunctions.GetUsername(principal);
            string applicationName = ClaimFunctions.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new AssistantFilterModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Patch) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}, {ParameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(request) || !_modelValidator.IsValid(filters, true))
            {
                if (request == null)
                {
                    await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                        username, applicationName, new string[] { filters.AssistantName, filters.AssistantId, null });
                }

                else
                {
                    await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                        username, applicationName, new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });
                }

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Filter or body parameters missing."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _configService.AssistantExists(filters.AssistantName, filters.AssistantId))
            {
                LocationResponseModel locationResponse = await _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantId);

                if (await _locationService.AssistantLocationUpdated(filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                        username, applicationName, new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

                    if (!string.IsNullOrEmpty(request.HostName) && request.HostName != locationResponse.HostName)
                    {
                        await _changeService.LogChange(audit.Item2, "Host Name", locationResponse.HostName, request.HostName);
                        locationResponse.HostName = request.HostName;
                    }

                    if (!string.IsNullOrEmpty(request.IPAddress) && request.IPAddress != locationResponse.IPAddress)
                    {
                        await _changeService.LogChange(audit.Item2, "IP Address", locationResponse.IPAddress, request.IPAddress);
                        locationResponse.IPAddress = request.IPAddress;
                    }

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = locationResponse
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"),
                        AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                    username, applicationName, new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No config exists with the given parameters."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Location (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
