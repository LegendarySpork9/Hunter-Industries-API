// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HunterIndustriesAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "APIControlPanel")]
    [Route("api/AuditHistory")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        /// <summary>
        /// Returns a collection of calls made to the api.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /audithistory?FromDate=02/11/2024&amp;Endpoint=https://hunter-industries.co.uk/api/auth/token
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <response code="200">Returns the audit history collection or nothing.</response>
        /// <response code="400">If the filters are invalid.</response>
        /// <response code="401">If the bearer token is expired or fails validation.</response>
        [HttpGet]
        [ProducesResponseType(typeof(AuditHistoryResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult RequestAuditHistory([FromQuery] AuditHistoryFilterModel filters)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint called with the following parameters {_logger.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("audithistory"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("BadRequest"), null);

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("audithistory"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.IPAddress, filters.Endpoint, filters.FromDate, filters.PageSize.ToString(), filters.PageNumber.ToString() });

            var result = _auditHistoryService.GetAuditHistory(filters.IPAddress, filters.Endpoint, DateTime.ParseExact(filters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), filters.PageSize, filters.PageNumber);
            List<AuditHistoryRecord> auditHistories = result.Item1;

            if (auditHistories.Count == 0)
            {
                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            int totalRecords = result.Item2;
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)filters.PageSize);

            response = new()
            {
                StatusCode = 200,
                Data = new AuditHistoryResponseModel()
                {
                    Entries = auditHistories,
                    EntryCount = auditHistories.Count,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalPageCount = totalPages,
                    TotalCount = totalRecords
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
