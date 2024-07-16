// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestVersion([FromQuery] AssistantFilterModel filters)
        {
            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantID == null)
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid or no filters provided."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the version from the AssistantInformation table.
            var Result = GetAssistantVersion(filters.AssistantName, filters.AssistantID);

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
                assistantName = Result.Item1,
                assistantID = Result.Item2,
                version = Result.Item3
            });
        }

        [HttpPatch]
        public IActionResult UpdateVersion([FromBody] VersionModel request, [FromQuery] AssistantFilterModel filters)
        {
            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantID) || string.IsNullOrWhiteSpace(request.Version))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                return BadRequest(new
                {
                    error = "Filter or body parameters missing."
                });
            }

            // Checks if a config exists.
            if (ConfigController.AssistantExists(filters.AssistantName, filters.AssistantID))
            {
                var currentVersion = GetAssistantVersion(filters.AssistantName, filters.AssistantID);

                // Updates the version and returns the result.
                if (AssistantVersionUpdated(filters.AssistantName, filters.AssistantID, request.Version))
                {
                    var auditID = AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                    if (request.Version != currentVersion.Item2)
                    {
                        ChangeLogger.LogChange(AuditHistoryConverter.GetEndpointID("assistant/version"), auditID.Item2, "Version", currentVersion.Item3, request.Version);
                    }

                    // Gets the version from the AssistantInformation table.
                    var Result = GetAssistantVersion(filters.AssistantName, filters.AssistantID);

                    return Ok(new
                    {
                        assistantName = Result.Item1,
                        assistantID = Result.Item2,
                        version = Result.Item3
                    });
                }

                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("PATCH"),
                    AuditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantID, request.Version });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/version"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Version });

            return StatusCode(404, new
            {
                information = "No config exists with the given parameters."
            });
        }

        //* SQL *//

        // Gets the version number of the given assistant.
        private static (string, string, string) GetAssistantVersion(string assistantName, string assistantID)
        {
            try
            {
                string? name = null;
                string? id = null;
                string? version = null;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name, AI.IDNumber, V.Value from AssistantInformation AI
join [Version] V on AI.VersionID = V.VersionID
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
                    name = dataReader.GetString(0);
                    id = dataReader.GetString(1);
                    version = dataReader.GetString(2);
                }

                dataReader.Close();
                connection.Close();

                return (name, id, version);
            }

            catch (Exception ex)
            {
                return (string.Empty, string.Empty, string.Empty);
            }
        }

        // Updates the version number of the given assistant.
        private static bool AssistantVersionUpdated(string assistantName, string idNumber, string version)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Updates the VersionID column on the AssistantInformation table.
                string sqlQuery = DatabaseModel.AssistantQueries[1];
                int rowsAffected;

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Version", version));
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", idNumber));
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
