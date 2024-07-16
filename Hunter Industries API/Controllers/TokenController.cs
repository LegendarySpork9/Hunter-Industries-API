// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace HunterIndustriesAPI.Controllers
{
    [Route("api/auth/[controller]")]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult RequestToken([FromBody] AuthenticationModel request)
        {
            // Checks if the request contains a body.
            if (request == null)
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid or no body provided."
                });
            }

            // Checks the number of headers on the request.
            var authHeader = Request.Headers["Authorization"];
            if (authHeader.Count == 0 || string.IsNullOrEmpty(authHeader[0]))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Phrase });

                return BadRequest(new
                {
                    error = "Authorisation header is missing or empty."
                });
            }

            // Checks if the phrase variable contains a string.
            if (string.IsNullOrWhiteSpace(request.Phrase))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Phrase });

                return BadRequest(new
                {
                    error = "No phrase provided."
                });
            }

            TokenService _tokenService = new(request.Phrase);

            // Obtains the headers on the request.
            var authHeadervalue = authHeader[0];
            var (username, password) = _tokenService.ExtractCredentialsFromBasicAuth(authHeadervalue);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { username, password, request.Phrase });

                return BadRequest(new
                {
                    error = "Invalid or malformed basic authentication header."
                });
            }

            // Checks if the user is valid.
            if (_tokenService.IsValidUser(username, password, request.Phrase))
            {
                // Creates the token.
                var claims = _tokenService.GetClaims(username, request.Phrase);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationModel.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                    ValidationModel.Issuer,
                    ValidationModel.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds
                ));

                TokenResponseModel response = new()
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
                };

                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { username, password, request.Phrase });

                // Returns the token.
                return StatusCode(200, response);
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("token"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Unauthorized"),
                new string[] { username, password, request.Phrase });

            return Unauthorized(new
            {
                error = "Invalid credentials or phrase provided."
            });

        }
    }
}