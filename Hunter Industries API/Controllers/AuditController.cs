// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("AuditHistory")]
    public class AuditController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public AuditController(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabase _database,
            IDatabaseOptions _options,
            IClock _clock)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Database = _database;
            _Options = _options;
            _Clock = _clock;
        }

        /// <summary>
        /// Returns a collection of calls made to the api.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /audithistory?FromDate=02/11/2024&amp;Endpoint=https://api.hunter-industries.co.uk/v2.0/auth/token
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [VersionedRoute("audithistory", "1.0")]
        [SwaggerOperation("GetAuditList")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuditHistoryResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] AuditHistoryFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunctions.GetUsername(principal);
            string applicationName = ClaimFunctions.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new AuditHistoryFilterModel();
            }

            if (filters.PageSize > 200)
            {
                filters.PageSize = 200;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint called with the following parameters {ParameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("audithistory"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("BadRequest"), username, applicationName, ParameterFunction.FormatParameters(null, filters));

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("audithistory"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"), username, applicationName,
                    new string[] { filters.IPAddress, filters.Endpoint, filters.FromDate, filters.ToDate, filters.PageSize.ToString(), filters.PageNumber.ToString() });

            var result = await _auditHistoryService.GetAuditHistory(0, filters.IPAddress, filters.Endpoint, filters.Username, filters.Application, DateTime.SpecifyKind(DateTime.ParseExact(filters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc), DateTime.SpecifyKind(DateTime.ParseExact(filters.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc), filters.PageSize, filters.PageNumber);
            List<AuditHistoryRecord> auditHistories = result.Item1;

            if (auditHistories.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            int totalRecords = result.Item2;
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)filters.PageSize);

            response = new ResponseModel()
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

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the record matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /audithistory/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the audit history record.</param>
        [VersionedRoute("audithistory/{id:int}", "1.0")]
        [SwaggerOperation("GetAuditById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuditHistoryRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunctions.GetUsername(principal);
            string applicationName = ClaimFunctions.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint called with the following parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("audithistory"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunctions.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"), username, applicationName,
                    new string[] { id.ToString() });

            var result = await _auditHistoryService.GetAuditHistory(id, null, null, null, null, _Clock.DefaultDate, _Clock.DefaultDate, 25, 1);
            List<AuditHistoryRecord> auditHistories = result.Item1;

            if (auditHistories.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = auditHistories[0]
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Audit History endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }
    }
}
