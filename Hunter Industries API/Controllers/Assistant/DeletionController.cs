// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPI.Services.Assistant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class DeletionController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestDeletion([FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            DeletionService _deletionService = new();

            ResponseModel response = new();
            bool responseGiven = false;

            // Checks if the request contains the needed filters.
            if (!_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"), 
                    new string[] { filters.AssistantName, filters.AssistantId });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Invalid or no filters provided."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("GET"), _auditHistoryConverter.GetStatusID("OK"),
                   new string[] { filters.AssistantName, filters.AssistantId });

            // Gets the deletion status from the Assistant_Information table.
            DeletionResponseModel deletionResponse = _deletionService.GetAssistantDeletion(filters.AssistantName, filters.AssistantId);

            // Checks if data was returned.
            if (deletionResponse == new DeletionResponseModel())
            {
                response = new()
                {
                    StatusCode = 200,
                    Data = new
                    {
                        information = "No data returned by given parameters."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            response = new()
            {
                StatusCode = 200,
                Data = deletionResponse
            };

            return StatusCode(response.StatusCode, response.Data);
        }

        [HttpPatch]
        public IActionResult UpdateDeletion([FromBody] DeletionModel request, [FromQuery] AssistantFilterModel filters)
        {
            AuditHistoryService _auditHistoryService = new();
            AuditHistoryConverter _auditHistoryConverter = new();
            ModelValidationService _modelValidator = new();
            ConfigService _configService = new();
            DeletionService _deletionService = new();
            ChangeService _changeService = new();

            ResponseModel response = new();

            // Checks whether all requireds are present.
            if (!_modelValidator.IsValid(request) || !_modelValidator.IsValid(filters, true))
            {
                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.Deletion.ToString() });

                response = new()
                {
                    StatusCode = 400,
                    Data = new
                    {
                        error = "Filter or body parameters missing."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            // Checks if a config exists.
            if (_configService.AssistantExists(filters.AssistantName, filters.AssistantId))
            {
                DeletionResponseModel deletionResponse = _deletionService.GetAssistantDeletion(filters.AssistantName, filters.AssistantId);

                // Updates the deletion status and returns the result.
                if (_deletionService.AssistantDeletionUpdated(filters.AssistantName, filters.AssistantId, bool.Parse(request.Deletion.ToString())))
                {
                    var auditID = _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.Deletion.ToString() });

                    if (request.Deletion != deletionResponse.Deletion)
                    {
                        _changeService.LogChange(_auditHistoryConverter.GetEndpointID("assistant/deletion"), auditID.Item2, "Deletion", deletionResponse.Deletion.ToString(), request.Deletion.ToString());
                    }

                    deletionResponse.Deletion = request.Deletion;

                    response = new()
                    {
                        StatusCode = 200,
                        Data = deletionResponse
                    };

                    return StatusCode(response.StatusCode, response.Data);
                }

                _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("PATCH"),
                        _auditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantId, request.Deletion.ToString() });

                response = new()
                {
                    StatusCode = 500,
                    Data = new
                    {
                        error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                    }
                };

                return StatusCode(response.StatusCode, response.Data);
            }

            _auditHistoryService.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), _auditHistoryConverter.GetEndpointID("assistant/deletion"), _auditHistoryConverter.GetMethodID("PATCH"), _auditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantId, request.Deletion.ToString() });

            response = new()
            {
                StatusCode = 404,
                Data = new
                {
                    information = "No config exists with the given parameters."
                }
            };

            return StatusCode(response.StatusCode, response.Data);
        }
    }
}
