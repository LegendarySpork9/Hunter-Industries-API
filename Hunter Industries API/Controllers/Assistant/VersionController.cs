// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Assistant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Assistant")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// Returns the version of an assistant.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /assistant/version?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <response code="200">Returns the assistant version or nothing.</response>
        /// <response code="400">If the filters are invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        [HttpGet]
        [MakeFiltersRequired]
        [ProducesResponseType(typeof(VersionResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult RequestVersion([FromQuery] AssistantFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            VersionService _versionService = new(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Get) endpoint called with the following parameters {_logger.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"), 
                    new string[] { filters.AssistantName, filters.AssistantId });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Get) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId });

            VersionResponseModel versionResponse = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantId);

            if (versionResponse == new VersionResponseModel())
            {
                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Get) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            response = new()
            {
                StatusCode = 200,
                Data = versionResponse
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Get) endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
        }

        /// <summary>
        /// Updates the version for an assistant.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /assistant/version?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "version": "1.0.0.0"
        ///     }
        /// </remarks>
        /// <response code="200">If the version is updated.</response>
        /// <response code="400">If the body or filters are invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        /// <response code="404">If no configuration was found using the filters.</response>
        /// <response code="500">If something went wrong on the server.</response>
        [HttpPatch]
        [MakeFiltersRequired]
        [ProducesResponseType(typeof(VersionResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult UpdateVersion([FromBody] VersionModel request, [FromQuery] AssistantFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            ConfigService _configService = new(_logger);
            VersionService _versionService = new(_logger);
            ChangeService _changeService = new(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Patch) endpoint called with the following parameters {_logger.FormatParameters(request)}, {_logger.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(request) || !_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.Version });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Filter or body parameters missing."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Patch) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            if (_configService.AssistantExists(filters.AssistantName, filters.AssistantId))
            {
                VersionResponseModel versionResponse = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantId);

                if (_versionService.AssistantVersionUpdated(filters.AssistantName, filters.AssistantId, request.Version))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"),
                        new string[] { filters.AssistantName, filters.AssistantId, request.Version });

                    if (request.Version != versionResponse.Version)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/version"), auditID.Item2, "Version", versionResponse.Version, request.Version);
                    }

                    versionResponse.Version = request.Version;

                    response = new()
                    {
                        StatusCode = 200,
                        Data = versionResponse
                    };

                    _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Patch) endpoint returned a {response.StatusCode} with the data {response.Data}");
                    return StatusCode(response.StatusCode, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantId, request.Version });

                response = new()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Patch) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.Version });

            response = new()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No config exists with the given parameters."
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Version (Patch) endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
