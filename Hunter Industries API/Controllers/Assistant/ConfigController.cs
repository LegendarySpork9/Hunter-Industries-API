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

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpPost]
        public IActionResult CreateConfig([FromBody] ConfigModel request)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            ConfigService _configService = new();

            ResponseModel response = new();

            // Checks if the request contains a body.
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

                return StatusCode(response.StatusCode, response.Data);
            }

            // Checks if a config already exists.
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

                return StatusCode(response.StatusCode, response.Data);
            }

            // Creates the config and returns the result.
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

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
