// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestLocation([FromQuery] AssistantFilterModel filters)
        {
            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantID == null)
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid or no filters provided."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the location from the AssistantInformation table.
            var Result = GetAssistantLocation(filters.AssistantName, filters.AssistantID);

            // Checks if data was returned.
            if (Result.Item1 == "")
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            return Ok(new
            {
                assistantname = Result.Item1,
                assistantid = Result.Item2,
                hostname = Result.Item3,
                ipaddress = Result.Item4
            });
        }

        [HttpPatch]
        public IActionResult UpdateLocation([FromBody] LocationModel request, [FromQuery] AssistantFilterModel filters)
        {
            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantID) || (string.IsNullOrWhiteSpace(request.HostName) && string.IsNullOrEmpty(request.IPAddress)))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                return BadRequest(new
                {
                    error = "Filter or body parameters missing."
                });
            }

            // Checks if a config exists.
            if (ConfigController.AssistantExists(filters.AssistantName, filters.AssistantID))
            {
                var currentLocation = GetAssistantLocation(filters.AssistantName, filters.AssistantID);

                // Updates the location and returns the result.
                if (AssistantLocationUpdated(filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress))
                {
                    var auditID = AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                    if (!string.IsNullOrEmpty(request.HostName) && request.HostName != currentLocation.Item3)
                    {
                        ChangeLogger.LogChange(AuditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "Host Name", currentLocation.Item3, request.HostName);
                    }

                    if (!string.IsNullOrEmpty(request.IPAddress) && request.IPAddress != currentLocation.Item4)
                    {
                        ChangeLogger.LogChange(AuditHistoryConverter.GetEndpointID("assistant/location"), auditID.Item2, "IP Address", currentLocation.Item4, request.IPAddress);
                    }

                    // Gets the location from the AssistantInformation table.
                    var Result = GetAssistantLocation(filters.AssistantName, filters.AssistantID);

                    return Ok(new
                    {
                        assistantname = Result.Item1,
                        assistantid = Result.Item2,
                        hostname = Result.Item3,
                        ipaddress = Result.Item4
                    });
                }

                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("PATCH"),
                    AuditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/location"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.HostName, request.IPAddress });

            return StatusCode(404, new
            {
                information = "No config exists with the given parameters."
            });
        }

        //* SQL *//

        // Gets the host name and ip address of the given assistant.
        private static (string, string, string, string) GetAssistantLocation(string assistantName, string assistantID)
        {
            try
            {
                string? Name = null;
                string? ID = null;
                string? HostName = null;
                string? IPAddress = null;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name, AI.IDNumber, L.HostName, L.IPAddress from AssistantInformation AI
join Location L on AI.LocationID = L.LocationID
where AI.Name = @AssistantName
and AI.IDNumber = @AssistantID";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantID));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    Name = dataReader.GetString(0);
                    ID = dataReader.GetString(1);
                    HostName = dataReader.GetString(2);
                    IPAddress = dataReader.GetString(3);
                }

                dataReader.Close();
                connection.Close();

                return (Name, ID, HostName, IPAddress);
            }

            catch (Exception ex)
            {
                return (string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }

        // Updates the host name or ip address of the given assistant.
        private static bool AssistantLocationUpdated(string assistantName, string idNumber, string hostName, string ipAddress)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Updates the HostName or the IPAddress on the Location table.
                string sqlQuery = DatabaseModel.AssistantQueries[2];
                int rowsAffected;

                if (string.IsNullOrEmpty(hostName))
                {
                    sqlQuery = sqlQuery.Replace("HostName = @HostName, ", "");
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    sqlQuery = sqlQuery.Replace(", IPAddress = @IPAddress", "");
                }

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", idNumber));

                if (!string.IsNullOrEmpty(hostName))
                {
                    command.Parameters.Add(new SqlParameter("@HostName", hostName));
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                }

                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    connection.Close();
                    return false;
                }

                connection.Close();
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
