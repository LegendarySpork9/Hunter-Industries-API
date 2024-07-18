// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Converters.Assistant;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Services;
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
            ConfigConverter _configConverter = new();

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the config(s) from the AssistantInformation table.
            var Result = GetAssistantConfig(filters.AssistantName, filters.AssistantID);
            string[] AssistantNames = Result.Item1;

            // Checks if data was returned.
            if (AssistantNames == Array.Empty<string>())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            var FormattedRecords = _configConverter.FormatData(Result.Item1, Result.Item2, Result.Item3, Result.Item4, Result.Item5, Result.Item6);
            int TotalRecords = Result.Item7;

            return Ok(new
            {
                latestrelease = Result.Item8,
                configs = FormattedRecords,
                configcount = Result.Item1.Length,
                totalcount = TotalRecords
            });
        }

        [HttpPost]
        public IActionResult CreateConfig([FromBody] ConfigModel request)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();

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
            if (AssistantExists(request.AssistantName, request.IDNumber))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"),
                    _auditHistoryConverter.GetStatusID("OK"), new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return Ok(new
                {
                    information = "A config with the name and ID already exists."
                });
            }

            // Creates the config and returns the result.
            if (!AssistantConfigCreated(request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"),
                    _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/config"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"),
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

            string Version = GetMostRecentVersion();

            return StatusCode(201, new
            {
                assistantname = request.AssistantName,
                assistantid = request.IDNumber,
                designateduser = request.AssignedUser,
                hostname = request.HostName,
                deletion = false,
                version = Version
            });
        }
    }
}
