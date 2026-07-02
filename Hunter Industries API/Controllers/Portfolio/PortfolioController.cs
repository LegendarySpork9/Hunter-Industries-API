// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Models.Requests.Filters.Media;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Media;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.Portfolio
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Portfolio")]
    public class PortfolioController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public PortfolioController(
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
        /// Returns a collection of portfolio items.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /portfolio
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="includeDeleted">Whether to return deleted media.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Portfolio.Read")]
        [VersionedRoute("portfolio", "2.2")]
        [SwaggerOperation("GetPortfolio")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ItemRecord>), Description = "Returns the item(s) matching the given parameters.")]
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
            PortfolioService _portfolioService = new PortfolioService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            GitHubService _gitHubService = new GitHubService(
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
                $"Portfolio (Get) endpoint called with the following parameter \"{includeDeleted}\".");

            List<ItemRecord> portfolio = await _portfolioService.GetItems(includeDeleted);

            if (portfolio.Count == 0)
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            List<object> rawFrameworks = await _portfolioService.GetLinkedItemData(
                "frameworks",
                includeDeleted: includeDeleted);
            List<(int, string)> frameworks = rawFrameworks.Cast<(int, string)>()
                .ToList();
            List<object> rawLanguages = await _portfolioService.GetLinkedItemData(
                "languages",
                includeDeleted: includeDeleted);
            List<(int, string)> languages = rawLanguages.Cast<(int, string)>()
                .ToList();
            List<object> rawEnvironments = await _portfolioService.GetLinkedItemData(
                "environments",
                includeDeleted: includeDeleted);
            List<(int, string)> environments = rawEnvironments.Cast<(int, string)>()
                .ToList();
            List<object> rawBuildHistories = await _portfolioService.GetLinkedItemData(
                "buildHistories",
                includeDeleted: includeDeleted);
            List<(int, BuildHistoryRecord)> buildHistories = rawBuildHistories.Cast<(int, BuildHistoryRecord)>()
                .ToList();
            
            foreach (ItemRecord item in portfolio)
            {
                string repository = item.GitHubInformation.URL.Split('/')
                    .Last();

                Dictionary<string, string> ciStatuses = await _gitHubService.GetCIStatus(repository);
                GitHubIssueBreakdownRecord issueBreakdown = await _gitHubService.GetIssueBreakdown(repository);
                List<(int, string)> itemFrameworks = frameworks.Where(f => f.Item1 == item.Id)
                    .ToList();
                List<(int, string)> itemLanguages = languages.Where(l => l.Item1 == item.Id)
                    .ToList();
                List<(int, string)> itemEnvironments = environments.Where(e => e.Item1 == item.Id)
                    .ToList();
                List<(int, BuildHistoryRecord)> itemBuildHistories = buildHistories.Where(bh => bh.Item1 == item.Id)
                    .ToList();

                item.Frameworks = itemFrameworks.Select(f => f.Item2)
                    .ToList();
                item.Languages = itemLanguages.Select(l => l.Item2)
                    .ToList();
                item.Environments = itemEnvironments.Select(e => e.Item2)
                    .ToList();
                item.BuildHistory = itemBuildHistories.Select(bh => bh.Item2)
                    .ToList();
                item.GitHubInformation.CIStatus = ciStatuses;
                item.GitHubInformation.IssueBreakdown = issueBreakdown;
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = portfolio
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio"),
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
                $"Portfolio (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Returns the portfolio item matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /portfolio/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the portfolio item.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Portfolio.Read")]
        [VersionedRoute("portfolio/{id:int}", "2.2")]
        [SwaggerOperation("GetPortfolioId")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(MediaRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            PortfolioService _portfolioService = new PortfolioService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            GitHubService _gitHubService = new GitHubService(
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
                $"Portfolio (Get) endpoint called with the following parameters \"{id}\".");

            ItemRecord item = await _portfolioService.GetItem(id);

            if (item == null)
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
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
                    $"Portfolio (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            List<object> rawFrameworks = await _portfolioService.GetLinkedItemData(
                "frameworks",
                id);
            List<(int, string)> frameworks = rawFrameworks.Cast<(int, string)>()
                .ToList();
            List<object> rawLanguages = await _portfolioService.GetLinkedItemData(
                "languages",
                id);
            List<(int, string)> languages = rawLanguages.Cast<(int, string)>()
                .ToList();
            List<object> rawEnvironments = await _portfolioService.GetLinkedItemData(
                "environments",
                id);
            List<(int, string)> environments = rawEnvironments.Cast<(int, string)>()
                .ToList();
            List<object> rawBuildHistories = await _portfolioService.GetLinkedItemData(
                "buildHistories",
                id);
            List<(int, BuildHistoryRecord)> buildHistories = rawBuildHistories.Cast<(int, BuildHistoryRecord)>()
                .ToList();

            string repository = item.GitHubInformation.URL.Split('/')
                    .Last();

            Dictionary<string, string> ciStatuses = await _gitHubService.GetCIStatus(repository);
            GitHubIssueBreakdownRecord issueBreakdown = await _gitHubService.GetIssueBreakdown(repository);
            List<GitHubIssueAssigneeBreakdownRecord> issueAssigneeBreakdown = await _gitHubService.GetIssueAssigneeBreakdown(repository);
            List<GitHubIssueInProgressBreakdownRecord> issueInProgressBreakdown = await _gitHubService.GetIssueInProgressBreakdown(repository);

            item.Frameworks = frameworks.Select(f => f.Item2)
                    .ToList();
            item.Languages = languages.Select(l => l.Item2)
                .ToList();
            item.Environments = environments.Select(e => e.Item2)
                .ToList();
            item.BuildHistory = buildHistories.Select(bh => bh.Item2)
                .ToList();
            item.GitHubInformation.CIStatus = ciStatuses;
            item.GitHubInformation.IssueBreakdown = issueBreakdown;
            item.GitHubInformation.AssigneeBreakdown = issueAssigneeBreakdown;
            item.GitHubInformation.InProgressBreakdown = issueInProgressBreakdown;

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = item
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio"),
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
                $"Portfolio (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Creates a new portfolio record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /portfolio/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Example",
        ///         "extension": ".png",
        ///         "mimeType": "image/png",
        ///         "size": 1024,
        ///         "path": null,
        ///         "domain": "https://media.example.com"
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the portfolio information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Portfolio.Create")]
        [VersionedRoute("portfolio", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If a record matching the details already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(MediaRecord), Description = "If the record is successfully created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] ItemModel request)
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

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new ItemModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Portfolio (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(
                request,
                true,
                null,
                new string[]
                {
                    "DemoLink",
                    "UnitTestCoverage",
                    "LLMUsage",
                    "LLMUsageNotes"
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _portfolioService.ItemExists(request.Name))
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            bool failed = false;
            bool created = false;

            created = await _portfolioService.LinkedItemDataCreated(
                "type",
                0,
                request.Type);
            created = await _portfolioService.LinkedItemDataCreated(
                "llmCompany",
                0,
                request.LLMUsage.Company);
            created = await _portfolioService.LinkedItemDataCreated(
                "llmModel",
                0,
                request.LLMUsage);

            foreach (string framework in request.Frameworks)
            {
                created = await _portfolioService.LinkedItemDataCreated(
                    "frameworks",
                    0,
                    framework);

                if (!created)
                {
                    failed = true;
                }
            }

            foreach (string language in request.Languages)
            {
                created = await _portfolioService.LinkedItemDataCreated(
                    "languages",
                    0,
                    language);

                if (!created)
                {
                    failed = true;
                }
            }

            foreach (string environment in request.Environments)
            {
                created = await _portfolioService.LinkedItemDataCreated(
                    "environments",
                    0,
                    environment);

                if (!created)
                {
                    failed = true;
                }
            }

            int id = 0;

            (created, id) = await _portfolioService.ItemCreated(request);

            if (created)
            {
                foreach (BuildHistoryRecord buildHistory in request.BuildHistory)
                {
                    created = await _portfolioService.LinkedItemDataCreated(
                        "buildHistories",
                        id,
                        buildHistory);

                    if (!created)
                    {
                        failed = true;
                    }
                }

                foreach (string framework in request.Frameworks)
                {
                    created = await _portfolioService.LinkItemDataCreated(
                        "frameworks",
                        id,
                        framework);

                    if (!created)
                    {
                        failed = true;
                    }
                }

                foreach (string language in request.Languages)
                {
                    created = await _portfolioService.LinkItemDataCreated(
                        "languages",
                        id,
                        language);

                    if (!created)
                    {
                        failed = true;
                    }
                }

                foreach (string environment in request.Environments)
                {
                    created = await _portfolioService.LinkItemDataCreated(
                        "environments",
                        id,
                        environment);

                    if (!created)
                    {
                        failed = true;
                    }
                }
            }

            if (!created || failed)
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            ItemRecord item = await _portfolioService.GetItem(id);

            if (item == null)
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = item
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("portfolio"),
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
                $"Portfolio (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.Created,
                response.Data);
        }

        /// <summary>
        /// Updates the details of the portfolio item.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /portfolio/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Test 2"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the portfolio item.</param>
        /// <param name="request">An object containing the record data.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Portfolio.Update")]
        [VersionedRoute("portfolio/{id:int}", "2.2")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ItemRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(
            int id,
            [FromBody, Required] ItemModel request)
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
                request = new ItemModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Portfolio (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(request)}.");

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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _portfolioService.ItemExists(id))
            {
                await _portfolioService.LinkedItemDataCreated(
                    "type",
                    0,
                    request.Type);
                await _portfolioService.LinkedItemDataCreated(
                    "llmCompany",
                    0,
                    request.LLMUsage.Company);
                await _portfolioService.LinkedItemDataCreated(
                    "llmModel",
                    0,
                    request.LLMUsage);

                List<object> rawFrameworks = await _portfolioService.GetLinkedItemData(
                    "frameworks",
                    id);
                List<(int, string)> frameworks = rawFrameworks.Cast<(int, string)>()
                    .ToList();
                List<object> rawLanguages = await _portfolioService.GetLinkedItemData(
                    "languages",
                    id);
                List<(int, string)> languages = rawLanguages.Cast<(int, string)>()
                    .ToList();
                List<object> rawEnviornments = await _portfolioService.GetLinkedItemData(
                    "environments",
                    id);
                List<(int, string)> environments = rawEnviornments.Cast<(int, string)>()
                    .ToList();

                string frameworkString = null;
                string updatedFrameworks = null;
                string languageString = null;
                string updatedLanguages = null;
                string environmentString = null;
                string updatedEnvironments = null;
                List<BuildHistoryRecord> updatedBuildHistory = new List<BuildHistoryRecord>();

                if (!string.IsNullOrWhiteSpace(string.Join(",", request.Frameworks)))
                {
                    frameworkString = string.Join(",", frameworks.Select(f => f.Item2));
                    updatedFrameworks = string.Join(",", request.Frameworks);

                    if (updatedFrameworks != frameworkString)
                    {
                        frameworks.Clear();

                        foreach (string framework in frameworks.Select(f => f.Item2))
                        {
                            await _portfolioService.LinkItemDataDeleted(
                                "frameworks",
                                id,
                                framework);
                        }

                        foreach (string framework in request.Frameworks)
                        {
                            frameworks.Add((0, framework));

                            await _portfolioService.LinkItemDataCreated(
                                "frameworks",
                                id,
                                framework);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(string.Join(",", request.Languages)))
                {
                    languageString = string.Join(",", languages.Select(f => f.Item2));
                    updatedLanguages = string.Join(",", request.Languages);

                    if (updatedLanguages != languageString)
                    {
                        languages.Clear();

                        foreach (string language in languages.Select(f => f.Item2))
                        {
                            await _portfolioService.LinkItemDataDeleted(
                                "languages",
                                id,
                                language);
                        }

                        foreach (string language in request.Languages)
                        {
                            languages.Add((0, language));

                            await _portfolioService.LinkItemDataCreated(
                                "languages",
                                id,
                                language);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(string.Join(",", request.Environments)))
                {
                    environmentString = string.Join(",", environments.Select(f => f.Item2));
                    updatedEnvironments = string.Join(",", request.Environments);

                    if (updatedEnvironments != environmentString)
                    {
                        environments.Clear();

                        foreach (string environment in environments.Select(f => f.Item2))
                        {
                            await _portfolioService.LinkItemDataDeleted(
                                "enviornments",
                                id,
                                environment);
                        }

                        foreach (string environment in request.Environments)
                        {
                            environments.Add((0, environment));

                            await _portfolioService.LinkItemDataCreated(
                                "environments",
                                id,
                                environment);
                        }
                    }
                }

                if (request.BuildHistory != null && request.BuildHistory.Count > 0)
                {
                    foreach (BuildHistoryRecord buildHistoryRecord in request.BuildHistory)
                    {
                        await _portfolioService.LinkedItemDataCreated(
                            "buildHistories",
                            id,
                            request.BuildHistory);
                    }

                    updatedBuildHistory.AddRange(request.BuildHistory);
                }

                List<object> rawBuildHistories = await _portfolioService.GetLinkedItemData(
                    "buildHistories",
                    id);
                List<(int, BuildHistoryRecord)> buildHistories = rawBuildHistories.Cast<(int, BuildHistoryRecord)>()
                    .ToList();

                ItemRecord item = await _portfolioService.GetItem(id);

                if (await _portfolioService.ItemUpdated(
                    id,
                    request))
                {
                    PropertyInfo[] requestProperties = request.GetType()
                        .GetProperties();

                    ItemRecord updatedItem = await _portfolioService.GetItem(id);
                    updatedItem.Frameworks = frameworks.Select(f => f.Item2)
                        .ToList();
                    updatedItem.Languages = languages.Select(l => l.Item2)
                        .ToList();
                    updatedItem.Environments = environments.Select(e => e.Item2)
                        .ToList();
                    updatedItem.BuildHistory = buildHistories.Select(bh => bh.Item2)
                        .ToList();

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = updatedItem
                    };

                    (bool, int) audit = await _auditHistoryService.LogRequest(
                        IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                        AuditHistoryConverter.GetEndpointId("portfolio"),
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

                    foreach (PropertyInfo prop in requestProperties)
                    {
                        object newValue = prop.GetValue(request);

                        if (newValue == null)
                        {
                            continue;
                        }

                        PropertyInfo recordProp = item.GetType()
                            .GetProperty(prop.Name);

                        if (recordProp == null)
                        {
                            continue;
                        }

                        Type propType = prop.PropertyType;

                        if (propType.IsClass || (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>)))
                        {
                            continue;
                        }

                        string newString = newValue.ToString();
                        string oldString = recordProp.GetValue(item)?
                            .ToString() ?? string.Empty;

                        if (newString != oldString)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                prop.Name,
                                oldString,
                                newString);
                        }
                    }

                    if (updatedFrameworks != frameworkString)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Frameworks",
                            frameworkString,
                            updatedFrameworks);
                    }

                    if (updatedLanguages != languageString)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Languages",
                            languageString,
                            updatedLanguages);
                    }

                    if (updatedEnvironments != environmentString)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Environments",
                            environmentString,
                            updatedEnvironments);
                    }

                    if (request.LLMUsage != null)
                    {
                        if (item.LLMUsage == null)
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "LLM Company",
                                null,
                                request.LLMUsage.Company);
                            await _changeService.LogChange(
                                audit.Item2,
                                "LLM Model",
                                null,
                                request.LLMUsage.Model);
                        }

                        else
                        {
                            await _changeService.LogChange(
                                audit.Item2,
                                "LLM Company",
                                item.LLMUsage.Company,
                                request.LLMUsage.Company);
                            await _changeService.LogChange(
                                audit.Item2,
                                "LLM Model",
                                item.LLMUsage.Model,
                                request.LLMUsage.Model);
                        }
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Portfolio (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                AuditHistoryConverter.GetEndpointId("portfolio"),
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
                $"Portfolio (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }

        /// <summary>
        /// Deletes the portfolio item record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     Delete /portfolio/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the portfolio item.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Portfolio.Delete")]
        [VersionedRoute("portfolio/{id:int}", "2.2")]
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
            PortfolioService _portfolioService = new PortfolioService(
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
                $"Portfolio (Delete) endpoint called with the following parameters \"{id}\".");

            if (await _portfolioService.ItemExists(id))
            {
                if (await _portfolioService.ItemDeleted(id))
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
                        AuditHistoryConverter.GetEndpointId("portfolio"),
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
                        $"Portfolio (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                    AuditHistoryConverter.GetEndpointId("portfolio"),
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
                    $"Portfolio (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                AuditHistoryConverter.GetEndpointId("portfolio"),
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
                $"Portfolio (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }
    }
}