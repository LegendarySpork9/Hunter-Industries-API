// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Portfolio
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Filter")]
    public class FilterController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public FilterController(
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
        /// Returns a collection of filters.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /portfolio/filter
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="includeDeleted">Whether to return deleted filters.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Filter.Read")]
        [VersionedRoute("portfolio/filter", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<FilterRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] bool includeDeleted = false)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            FilterService _filterService = new FilterService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Get) endpoint called with the following parameter \"{includeDeleted}\".");

            List<FilterRecord> filters = await _filterService.GetFilters(includeDeleted);

            if (filters.Count == 0)
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
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"IncludeDeleted: {includeDeleted}"
                    },
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = filters
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"IncludeDeleted: {includeDeleted}"
                },
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Creates a new filter record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /portfolio/filter
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Test",
        ///         "values": "C#,Python,HTML,CSS"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the filter information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Filter.Create")]
        [VersionedRoute("portfolio/filter", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If a record matching the details already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(FilterRecord), Description = "If the record is successfully created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] FilterModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            FilterService _filterService = new FilterService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new FilterModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(
                request,
                propertiesAllowedNulls: new string[]
                {
                    "Operator",
                    "Path",
                    "Values"
                }))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            string filterTypeValidationError = FilterFunction.ValidateFilterType(request);

            if (filterTypeValidationError != null)
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = filterTypeValidationError
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (string.IsNullOrWhiteSpace(request.Type))
            {
                request.Type = "tag";
            }

            if (await _filterService.FilterExists(request.Name))
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A record with the details already exists."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("OK"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            (bool created, int id) = await _filterService.FilterCreated(request);

            if (!created)
            {
                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            if (id == 0)
            {
                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "The new record could not be found. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new FilterRecord
                {
                    Id = id,
                    Name = request.Name,
                    Type = request.Type,
                    Operator = request.Operator,
                    Path = request.Path,
                    Values = request.Values?.Split(',')
                        .ToList()
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("POST"),
                AuditHistoryConverter.GetStatusId("Created"),
                username,
                applicationName,
                null,
                requestBody: ParameterFunction.SerialiseRequestBody(request),
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.Created,
                response.Data);
        }

        /// <summary>
        /// Updates the details of the filter record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /portfolio/filter/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Test 2"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the filter.</param>
        /// <param name="request">An object containing the record data.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Filter.Update")]
        [VersionedRoute("portfolio/filter/{id:int}", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FilterRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(
            int id,
            [FromBody, Required] FilterModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            FilterService _filterService = new FilterService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            ChangeService _changeService = new ChangeService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new FilterModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request))
            {
                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _filterService.FilterExists(id))
            {
                FilterRecord filter = await _filterService.GetFilter(id);

                if (await _filterService.FilterUpdated(
                    id,
                    request))
                {
                    FilterRecord updatedFilter = await _filterService.GetFilter(id);

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = updatedFilter
                    };

                    (bool, int) audit = await _auditHistoryService.LogRequest(
                        IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                        AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                        AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                        AuditHistoryConverter.GetMethodId("PATCH"),
                        AuditHistoryConverter.GetStatusId("OK"),
                        username,
                        applicationName,
                        new string[]
                        {
                            $"Id: {id}"
                        },
                        requestBody: ParameterFunction.SerialiseRequestBody(request),
                        responseBody: ResponseFunction.GetModelJSON(response.Data));

                    if (updatedFilter.Name != filter.Name)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Name",
                            filter.Name,
                            updatedFilter.Name);
                    }

                    if (updatedFilter.Type != filter.Type)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Type",
                            filter.Type,
                            updatedFilter.Type);
                    }

                    if (updatedFilter.Operator != filter.Operator)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Operator",
                            filter.Operator ?? "",
                            updatedFilter.Operator ?? "");
                    }

                    if (updatedFilter.Path != filter.Path)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Path",
                            filter.Path ?? "",
                            updatedFilter.Path ?? "");
                    }

                    string updatedValues = string.Join(",", updatedFilter.Values);
                    string values = string.Join(",", filter.Values);

                    if (updatedValues != values)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Values",
                            values,
                            updatedValues);
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Filter (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(
                        HttpStatusCode.OK,
                        response.Data);
                }

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an update statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Id: {id}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Filter (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No filter exists with the given id."
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("PATCH"),
                AuditHistoryConverter.GetStatusId("NotFound"),
                username,
                applicationName,
                new string[]
                {
                    $"Id: {id}"
                },
                requestBody: ParameterFunction.SerialiseRequestBody(request),
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }

        /// <summary>
        /// Deletes the filter record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     Delete /portfolio/filter/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the filter.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Filter.Delete")]
        [VersionedRoute("portfolio/filter/{id:int}", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "Returns a confirmation that the record was deleted.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Delete(
            int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            FilterService _filterService = new FilterService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            ChangeService _changeService = new ChangeService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Filter (Delete) endpoint called with the following parameter \"{id}\".");

            if (await _filterService.FilterExists(id))
            {
                if (await _filterService.FilterDeleted(id))
                {
                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            information = "The given record has been deleted."
                        }
                    };

                    (bool, int) audit = await _auditHistoryService.LogRequest(
                        IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                        AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                        AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                        AuditHistoryConverter.GetMethodId("DELETE"),
                        AuditHistoryConverter.GetStatusId("OK"),
                        username,
                        applicationName,
                        new string[]
                        {
                            $"Id: {id}"
                        },
                        requestBody: null,
                        responseBody: ResponseFunction.GetModelJSON(response.Data));

                    await _changeService.LogChange(
                        audit.Item2,
                        "IsDeleted",
                        "0",
                        "1");

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Filter (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(
                        HttpStatusCode.OK,
                        response.Data);
                }

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running a delete statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                    AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("DELETE"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
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
                    $"Filter (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No record exists with the given id."
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio/filter"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("DELETE"),
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
                $"Filter (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }

    }
}