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
            ModelValidationService _modelValidator = new();

            ResponseModel response = new();

            // Checks if there are filter values.
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

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("audithistory"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.IPAddress, filters.Endpoint, filters.FromDate, filters.PageSize.ToString(), filters.PageNumber.ToString() });

            // Gets the data from the AuditHistory table.
            var result = _auditHistoryService.GetAuditHistory(filters.IPAddress, filters.Endpoint, DateTime.ParseExact(filters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), filters.PageSize, filters.PageNumber);
            List<AuditHistoryRecord> auditHistories = result.Item1;

            // Checks if data was returned.
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

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
