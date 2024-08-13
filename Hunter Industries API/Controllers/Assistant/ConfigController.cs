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

            ResponseModel response;

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId });

            // Gets the config(s) from the AssistantInformation table.
            var result = _configService.GetAssistantConfig(filters.AssistantName, filters.AssistantId);
            List<AssistantConfiguration> assistantConfigurations = result.Item1;

            // Checks if data was returned.
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
            }

            else
            {
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
            }

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpPost]
        public IActionResult CreateConfig([FromBody] ConfigModel request)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ConfigService _configService = new();

            ResponseModel response = new();
            bool responseGiven = false;

            // Checks if the request contains a body.
            if (request == null)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no body provided."
                    }
                };
                responseGiven = true;
            }

            // Checks whether all requireds are present.
            if (!responseGiven && (string.IsNullOrWhiteSpace(request.AssistantName) || string.IsNullOrWhiteSpace(request.IdNumber) || string.IsNullOrWhiteSpace(request.AssignedUser) || string.IsNullOrWhiteSpace(request.HostName)))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Body parameters missing."
                    }
                };
                responseGiven = true;
            }

            // Checks if a config already exists.
            if (!responseGiven && _configService.AssistantExists(request.AssistantName, request.IdNumber))
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
                responseGiven = true;
            }

            // Creates the config and returns the result.
            if (!responseGiven && !_configService.AssistantConfigCreated(request.AssistantName, request.IdNumber, request.AssignedUser, request.HostName))
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
                responseGiven = true;
            }

            if (!responseGiven)
            {
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
            }

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
