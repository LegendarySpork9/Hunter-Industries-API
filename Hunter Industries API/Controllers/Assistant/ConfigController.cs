// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Objects.Assistant;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Assistant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
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
        /// <response code="200">Returns the assistant configuration collection or nothing.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ConfigResponseModel), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult RequestConfig([FromQuery] AssistantFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint called with the following parameters {_logger.FormatParameters(filters)}.");

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId });

            var result = _configService.GetAssistantConfig(filters.AssistantName, filters.AssistantId);
            List<AssistantConfiguration> assistantConfigurations = result.Item1;

            if (assistantConfigurations.Count == 0)
            {
                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            response = new()
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

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Get) endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
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
        ///         "IDNumber": "TST 1419-9",
        ///         "AssignedUser": "Tester",
        ///         "HostName": "PlaceHolder"
        ///     }
        /// </remarks>
        /// <response code="200">If the a configuration matching the name and id number already exists.</response>
        /// <response code="201">If the configuration is successfuly created.</response>
        /// <response code="400">If the body is invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        /// <response code="500">If something went wrong on the server.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AssistantConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult CreateConfig([FromBody, Required] ConfigModel request)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            ConfigService _configService = new(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint called with the following parameters {_logger.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            if (_configService.AssistantExists(request.AssistantName, request.IdNumber))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), 
                    new string[] { request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName });

                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A config with the name and/or ID already exists."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            if (!_configService.AssistantConfigCreated(request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), 
                    new string[] { request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName });

                response = new()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"),
                    new string[] { request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName });

            string version = _configService.GetMostRecentVersion();

            response = new()
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

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Assistant Configuration (Post) endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
