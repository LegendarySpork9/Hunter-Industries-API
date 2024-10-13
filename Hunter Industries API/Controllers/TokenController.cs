using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.Swagger.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Route("api/auth/token")]
    public class TokenController : ApiController
    {
        /// <summary>
        /// Creates a bearer token.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /token
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
        public IHttpActionResult Post([FromBody, Required] AuthenticationModel request)
        {
            LoggerService _logger = new LoggerService(HttpContext.Current.Request.UserHostAddress);
            AuditHistoryService _auditHistoryService = new AuditHistoryService(_logger);
            AuditHistoryConverter _auditHistoryConverter = new AuditHistoryConverter();
            ModelValidationService _modelValidator = new ModelValidationService();

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

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint called with the following parameters {_logger.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true, validationIgnore))
            {
                auditId = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"), null).Item2;

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present."
                    }
                };

                _auditHistoryService.LogLoginAttempt(auditId, false);
                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            TokenService _tokenService = new TokenService(request.Phrase, _logger);

            (request.Username, request.Password) = _tokenService.ExtractCredentialsFromBasicAuth(request.AuthHeader.ToString());

            if (!_modelValidator.IsValid(request, true))
            {
                auditId = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

                response = new ResponseModel()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or malformed basic authentication header."
                    }
                };

                _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
                return Content(HttpStatusCode.BadRequest, response.Data);
            }

            if (_tokenService.IsValidUser(request.Username, request.Password, request.Phrase))
            {
                var claims = _tokenService.GetClaims(request.Username);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                    ValidationModel.Issuer,
                    ValidationModel.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds
                ));

                auditId = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

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
                            ApplicationName = _tokenService.ApplicationName(),
                            Scope = claims.Where(c => c.Type == "scope").Select(c => c.Value),
                            Issued = DateTime.Now,
                            Expires = DateTime.Now.AddMinutes(15)
                        }
                    }
                };

                _auditHistoryService.LogLoginAttempt(auditId, true, request.Username, request.Password, request.Phrase);
                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
                return Content(HttpStatusCode.OK, response.Data);
            }

            auditId = _auditHistoryService.LogRequest(HttpContext.Current.Request.UserHostAddress, _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Unauthorized"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

            response = new ResponseModel()
            {
                StatusCode = 401,
                Data = new
                {
                    error = "Invalid credentials or phrase provided."
                }
            };

            _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
            return Content(HttpStatusCode.Unauthorized, response.Data);
        }
    }
}