// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("Configuration")]
    public class ConfigurationController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ConfigurationController(ILoggerService _logger,
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
        /// Returns a list of configuration entities.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /configuration
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Read")]
        [VersionedRoute("configuration", "2.0")]
        [SwaggerOperation("GetConfiguration")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<object>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get()
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint called.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, null);

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new
                {
                    ConfigurationObjects = new string[]
                    {
                        "application",
                        "applicationSetting",
                        "authorisation",
                        "component",
                        "connection",
                        "downtime",
                        "game",
                        "machine"
                    }
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns a collection of records.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /configuration/application
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="entity">The name of the configuration object.</param>
        /// <param name="entityId">The id number of the parent entity record.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Read")]
        [VersionedRoute("configuration/{entity}", "2.0")]
        [SwaggerOperation("GetConfigurationList")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ConfigurationResponseModel>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(string entity, [FromUri] int pageSize = 25, [FromUri] int pageNumber = 1, [FromUri] int entityId = 0)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ConfigurationService _configurationService = new ConfigurationService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(new string[] { entity, entityId.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            int? parentEntityId = null;

            if (entityId != 0)
            {
                parentEntityId = entityId;
            }

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { entity, entityId.ToString(), pageSize.ToString(), pageNumber.ToString() });

            var result = await _configurationService.GetRecords(entity, 0, parentEntityId, pageSize, pageNumber);
            List<object> records = result.Item1;

            if (records.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            int totalRecords = result.Item2;
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize);

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = new ConfigurationResponseModel()
                {
                    Entries = records,
                    EntryCount = records.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPageCount = totalPages,
                    TotalCount = totalRecords
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the record matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /configuration/application/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="entity">The name of the configuration object.</param>
        /// <param name="id">The id number of the record.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Read")]
        [VersionedRoute("configuration/{entity}/{id:int}", "2.0")]
        [SwaggerOperation("GetConfigurationById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(object), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(string entity, int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ConfigurationService _configurationService = new ConfigurationService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(new string[] { entity, id.ToString() })}.");

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { entity, id.ToString() });

            List<object> records = (await _configurationService.GetRecords(entity, id)).Item1;

            if (records.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = records[0]
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /configuration/application
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "Name": "Test",
        ///         "Phrase": "This is a test phrase."
        ///     }
        /// </remarks>
        /// <param name="entity">The name of the configuration object.</param>
        /// <param name="entityId">The id number of the parent entity record.</param>
        /// <param name="request">An object containing the record information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Create")]
        [VersionedRoute("configuration/{entity}", "2.0")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a record matching the details already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(object), Description = "If the record is successfuly created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post(string entity, [FromBody, Required] object request, [FromUri] int entityId = 0)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ConfigurationService _configurationService = new ConfigurationService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new object();
            }

            else
            {
                request = ConfigurationConverter.GetRequestObject(entity, request);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint called with the following parameters \"{entity}\", \"{entityId}\", {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    username, applicationName, null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            int? parentEntityId = null;

            if (entityId != 0)
            {
                parentEntityId = entityId;
            }

            if (await _configurationService.RecordExists(entity, request, parentEntityId))
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, entityId.ToString() }));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A record with the details already exists."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            (bool created, int id) = await _configurationService.RecordCreated(entity, request, parentEntityId);

            if (!created)
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"),
                    username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, entityId.ToString() }));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            List<object> records = (await _configurationService.GetRecords(entity, id)).Item1;

            if (records.Count == 0)
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"),
                    username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, entityId.ToString() }));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }


            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Created"),
                username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, entityId.ToString() }));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = records[0]
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.Created, response.Data);
        }

        /// <summary>
        /// Updates the details of the record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /configuration/application/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "Name": "TestApplication"
        ///     }
        /// </remarks>
        /// <param name="entity">The name of the configuration object.</param>
        /// <param name="id">The id number of the record.</param>
        /// <param name="request">An object containing the record data.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Update")]
        [VersionedRoute("configuration/{entity}/{id:int}", "2.0")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(object), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(string entity, int id, [FromBody, Required] object request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            ConfigurationService _configurationService = new ConfigurationService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new object();
            }

            else
            {
                request = ConfigurationConverter.GetRequestObject(entity, request);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Patch) endpoint called with the following parameters \"{entity}\", \"{id}\", {ParameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request))
            {
                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    username, applicationName, null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _configurationService.RecordExists(entity, id))
            {
                object record = (await _configurationService.GetRecords(entity, id)).Item1[0];

                if (await _configurationService.RecordUpdated(entity, id, request))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                        username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, id.ToString() }));

                    PropertyInfo[] requestProperties = request.GetType().GetProperties();

                    foreach (PropertyInfo prop in requestProperties)
                    {
                        object newValue = prop.GetValue(request);

                        if (newValue == null)
                        {
                            continue;
                        }

                        PropertyInfo recordProp = record.GetType().GetProperty(prop.Name);

                        if (recordProp == null)
                        {
                            continue;
                        }

                        string newString = newValue.ToString();
                        string oldString = recordProp.GetValue(record)?.ToString() ?? string.Empty;

                        if (newString != oldString)
                        {
                            await _changeService.LogChange(audit.Item2, prop.Name, oldString, newString);
                        }
                    }

                    object updatedRecord = (await _configurationService.GetRecords(entity, id)).Item1[0];

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = updatedRecord
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("InternalServerError"),
                    username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, id.ToString() }));

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                username, applicationName, ParameterFunction.FormatParameters(request, new string[] { entity, id.ToString() }));

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No record exists with the given id."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     Delete /configuration/Application/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="entity">The name of the configuration object.</param>
        /// <param name="id">The id number of the user.</param>
        [RequiredPolicyAuthorisationAttributeFilter("Configuration.Delete")]
        [VersionedRoute("configuration/{entity}/{id:int}", "2.0")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "Returns a confirmation that the record was deleted.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no record was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Delete(string entity, int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ConfigurationService _configurationService = new ConfigurationService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Delete) endpoint called with the following parameters \"{entity}\", \"{id}\".");

            if (await _configurationService.RecordExists(entity, id))
            {
                if (await _configurationService.RecordDeleted(entity, id))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"), AuditHistoryConverter.GetStatusID("OK"),
                        username, applicationName, new string[] { entity, id.ToString() });

                    await _changeService.LogChange(audit.Item2, "IsDeleted", "0", "1");

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            information = "The given record has been deleted."
                        }
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"),
                        AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, new string[] { entity, id.ToString() });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)), AuditHistoryConverter.GetEndpointID("configuration"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"), AuditHistoryConverter.GetStatusID("NotFound"),
                username, applicationName, new string[] { entity, id.ToString() });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No record exists with the given id."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Configuration (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
