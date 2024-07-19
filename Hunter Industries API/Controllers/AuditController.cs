// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HunterIndustriesAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AssistantControlPanel")]
    [Route("api/audithistory")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestAuditHistory([FromQuery] AuditHistoryFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();

            // Checks if there are filter values.
            if (filters.PageSize == 0 || filters.PageNumber == 0)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("audithistory"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid Filters Provided."
                });
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("audithistory"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.IPAddress, filters.Endpoint, filters.FromDate, filters.PageSize.ToString(), filters.PageNumber.ToString() });

            // Gets the data from the AuditHistory table.
            var result = _auditHistoryService.GetAuditHistory(filters.IPAddress, filters.Endpoint, DateTime.ParseExact(filters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), filters.PageSize, filters.PageNumber);
            int[] auditIDs = result.Item1;

            // Checks if data was returned.
            if (auditIDs == Array.Empty<int>())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            List<AuditHistoryRecord> auditHistory = _auditHistoryConverter.FormatData(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7);
            int totalRecords = result.Item8;
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)filters.PageSize);

            AuditHistoryResponseModel response = new()
            {
                Entries = auditHistory,
                EntryCount = auditHistory.Count,
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize,
                TotalPageCount = totalPages,
                TotalCount = totalRecords
            };

            // Returns the records.
            return StatusCode(200, response);
        }
    }
}
