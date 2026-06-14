// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Models.Requests.Filters.Media;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Media;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Media")]
    public class MediaController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public MediaController(
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
        /// Returns a collection of media.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /media/Portfolio
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="application">The application the media belongs to.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Read")]
        [VersionedRoute("media/{application}", "2.1")]
        [SwaggerOperation("GetApplicationMedia")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(MediaResponseModel), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(
            string application,
            [FromUri] MediaFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            MediaService _mediaService = new MediaService(
                _Logger,
                _FileSystem,
                _Options,
                _Database);
            ModelValidationService _modelValidator = new ModelValidationService();

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new MediaFilterModel();
            }

            if (filters.PageSize > 200)
            {
                filters.PageSize = 200;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Media (Get) endpoint called with the following parameters \"{application}\", {ParameterFunction.FormatParameters(filters)}.");

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
                    AuditHistoryConverter.GetEndpointId("media"),
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
                    $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            (List<MediaRecord> media, int totalRecords) = await _mediaService.GetApplicationMedia(
                application,
                filters.PageSize,
                filters.PageNumber,
                filters.IncludeDeleted);

            if (media.Count == 0)
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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"IncludeDeleted: {filters.IncludeDeleted}",
                        $"PageSize: {filters.PageSize}",
                        $"PageNumber: {filters.PageNumber}"
                    },
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)filters.PageSize);

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new MediaResponseModel()
                {
                    Entries = media,
                    EntryCount = media.Count,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalPageCount = totalPages,
                    TotalCount = totalRecords
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("media"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"Application: {application}",
                    $"IncludeDeleted: {filters.IncludeDeleted}",
                    $"PageSize: {filters.PageSize}",
                    $"PageNumber: {filters.PageNumber}"
                },
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Returns a collection of media.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /media/Portfolio/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="application">The application the media belongs to.</param>
        /// <param name="entityId">The id number of the application item the media is for.</param>
        /// <param name="includeDeleted">Whether to return deleted media.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Read")]
        [VersionedRoute("media/{application}/{entityId:int}", "2.1")]
        [SwaggerOperation("GetApplicationItemMedia")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<MediaRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = typeof(ResponseModel), Description = "If there is no data matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(
            string application,
            int entityId,
            [FromUri] bool includeDeleted = false)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            MediaService _mediaService = new MediaService(
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
                $"Media (Get) endpoint called with the following parameters \"{application}\", \"{entityId}\".");

            List<MediaRecord> media = await _mediaService.GetApplicationEntityMedia(
                application,
                entityId,
                includeDeleted);

            if (media.Count == 0)
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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("GET"),
                    AuditHistoryConverter.GetStatusId("NoContent"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"Application Entity Id: {entityId}",
                        $"IncludeDeleted: {includeDeleted}"
                    },
                    requestBody: null,
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = media
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("media"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("GET"),
                AuditHistoryConverter.GetStatusId("OK"),
                username,
                applicationName,
                new string[]
                {
                    $"Application: {application}",
                    $"Application Entity Id: {entityId}",
                    $"IncludeDeleted: {includeDeleted}"
                },
                requestBody: null,
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Returns the media matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /media/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the media.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Read")]
        [VersionedRoute("media/{id:int}", "2.1")]
        [SwaggerOperation("GetMediaId")]
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
            MediaService _mediaService = new MediaService(
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
                $"Media (Get) endpoint called with the following parameters \"{id}\".");

            MediaRecord media = await _mediaService.GetMediaId(id);

            if (media == null)
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
                    AuditHistoryConverter.GetEndpointId("media"),
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
                    $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = media
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("media"),
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
                $"Media (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.OK,
                response.Data);
        }

        /// <summary>
        /// Creates a new media record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /media/Portfolio/1
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
        /// <param name="application">The application the media belongs to.</param>
        /// <param name="entityId">The id number of the application item the media is for.</param>
        /// <param name="request">An object containing the media information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Create")]
        [VersionedRoute("media/{application}/{entityId:int}", "2.1")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If a record matching the details already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(MediaRecord), Description = "If the record is successfully created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post(
            string application,
            int entityId,
            [FromBody, Required] MediaModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            MediaService _mediaService = new MediaService(
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
                request = new MediaModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Media (Post) endpoint called with the following parameters \"{application}\", \"{entityId}\", {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(
                request,
                true,
                null,
                new string[]
                {
                    "Path"
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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("BadRequest"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"Application Entity Id: {entityId}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _mediaService.MediaExists(
                application,
                request.Name))
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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("OK"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"Application Entity Id: {entityId}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.OK,
                    response.Data);
            }

            bool created = false;
            int id = 0;

            if (await _mediaService.MediaTypeCreated(
                request.Extension,
                request.MimeType))
            {
                (created, id) = await _mediaService.MediaCreated(
                application,
                request);

                if (created)
                {
                    if (MediaConverter.GetSQLCreateApplicationEntityLink(application) != "NoApplicationEntityLink")
                    {
                        created = await _mediaService.ApplicationEntityLinkCreated(
                        application,
                        entityId,
                        id);
                    }
                }
            }

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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"Application Entity Id: {entityId}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            MediaRecord media = await _mediaService.GetMediaId(id);

            if (media == null)
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
                    AuditHistoryConverter.GetEndpointId("media"),
                    AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                    AuditHistoryConverter.GetMethodId("POST"),
                    AuditHistoryConverter.GetStatusId("InternalServerError"),
                    username,
                    applicationName,
                    new string[]
                    {
                        $"Application: {application}",
                        $"Application Entity Id: {entityId}"
                    },
                    requestBody: ParameterFunction.SerialiseRequestBody(request),
                    responseBody: ResponseFunction.GetModelJSON(response.Data));

                _Logger.LogMessage(
                    StandardValues.LoggerValues.Info,
                    $"Media (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = media
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("media"),
                AuditHistoryConverter.GetEndpointVersionId(AuditHistoryFunction.ExtractVersionFromRequest(Request)),
                AuditHistoryConverter.GetMethodId("POST"),
                AuditHistoryConverter.GetStatusId("Created"),
                username,
                applicationName,
                new string[]
                {
                    $"Application: {application}",
                    $"Application Entity Id: {entityId}"
                },
                requestBody: ParameterFunction.SerialiseRequestBody(request),
                responseBody: ResponseFunction.GetModelJSON(response.Data));

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info,
                $"Media (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.Created,
                response.Data);
        }

        /// <summary>
        /// Updates the details of the media record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /media/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "name": "Test 2"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the media.</param>
        /// <param name="request">An object containing the record data.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Update")]
        [VersionedRoute("media/{id:int}", "2.1")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(MediaRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(
            int id,
            [FromBody, Required] MediaUpdateModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(
                _Logger,
                _FileSystem,
                _Options,
                _Database,
                _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            MediaService _mediaService = new MediaService(
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
                request = new MediaUpdateModel();
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Info, 
                $"Media (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(request)}.");

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
                    AuditHistoryConverter.GetEndpointId("media"),
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
                    $"Media (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.BadRequest,
                    response.Data);
            }

            if (await _mediaService.MediaExists(id))
            {
                MediaRecord media = await _mediaService.GetMediaId(id);

                if (await _mediaService.MediaUpdated(
                    id,
                    request))
                {
                    MediaRecord updatedMedia = await _mediaService.GetMediaId(id);

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = updatedMedia
                    };

                    (bool, int) audit = await _auditHistoryService.LogRequest(
                        IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                        AuditHistoryConverter.GetEndpointId("media"),
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

                    if (updatedMedia.Name != media.Name)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Name",
                            media.Name,
                            updatedMedia.Name);
                    }

                    if (updatedMedia.Size != media.Size)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Size",
                            media.Size.ToString(),
                            updatedMedia.Size.ToString());
                    }

                    if (updatedMedia.Path != media.Path)
                    {
                        await _changeService.LogChange(
                            audit.Item2,
                            "Path",
                            media.Path,
                            updatedMedia.Path);
                    }

                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Info,
                        $"Media (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                    AuditHistoryConverter.GetEndpointId("media"),
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
                    $"Media (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(
                    HttpStatusCode.InternalServerError,
                    response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No media exists with the given id."
                }
            };

            await _auditHistoryService.LogRequest(
                IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)),
                AuditHistoryConverter.GetEndpointId("media"),
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
                $"Media (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }

        /// <summary>
        /// Deletes the media record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     Delete /media/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the media.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Media.Delete")]
        [VersionedRoute("media/{id:int}", "2.1")]
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
            MediaService _mediaService = new MediaService(
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
                $"Media (Delete) endpoint called with the following parameters \"{id}\".");

            if (await _mediaService.MediaExists(id))
            {
                if (await _mediaService.MediaDeleted(id))
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
                        AuditHistoryConverter.GetEndpointId("media"),
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
                        $"Media (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                    AuditHistoryConverter.GetEndpointId("media"),
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
                    $"Media (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
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
                AuditHistoryConverter.GetEndpointId("media"),
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
                $"Media (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(
                HttpStatusCode.NotFound,
                response.Data);
        }
    }
}
