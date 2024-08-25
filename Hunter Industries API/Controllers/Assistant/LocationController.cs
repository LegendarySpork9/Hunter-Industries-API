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
    public class LocationController : ControllerBase
    {
        /// <summary>
        /// Returns the location details of an assistants.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /assistant/location?AssistantName=Test&amp;AssistantID=TST 1456-4
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <response code="200">Returns the assistant location details or nothing.</response>
        /// <response code="400">If the filters are invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        [HttpGet]
        [ProducesResponseType(typeof(LocationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult RequestLocation([FromQuery] AssistantFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            LocationService _locationService = new();

            ResponseModel response = new();

            // Checks if the request contains the needed filters.
            if (!_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"), 
                    new string[] { filters.AssistantName, filters.AssistantId });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId });

            // Gets the location from the Assistant_Information table.
            LocationResponseModel locationResponse = _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantId);

            // Checks if data was returned.
            if (locationResponse == new LocationResponseModel())
            {
                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            response = new()
            {
                StatusCode = 200,
                Data = locationResponse
            };

            return StatusCode(response.StatusCode, response.Data);
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
        /// <response code="200">If the location details are updated.</response>
        /// <response code="400">If the body or filters are invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        /// <response code="404">If no configuration was found using the filters.</response>
        /// <response code="500">If something went wrong on the server.</response>
        [HttpPatch]
        [ProducesResponseType(typeof(LocationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult UpdateLocation([FromBody] LocationModel request, [FromQuery] AssistantFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            ConfigService _configService = new();
            LocationService _locationService = new();
            ChangeService _changeService = new();

            ResponseModel response = new();

            // Checks whether all requireds are present.
            if (!_modelValidator.IsValid(request) || !_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Filter or body parameters missing."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            // Checks if a config exists.
            if (_configService.AssistantExists(filters.AssistantName, filters.AssistantId))
            {
                LocationResponseModel locationResponse = _locationService.GetAssistantLocation(filters.AssistantName, filters.AssistantId);

                // Updates the location and returns the result.
                if (_locationService.AssistantLocationUpdated(filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"), 
                        new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

                    if (!string.IsNullOrEmpty(request.HostName) && request.HostName != locationResponse.HostName)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "Host Name", locationResponse.HostName, request.HostName);
                        locationResponse.HostName = request.HostName;
                    }

                    if (!string.IsNullOrEmpty(request.IPAddress) && request.IPAddress != locationResponse.IPAddress)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "IP Address", locationResponse.IPAddress, request.IPAddress);
                        locationResponse.IPAddress = request.IPAddress;
                    }

                    response = new()
                    {
                        StatusCode = 200,
                        Data = locationResponse
                    };

                    return StatusCode(response.StatusCode, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

                response = new()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/location"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.HostName, request.IPAddress });

            response = new()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No config exists with the given parameters."
                }
            };

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
