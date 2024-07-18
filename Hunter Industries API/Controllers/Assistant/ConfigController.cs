// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Objects.Assistant;
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
    public class ConfigController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestConfig([FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the config(s) from the AssistantInformation table.
            var result = _configService.GetAssistantConfig(filters.AssistantName, filters.AssistantID);
            List<AssistantConfiguration> assistantConfigurations = result.Item1;

            // Checks if data was returned.
            if (assistantConfigurations == new List<AssistantConfiguration>())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            ConfigResponseModel response = new()
            {
                LatestRelease = result.Item3,
                AssistantConfigurations = assistantConfigurations,
                ConfigCount = assistantConfigurations.Count,
                TotalCount = result.Item2,
            };

            return StatusCode(200, response);
        }

        [HttpPost]
        public IActionResult CreateConfig([FromBody] ConfigModel request)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();

            // Checks if the request contains a body.
            if (request == null)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                return BadRequest(new
                {
                    error = "Invalid or no body provided."
                });
            }

            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(request.AssistantName) || string.IsNullOrWhiteSpace(request.IDNumber) || string.IsNullOrWhiteSpace(request.AssignedUser) || string.IsNullOrWhiteSpace(request.HostName))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return BadRequest(new
                {
                    error = "Body parameters missing."
                });
            }

            // Checks if a config already exists.
            if (_configService.AssistantExists(request.AssistantName, request.IDNumber))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), 
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return Ok(new
                {
                    information = "A config with the name and/or ID already exists."
                });
            }

            // Creates the config and returns the result.
            if (!_configService.AssistantConfigCreated(request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), 
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"),
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

            string version = _configService.GetMostRecentVersion();

            AssistantConfiguration response = new()
            {
                AssistantName = request.AssistantName,
                AssistantID = request.IDNumber,
                AssignedUser = request.AssignedUser,
                HostName = request.HostName,
                Deletion = false,
                Version = version,
            };

            return StatusCode(201, response);
        }
    }
}
