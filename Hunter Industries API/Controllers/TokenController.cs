﻿// Copyright © - unpublished - Toby Hunter
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
            ModelValidationService _modelValidator = new();

            ResponseModel response = new();

            request.AuthHeader = Request.Headers["Authorization"];
            string[] validationIgnore = { "Username", "Password" };

            // Checks if the request is valid.
            if (!_modelValidator.IsValid(request, true, validationIgnore))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"), null);

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid request, check the following. A body is provided with the 'Phrase' tag and the authorisation header is present."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            TokenService _tokenService = new(request.Phrase);

            // Obtains the headers on the request.
            (request.Username, request.Password) = _tokenService.ExtractCredentialsFromBasicAuth(request.AuthHeader.ToString());

            if (!_modelValidator.IsValid(request, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.Username, request.Password, request.Phrase });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or malformed basic authentication header."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            // Checks if the user is valid.
            if (_tokenService.IsValidUser(request.Username, request.Password, request.Phrase))
            {
                // Creates the token.
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

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { request.Username, request.Password, request.Phrase });

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

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("token"), _auditHistoryConverter.GetMethodID("POST"), _auditHistoryConverter.GetStatusID("Unauthorized"),
                    new string[] { request.Username, request.Password, request.Phrase });

            response = new()
            {
                StatusCode = 401,
                Data = new
                {
                    error = "Invalid credentials or phrase provided."
                }
            };

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}