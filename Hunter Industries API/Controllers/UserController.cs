using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
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
    }
}
