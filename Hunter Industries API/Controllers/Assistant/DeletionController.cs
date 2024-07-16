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
    public class DeletionController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestDeletion([FromQuery] AssistantFilterModel filters)
        {
            // Checks if the request contains the needed filters.
            if (filters.AssistantName == null || filters.AssistantID == null)
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid or no filters provided."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the deletion status from the AssistantInformation table.
            var Result = GetAssistantDeletion(filters.AssistantName, filters.AssistantID);

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
                deletion = Result.Item3
            });
        }

        [HttpPatch]
        public IActionResult UpdateDeletion([FromBody] DeletionModel request, [FromQuery] AssistantFilterModel filters)
        {
            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(filters.AssistantName) || string.IsNullOrWhiteSpace(filters.AssistantID) || string.IsNullOrWhiteSpace(request.Deletion.ToString()))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Deletion.ToString() });

                return BadRequest(new
                {
                    error = "Filter or body parameters missing."
                });
            }

            // Checks if a config exists.
            if (ConfigController.AssistantExists(filters.AssistantName, filters.AssistantID))
            {
                var currentDeletion = GetAssistantDeletion(filters.AssistantName, filters.AssistantID);

                // Updates the deletion status and returns the result.
                if (AssistantDeletionUpdated(filters.AssistantName, filters.AssistantID, (bool)request.Deletion))
                {
                    var auditID = AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Deletion.ToString() });

                    if (request.Deletion.ToString() != currentDeletion.Item2)
                    {
                        ChangeLogger.LogChange(AuditHistoryConverter.GetEndpointID("assistant/deletion"), auditID.Item2, "Deletion", currentDeletion.Item3.ToString(), request.Deletion.ToString());
                    }

                    // Gets the deletion status from the AssistantInformation table.
                    var Result = GetAssistantDeletion(filters.AssistantName, filters.AssistantID);

                    return Ok(new
                    {
                        assistantname = Result.Item1,
                        assistantid = Result.Item2,
                        deletion = Result.Item3
                    });
                }

                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("PATCH"),
                    AuditHistoryConverter.GetStatusID("InternalServerError"), new string[] { filters.AssistantName, filters.AssistantID, request.Deletion.ToString() });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/deletion"), AuditHistoryConverter.GetMethodID("PATCH"), AuditHistoryConverter.GetStatusID("NotFound"),
                    new string[] { filters.AssistantName, filters.AssistantID, request.Deletion.ToString() });

            return StatusCode(404, new
            {
                information = "No config exists with the given parameters."
            });
        }

        //* SQL *//

        // Gets the deletion status of the given assistant.
        private static (string, string, bool) GetAssistantDeletion(string? assistantName, string? assistantID)
        {
            try
            {
                string? Name = null;
                string? ID = null;
                bool Deletion = false;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name, AI.IDNumber, D.Value from AssistantInformation AI
join Deletion D on AI.DeletionStatusID = D.StatusID
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
                    Deletion = Convert.ToBoolean(dataReader.GetString(2));
                }

                dataReader.Close();
                connection.Close();

                return (Name, ID, Deletion);
            }

            catch (Exception ex)
            {
                return (string.Empty, string.Empty, false);
            }
        }

        // Updates the deletion status of the given assistant.
        private static bool AssistantDeletionUpdated(string assistantName, string idNumber, bool deletion)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Updates the deletionStatusID column on the AssistantInformation table.
                string sqlQuery = DatabaseModel.AssistantControlPanelQueries[4];
                int rowsAffected;

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Deletion", deletion));
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
