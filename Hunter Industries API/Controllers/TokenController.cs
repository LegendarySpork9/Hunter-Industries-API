// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HunterIndustriesAPI.Controllers
{
    [Route("api/auth/[controller]")]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult RequestToken([FromBody] AuthenticationModel request)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();

            ResponseModel response = new();
            bool responseGiven = false;

            // Checks if the request contains a body.
            if (request == null)
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"), null);

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no body provided."
                    }
                };
            }

            // Checks the number of headers on the request.
            var authHeader = Request.Headers["Authorization"];
            if (!responseGiven && (authHeader.Count == 0 || string.IsNullOrEmpty(authHeader[0])))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Phrase });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Authorisation header is missing or empty."
                    }
                };
            }

            // Checks if the phrase variable contains a string.
            if (!responseGiven && string.IsNullOrWhiteSpace(request.Phrase))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Phrase });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "No phrase provided."
                    }
                };
            }

            TokenService _tokenService = new(request.Phrase);

            // Obtains the headers on the request.
            var authHeaderValue = authHeader[0];
            var (username, password) = _tokenService.ExtractCredentialsFromBasicAuth(authHeaderValue);

            if (!responseGiven && (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { username, password, request.Phrase });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or malformed basic authentication header."
                    }
                };
            }

            // Checks if the user is valid.
            if (!responseGiven && _tokenService.IsValidUser(username, password, request.Phrase))
            {
                // Creates the token.
                var claims = _tokenService.GetClaims(username);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                    ValidationModel.Issuer,
                    ValidationModel.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds
                ));

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

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { username, password, request.Phrase });
            }

            if (!responseGiven && !_tokenService.IsValidUser(username, password, request.Phrase))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Unauthorized"),
                    new string[] { username, password, request.Phrase });

                response = new()
                {
                    StatusCode = 401,
                    Data = new
                    {
                        error = "Invalid credentials or phrase provided."
                    }
                };
            }

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}