// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Converters.Portfolio;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Portfolio
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Metric")]
    public class MetricController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public MetricController(
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
        /// Returns a list of metrics.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /portfolio/metric
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Metric.Read")]
        [VersionedRoute("portfolio/metric", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<object>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get()
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);
            string ipAddress = IPAddressFunction.FetchIpAddress(Request);

            ResponseModel response;

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Metric (Get) endpoint called.");

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new
                {
                    Metrics = new string[]
                    {
                        "summary",
                        "full"
                    }
                }
            };

            await _auditHistoryService.LogRequest(
                ipAddress,
                AuditHistoryConverter.GetEndpointId("portfolio/metric"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                null,
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Metric (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Updates the metric for the portfolio item.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /portfolio/metric
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "id": 1,
        ///         "metric": "summary"
        ///     }
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Metric.Create")]
        [VersionedRoute("portfolio/metric", "2.2")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(ResponseModel), Description = "If the metric is successfully updated.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ItemMetricModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            PortfolioService _portfolioService = new PortfolioService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            MetricService _metricService = new MetricService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);
            string ipAddress = IPAddressFunction.FetchIpAddress(Request);

            ResponseModel response;

            if (request == null)
            {
                request = new ItemMetricModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Metric (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(
                request,
                true))
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
                    ipAddress,
                    AuditHistoryConverter.GetEndpointId("portfolio/metric"),
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
                    $"Metric (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (!await _portfolioService.ItemExists(request.Id))
            {
                response = new ResponseModel()
                {
                    StatusCode = 404,
                    Data = new
                    {
                        information = "No portfolio item record exists with the given id."
                    }
                };

                await _auditHistoryService.LogRequest(
                    ipAddress,
                    AuditHistoryConverter.GetEndpointId("portfolio/metric"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("PATCH"),
                    AuditHistoryConverter.GetStatusId("NotFound"),
                    username,
                    applicationName,
                    null,
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Metric (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.NotFound,
                    response.Data);
            }

            bool updated = await _metricService.MetricUpdated(request);

            if (!updated)
            {
                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                await _auditHistoryService.LogRequest(
                    ipAddress,
                    AuditHistoryConverter.GetEndpointId("portfolio/metric"),
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
                    $"Metric (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new
                {
                    information = $"The {MetricConverter.GetMetricName(request.Metric)} metric has been updated for portfolio item {request.Id}."
                }
            };

            await _auditHistoryService.LogRequest(
                ipAddress,
                AuditHistoryConverter.GetEndpointId("portfolio/metric"),
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
                $"Metric (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.Created,
                response.Data);
        }
    }
}