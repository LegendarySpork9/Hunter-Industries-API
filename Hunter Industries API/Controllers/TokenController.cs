// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.User;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [VersionedRoute("auth/token", "1.0")]
    public class TokenController : ApiController
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabase _Database;
        private readonly IDatabaseOptions _Options;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public TokenController(ILoggerService _logger,
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
        /// Creates a bearer token.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///
        ///     POST /auth/token
        ///     Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ=
        ///     Content-Type: application/json
        ///     {
        ///         "phrase": "Some wise words or something here."
        ///     }
        /// </remarks>
        /// <param name="request">The application identifier.</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel), Description = "Returns the bearer token and token information.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ResponseModel), Description = "If the body or header is invalid.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(ResponseModel), Description = "If the details given do not match anything in the database.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(ResponseModel), Description = "If something went wrong on the server.")]
        public async Task<IHttpActionResult> Post([FromBody, Required] AuthenticationModel request)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_Logger, _FileSystem, _Options, _Database, _Clock);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();
            ResponseFunction _responseFunction = new ResponseFunction();

            ResponseModel response = new ResponseModel();

            if (request == null)
            {
                request = new AuthenticationModel();
            }

            if (Request.Headers.Authorization != null)
            {
                request.AuthHeader = new Microsoft.Extensions.Primitives.StringValues($"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}");
            }

            string[] validationIgnore = { "Username", "Password" };
            int auditId = 0;

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint called with the following parameters {_parameterFunction.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true, validationIgnore))
            {
                auditId = (await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"), _parameterFunction.FormatParameters(null, request))).Item2;

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present."
                    }
                };

                await _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            TokenService _tokenService = new TokenService(_Logger, _FileSystem, _Options, _Database, request.Phrase);
            TokenConverter _tokenConverter = new TokenConverter();
            TokenFunction _tokenFunction = new TokenFunction(_Logger);
            UserService _userService = new UserService(_Logger, _FileSystem, _Options, _Database);

            (request.Username, request.Password) = _tokenFunction.ExtractCredentialsFromBasicAuth(request.AuthHeader.ToString());

            if (!_modelValidator.IsValid(request, true))
            {
                auditId = (await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Username, request.Password, request.Phrase })).Item2;

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or malformed basic authentication header."
                    }
                };

                await _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            (string[] usernames, string[] passwords) = await _tokenService.GetUsers();
            string[] phrases = await _tokenService.GetAuthorisationPhrases();

            if (_tokenFunction.IsValidUser(usernames, passwords, phrases, request.Username, request.Password, request.Phrase))
            {
                Claim[] claims = _tokenConverter.GetClaims(await _userService.GetUserScopes(0, request.Username));

                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                    ValidationModel.Issuer,
                    ValidationModel.Audience,
                    claims: claims,
                    expires: _Clock.UtcNow.AddMinutes(15),
                    signingCredentials: creds
                ));

                auditId = (await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { request.Username, request.Password, request.Phrase })).Item2;

                response = new ResponseModel()
                {
                    StatusCode = 200,
                    Data = new TokenResponseModel()
                    {
                        Type = "Bearer",
                        Token = token,
                        ExpiresIn = 900,
                        Info = new TokenInfo()
                        {
                            ApplicationName = await _tokenService.ApplicationName(),
                            Scope = claims.Where(c => c.Type == "scope").Select(c => c.Value),
                            Issued = _Clock.UtcNow,
                            Expires = _Clock.UtcNow.AddMinutes(15)
                        }
                    }
                };

                await _auditHistoryService.LogLoginAttempt(auditId, true, request.Username, request.Password, request.Phrase);
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            auditId = (await _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Unauthorized"),
                    new string[] { request.Username, request.Password, request.Phrase })).Item2;

            response = new ResponseModel()
            {
                StatusCode = 401,
                Data = new
                {
                    error = "Invalid credentials or phrase provided."
                }
            };

            await _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {_responseFunction.GetModelJSON(response.Data)}");
            return Content(HttpStatusCode.Unauthorized, response.Data);
        }
    }
}