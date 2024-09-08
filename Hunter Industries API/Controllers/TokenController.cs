// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HunterIndustriesAPI.Controllers
{
    [Route("api/auth/[controller]")]
    public class TokenController : ControllerBase
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
        ///         "Phrase": "Some wise words or something here."
        ///     }
        /// </remarks>
        /// <response code="200">Returns the bearer token and token information.</response>
        /// <response code="400">If the body or header is invalid.</response>
        /// <response code="401">If the details given do not match anything in the database.</response>
        /// <response code="500">If something went wrong on the server.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult RequestToken([FromBody, Required] AuthenticationModel request)
        {
            LoggerService _logger = new(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            AuditHistoryService _auditHistoryService = new(_logger);
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();

            ResponseModel response = new();

            if (request == null)
            {
                request = new AuthenticationModel();
            }

            request.AuthHeader = Request.Headers["Authorization"];
            string[] validationIgnore = { "Username", "Password" };
            int auditId = 0;

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint called with the following parameters {_logger.FormatParameters(request)}.");

            if (!_modelValidator.IsValid(request, true, validationIgnore))
            {
                auditId = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"), null).Item2;

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present."
                    }
                };

                _auditHistoryService.LogLoginAttempt(auditId, false);
                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
            }

            TokenService _tokenService = new(request.Phrase, _logger);

            (request.Username, request.Password) = _tokenService.ExtractCredentialsFromBasicAuth(request.AuthHeader.ToString());

            if (!_modelValidator.IsValid(request, true))
            {
                auditId = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or malformed basic authentication header."
                    }
                };

                _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
                _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
                return StatusCode(response.StatusCode, response.Data);
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

                auditId = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

                response = new()
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
                return StatusCode(response.StatusCode, response.Data);
            }

            auditId = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Unauthorized"),
                    new string[] { request.Username, request.Password, request.Phrase }).Item2;

            response = new()
            {
                StatusCode = 401,
                Data = new
                {
                    error = "Invalid credentials or phrase provided."
                }
            };

            _auditHistoryService.LogLoginAttempt(auditId, false, request.Username, request.Password, request.Phrase);
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Token endpoint returned a {response.StatusCode} with the data {response.Data}");
            return StatusCode(response.StatusCode, response.Data);
        }
    }
}