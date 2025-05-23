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
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers.User
{
    /// <summary>
    /// </summary>
    [Authorize]
    [RequiredPolicyAuthorisationAttributeFilter("APIControlPanel")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Returns a collection of users.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /User?Username=TestUser
        ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSElBUElBZG1pbiIsInNjb3BlIjpbIkFzc2lzdGFudCBBUEkiLCJBc3Npc3RhbnQgQ29udHJvbCBQYW5lbCBBUEkiLCJCb29rIFJlYWRlciBBUEkiXSwiZXhwIjoxNzA4MjgyMjQ3LCJpc3MiOiJodHRwczovL2h1bnRlci1pbmR1c3RyaWVzLmNvLnVrL2FwaS9hdXRoL3Rva2VuIiwiYXVkIjoiSHVudGVyIEluZHVzdHJpZXMgQVBJIn0.tvIecko1tNnFvASv4fgHvUptUzaM7FofSF8vkqqOg0s
        /// </remarks>
        [Route("api/user")]
        [SwaggerOperation("GetUserList")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<UserRecord>), Description = "Returns the item(s) matching the given parameters.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the filters are invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Get([FromUri] UserFilterModel filters)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            if (filters == null)
            {
                filters = new UserFilterModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint called with the following parameters {_parameterFunction.FormatParameters(filters)}.");

            if (!_modelValidator.IsValid(filters))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("BadRequest"), _parameterFunction.FormatParameters(null, filters));

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.Id.ToString(), filters.Username });

            List<UserRecord> users = _userService.GetUsers(filters.Id, filters.Username);

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

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = users
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [Route("api/user/{id:int}")]
        [SwaggerOperation("GetUserById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserRecord), Description = "Returns the item matching the given id.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Get(int id)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint called with the following parameters {_parameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { id.ToString() });

            List<UserRecord> users = _userService.GetUsers(id, null);

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

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            response = new ResponseModel()
            {
                StatusCode = 200,
                Data = users[0]
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (get) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [Route("api/user")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "If the a user matching the username already exists.")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(UserRecord), Description = "If the user is successfuly created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Post([FromBody, Required] UserModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();
            HashFunction _hashFunction = new HashFunction();

            ResponseModel response;

            if (request == null)
            {
                request = new UserModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_userService.UserExists(request.Username))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "A user with the username already exists."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            (bool created, int id) = _userService.UserCreated(request.Username, request.Password);

            if (!created)
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            created = _userService.UserScopeCreated(id, request.Scopes);

            if (!created)
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("InternalServerError"), _parameterFunction.FormatParameters(null, request));

                response = new ResponseModel()
                {
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Created"), _parameterFunction.FormatParameters(null, request));

            response = new ResponseModel()
            {
                StatusCode = 201,
                Data = new UserRecord()
                {
                    Id = id,
                    Username = request.Username,
                    Password = _hashFunction.HashString(request.Password),
                    Scopes = request.Scopes
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Post) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [Route("api/user/{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserRecord), Description = "Returns the updated item.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no user was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Patch(int id, [FromBody, Required] UserModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            UserService _userService = new UserService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();
            UserFunction _userFunction = new UserFunction();
            HashFunction _hashFunction = new HashFunction();
            ChangeService _changeService = new ChangeService(_logger);

            ResponseModel response;

            if (request == null)
            {
                request = new UserModel();
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint called with the following parameters \"{id}\", {_parameterFunction.FormatParameters(null, request)}.");

            if (!_modelValidator.IsValid(request))
            {
                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the correct tags."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_userService.UserExists(id))
            {
                UserRecord userRecord = _userService.GetUsers(id, null)[0];

                if (_userService.UserUpdated(id, request.Username, _hashFunction.HashString(request.Password), _userFunction.GetScopesUpdateList(userRecord.Scopes, request.Scopes)))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"),
                        new string[] { id.ToString(), request.Username, _hashFunction.HashString(request.Password), _parameterFunction.FormatParameters(request.Scopes.ToList<object>(), true) });

                    if (!string.IsNullOrEmpty(request.Username) && request.Username != userRecord.Username)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("user"), auditID.Item2, "Username", userRecord.Username, request.Username);
                        userRecord.Username = request.Username;
                    }

                    if (!string.IsNullOrEmpty(request.Password) && _hashFunction.HashString(request.Password) != userRecord.Password)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("user"), auditID.Item2, "Password", userRecord.Password, _hashFunction.HashString(request.Password));
                        userRecord.Password = _hashFunction.HashString(request.Password);
                    }

                    if (request.Scopes != null && !Enumerable.SequenceEqual(request.Scopes, userRecord.Scopes))
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("user"), auditID.Item2, "Scopes", _parameterFunction.FormatParameters(userRecord.Scopes.ToList<object>()), _parameterFunction.FormatParameters(request.Scopes.ToList<object>()));
                        userRecord.Scopes = request.Scopes;
                    }

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = userRecord
                    };

                    _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("PATCH"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { id.ToString(), request.Username, _hashFunction.HashString(request.Password), _parameterFunction.FormatParameters(request.Scopes.ToList(), true) });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                new string[] { id.ToString(), request.Username, _hashFunction.HashString(request.Password), _parameterFunction.FormatParameters(request.Scopes.ToList<object>(), true) });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No user exists with the given id."
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Patch) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
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
        [Route("api/user/{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ResponseModel), Description = "Returns a confirmation that the user was deleted.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the bearer token is expired or fails validation.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(ResponseModel), Description = "If no user was found matching the id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public IHttpActionResult Delete(int id)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            UserService _userService = new UserService(_logger);
            ResponseFunction _responseFunction = new ResponseFunction();
            ChangeService _changeService = new ChangeService(_logger);

            ResponseModel response;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint called with the following parameters \"{id}\".");

            if (_userService.UserExists(id))
            {
                if (_userService.UserDeleted(id))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("DELETE"), _auditHistoryConverter.GetStatusID("OK"),
                        new string[] { id.ToString() });

                    _changeService.LogChange(_auditHistoryConverter.GetEndpointID("user"), auditID.Item2, "IsDeleted", "0", "1");

                    response = new ResponseModel()
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            information = "The given user has been deleted."
                        }
                    };

                    _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                    return Content(HttpStatusCode.OK, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("DELETE"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { id.ToString() });

                response = new ResponseModel()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.InternalServerError, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("user"), _auditHistoryConverter.GetMethodID("DELETE"), _auditHistoryConverter.GetStatusID("NotFound"),
                new string[] { id.ToString() });

            response = new ResponseModel()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No user exists with the given id."
                }
            };

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"User (Delete) endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.NotFound, response.Data);
        }
    }
}
