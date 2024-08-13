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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestVersion([FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            VersionService _versionService = new();

            ResponseModel response = new();
            bool responseGiven = false;

            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantId == null)
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
                responseGiven = true;
            }

            if (!responseGiven)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId });

                // Gets the version from the Assistant_Information table.
                VersionResponseModel versionResponse = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantId);

                // Checks if data was returned.
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
                }

                else
                {
                    response = new()
                    {
                        StatusCode = 200,
                        Data = versionResponse
                    };
                }
            }

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpPatch]
        public IActionResult UpdateVersion([FromBody] VersionModel request, [FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();
            VersionService _versionService = new();
            ChangeService _changeService = new();

            ResponseModel response = new();
            bool responseGiven = false;

            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantId) || string.IsNullOrWhiteSpace(request.Version))
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
                responseGiven = true;
            }

            // Checks if a config exists.
            if (!responseGiven && _configService.AssistantExists(filters.AssistantName, filters.AssistantId))
            {
                VersionResponseModel versionResponse = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantId);

                // Updates the version and returns the result.
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
                }

                else
                {
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
                }
            }

            if (!responseGiven)
            {
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
            }

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
