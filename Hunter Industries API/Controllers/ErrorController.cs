// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
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
    [RequiredPolicyAuthorisationAttributeFilter("ErrorLog")]
    public class ErrorController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ErrorController(
            ILoggerService _logger,
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
        /// Returns a collection of errors logged by the api.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /errorlog?FromDate=24/03/2026
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [VersionedRoute("errorlog", "2.0")]
        [SwaggerOperation("GetErrorList")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ErrorLogResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] ErrorLogFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ErrorLogService _errorLogService = new ErrorLogService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new ErrorLogFilterModel();
            }

            if (filters.PageSize > 200)
            {
                filters.PageSize = 200;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Error Log endpoint called with the following parameters {ParameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("errorlog"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    ParameterFunction.FormatParameters(
                        null,
                        filters),
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Error Log endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            (List<ErrorLogRecord> errorLogs, int totalRecords) = await _errorLogService.GetErrorLog(
                filters.IPAddress,
                filters.Summary,
                DateTime.SpecifyKind(DateTime.ParseExact(filters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc),
                DateTime.SpecifyKind(DateTime.ParseExact(filters.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTimeKind.Utc),
                filters.PageSize,
                filters.PageNumber);

            if (errorLogs.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 204,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("errorlog"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
                    username,
                    applicationName,
                    ParameterFunction.FormatParameters(
                        null,
                        filters),
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Error Log endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.NoContent,
                    response.Data);
            }

            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)filters.PageSize);

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ErrorLogResponseModel()
                {
                    Entries = errorLogs,
                    EntryCount = errorLogs.Count,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalPageCount = totalPages,
                    TotalCount = totalRecords
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("errorlog"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                ParameterFunction.FormatParameters(
                    null,
                    filters),
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Error Log endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Returns the record matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /errorlog/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the error log record.</param>
        [VersionedRoute("errorlog/{id:int}", "2.0")]
        [SwaggerOperation("GetErrorById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ErrorLogRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no error was found matching the given id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ErrorLogService _errorLogService = new ErrorLogService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Error Log endpoint called with the following parameter \"{id}\".");

            ErrorLogRecord errorLog = await _errorLogService.GetErrorLogId(id);

            if (errorLog == null)
            {
                response = new ResponseModel()
                {
                    StatusCode = 404,
                    Data = new
                    {
                        error = "No error exists with the given id."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("errorlog"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NotFound"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Error Log endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = errorLog
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("errorlog"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"Id: {id}"
                },
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Error Log endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }
    }
}
