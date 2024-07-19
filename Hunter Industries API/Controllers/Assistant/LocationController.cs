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
    public class LocationController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestLocation([FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            LocationService _locationService = new();

            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantID == null)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"), 
                    new string[] { filters.AssistantName, filters.AssistantID });

                return BadRequest(new
                {
                    error = "Invalid or no filters provided."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the location from the Assistant_Information table.
            LocationResponseModel response = _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantID);

            // Checks if data was returned.
            if (response == new LocationResponseModel())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            return StatusCode(200, response);
        }

        [HttpPatch]
        public IActionResult UpdateLocation([FromBody] LocationModel request, [FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();
            LocationService _locationService = new();
            ChangeService _changeService = new();

            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantID) || (string.IsNullOrWhiteSpace(request.HostName) && string.IsNullOrEmpty(request.IPAddress)))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                return BadRequest(new
                {
                    error = "Filter or body parameters missing."
                });
            }

            // Checks if a config exists.
            if (_configService.AssistantExists(filters.AssistantName, filters.AssistantID))
            {
                LocationResponseModel response = _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantID);

                // Updates the location and returns the result.
                if (_locationService.AssistantLocationUpdated(filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"), 
                        new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                    if (!string.IsNullOrEmpty(request.HostName) && request.HostName != response.HostName)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "Host Name", response.HostName, request.HostName);
                        response.HostName = request.HostName;
                    }

                    if (!string.IsNullOrEmpty(request.IPAddress) && request.IPAddress != response.IPAddress)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "IP Address", response.IPAddress, request.IPAddress);
                        response.IPAddress = request.IPAddress;
                    }

                    return StatusCode(200, response);
                }

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"),
                    _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

            return StatusCode(404, new
            {
                information = "No config exists with the given parameters."
            });
        }
    }
}
