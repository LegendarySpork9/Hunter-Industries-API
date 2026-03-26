// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects.User;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.User;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.User
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("User")]
    public class UserController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public UserController(ILoggerService _logger,
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
        /// Returns a collection of users.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /User?Username=TestUser
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [RequiredPolicyAuthorisationAttributeFilter("User.Read")]
        [VersionedRoute("user", "1.0")]
        [SwaggerOperation("GetUserList")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<UserRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get([FromUri] UserFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (filters == null)
            {
                filters = new UserFilterModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("BadRequest"), username, applicationName, ParameterFunction.FormatParameters(null, filters));

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { filters.Id.ToString(), filters.Username });

            List<UserRecord> users = await _userService.GetUsers(filters.Id, filters.Username);

            if (users.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = users
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Returns the record matching the given id.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     GET /User/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the user.</param>
        [RequiredPolicyAuthorisationAttributeFilter("User.Read")]
        [VersionedRoute("user/{id:int}", "1.0")]
        [SwaggerOperation("GetUserById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint called with the following parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    username, applicationName, new string[] { id.ToString() });

            List<UserRecord> users = await _userService.GetUsers(id, null);

            if (users.Count == 0)
            {
                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = users[0]
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.OK, response.Data);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /user
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "Username": "TestUser",
        ///         "Password": "Password",
        ///         "Scopes": [
        ///             "Assistant API"
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="request">An object containing the user information.</param>
        [RequiredPolicyAuthorisationAttributeFilter("User.Create")]
        [VersionedRoute("user", "1.0")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a user matching the username already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(UserRecord), Description = "If the user is successfuly created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] UserModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new UserModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint called with the following parameters {ParameterFunction.FormatParameters(request, true)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    username, applicationName, null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _userService.UserExists(request.Username))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("OK"), username, applicationName, ParameterFunction.FormatParameters(null, request, true));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A user with the username already exists."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.OK, response.Data);
            }

            (bool created, int id) = await _userService.UserCreated(request.Username, request.Password);

            if (!created)
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, ParameterFunction.FormatParameters(null, request, true));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            created = await _userService.UserScopeCreated(id, request.Scopes);

            if (!created)
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, ParameterFunction.FormatParameters(null, request, true));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Created"), username, applicationName, ParameterFunction.FormatParameters(null, request, true));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new UserRecord()
                {
                    Id = id,
                    Username = request.Username,
                    Password = HashFunction.HashString(request.Password),
                    Scopes = request.Scopes
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.Created, response.Data);
        }

        /// <summary>
        /// Updates the details of the user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     PATCH /user/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        ///     Content-Type: application/json
        ///     {
        ///         "Password": "Password2"
        ///     }
        /// </remarks>
        /// <param name="id">The id number of the user.</param>
        /// <param name="request">An object containing the user data.</param>
        [RequiredPolicyAuthorisationAttributeFilter("User.Update")]
        [VersionedRoute("user/{id:int}", "1.1")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no user was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Patch(int id, [FromBody, Required] UserModel request)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            if (request == null)
            {
                request = new UserModel();
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint called with the following parameters \"{id}\", {ParameterFunction.FormatParameters(null, request, true)}.");

            if (!_modelValidator.IsValid(request))
            {
                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    username, applicationName, null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (await _userService.UserExists(id))
            {
                UserRecord userRecord = (await _userService.GetUsers(id, null))[0];

                if (await _userService.UserUpdated(id, request.Username, HashFunction.HashString(request.Password), UserFunction.GetScopesUpdateList(userRecord.Scopes, request.Scopes)))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                        username, applicationName, new string[] { id.ToString(), request.Username, HashFunction.HashString(request.Password), ParameterFunction.FormatParameters(request.Scopes?.ToList<object>(), true) });

                    if (!string.IsNullOrEmpty(request.Username) && request.Username != userRecord.Username)
                    {
                        await _changeService.LogChange(audit.Item2, "Username", userRecord.Username, request.Username);
                        userRecord.Username = request.Username;
                    }

                    if (!string.IsNullOrEmpty(request.Password) && HashFunction.HashString(request.Password) != userRecord.Password)
                    {
                        await _changeService.LogChange(audit.Item2, "Password", userRecord.Password, HashFunction.HashString(request.Password));
                        userRecord.Password = HashFunction.HashString(request.Password);
                    }

                    if (request.Scopes != null && !Enumerable.SequenceEqual(request.Scopes, userRecord.Scopes))
                    {
                        await _changeService.LogChange(audit.Item2, "Scopes", ParameterFunction.FormatParameters(userRecord.Scopes.ToList<object>()), ParameterFunction.FormatParameters(request.Scopes.ToList<object>()));
                        userRecord.Scopes = request.Scopes;
                    }

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = userRecord
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"),
                        AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, new string[] { id.ToString(), request.Username, HashFunction.HashString(request.Password), ParameterFunction.FormatParameters(request.Scopes?.ToList<object>(), true) });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                username, applicationName, new string[] { id.ToString(), request.Username, HashFunction.HashString(request.Password), ParameterFunction.FormatParameters(request.Scopes?.ToList<object>(), true) });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No user exists with the given id."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     Delete /user/1
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        /// <param name="id">The id number of the user.</param>
        [RequiredPolicyAuthorisationAttributeFilter("User.Delete")]
        [VersionedRoute("user/{id:int}", "1.1")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "Returns a confirmation that the user was deleted.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no user was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);
            ChangeService _changeService = new ChangeService(_Logger, _FileSystem, _Options, _Database);

            ClaimsPrincipal principal = RequestContext.Principal as ClaimsPrincipal;
            string username = ClaimFunction.GetUsername(principal);
            string applicationName = ClaimFunction.GetApplicationName(principal);

            ResponseModel response;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint called with the following parameters \"{id}\".");

            if (await _userService.UserExists(id))
            {
                if (await _userService.UserDeleted(id))
                {
                    (bool, int) audit = await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"), AuditHistoryConverter.GetStatusID("OK"),
                        username, applicationName, new string[] { id.ToString() });

                    await _changeService.LogChange(audit.Item2, "IsDeleted", "0", "1");

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            information = "The given user has been deleted."
                        }
                    };

                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"),
                        AuditHistoryConverter.GetStatusID("InternalServerError"), username, applicationName, new string[] { id.ToString() });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, AuditHistoryConverter.GetEndpointID("user"), AuditHistoryConverter.GetEndpointVersionID(AuditHistoryFunction.ExtractVersionFromRequest(Request)), AuditHistoryConverter.GetMethodID("DELETE"), AuditHistoryConverter.GetStatusID("NotFound"),
                username, applicationName, new string[] { id.ToString() });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No user exists with the given id."
                }
            };

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {ResponseFunction.GetModelJSON(response.Data)}.");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
