// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
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

            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantID == null)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"), 
                    new string[] { filters.AssistantName, filters.AssistantID });

                return BadRequest(new
                {
                    error = "Invalid or no filters provided."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the version from the Assistant_Information table.
            VersionResponseModel response = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantID);

            // Checks if data was returned.
            if (response == new VersionResponseModel())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            return StatusCode(200, response);
        }

        [HttpPatch]
        public IActionResult UpdateVersion([FromBody] VersionModel request, [FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();
            VersionService _versionService = new();
            ChangeService _changeService = new();

            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantID) || string.IsNullOrWhiteSpace(request.Version))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                return BadRequest(new
                {
                    error = "Filter or body parameters missing."
                });
            }

            // Checks if a config exists.
            if (_configService.AssistantExists(filters.AssistantName, filters.AssistantID))
            {
                VersionResponseModel response = _versionService.GetAssistantVersion(filters.AssistantName, filters.AssistantID);

                // Updates the version and returns the result.
                if (_versionService.AssistantVersionUpdated(filters.AssistantName, filters.AssistantID, request.Version))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"),
                        new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                    if (request.Version != response.Version)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/version"), auditID.Item2, "Version", response.Version, request.Version);
                    }

                    response.Version = request.Version;

                    return StatusCode(200, response);
                }

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"),
                    _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/version"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Version });

            return StatusCode(404, new
            {
                information = "No config exists with the given parameters."
            });
        }
    }
}
