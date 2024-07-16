// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Controllers.Assistant
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AIAccess")]
    [Route("api/assistant/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestConfig([FromQuery] AssistantFilterModel filters)
        {
            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.AssistantName, filters.AssistantID });

            // Gets the config(s) from the AssistantInformation table.
            var Result = GetAssistantConfig(filters.AssistantName, filters.AssistantID);
            string[] AssistantNames = Result.Item1;

            // Checks if data was returned.
            if (AssistantNames == Array.Empty<string>())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            var FormattedRecords = FormatData(Result.Item1, Result.Item2, Result.Item3, Result.Item4, Result.Item5, Result.Item6);
            int TotalRecords = Result.Item7;

            return Ok(new
            {
                latestrelease = Result.Item8,
                configs = FormattedRecords,
                configcount = Result.Item1.Length,
                totalcount = TotalRecords
            });
        }

        [HttpPost]
        public IActionResult CreateConfig([FromBody] ConfigModel request)
        {
            // Checks if the request contains a body.
            if (request == null)
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    null);

                return BadRequest(new
                {
                    error = "Invalid or no body provided."
                });
            }

            // Checks whether all requireds are present.
            if (string.IsNullOrWhiteSpace(request.AssistantName) || string.IsNullOrWhiteSpace(request.IDNumber) || string.IsNullOrWhiteSpace(request.AssignedUser) || string.IsNullOrWhiteSpace(request.HostName))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("BadRequest"),
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return BadRequest(new
                {
                    error = "Body parameters missing."
                });
            }

            // Checks if a config already exists.
            if (AssistantExists(request.AssistantName, request.IDNumber))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("POST"),
                    AuditHistoryConverter.GetStatusID("OK"), new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return Ok(new
                {
                    information = "A config with the name and ID already exists."
                });
            }

            // Creates the config and returns the result.
            if (!AssistantConfigCreated(request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName))
            {
                AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("POST"),
                    AuditHistoryConverter.GetStatusID("InternalServerError"), new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

                return StatusCode(500, new
                {
                    error = "An error occured when running an insert statement. Please raise this with the time the error occured so it can be investigated."
                });
            }

            AuditController.LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("assistant/config"), AuditHistoryConverter.GetMethodID("POST"), AuditHistoryConverter.GetStatusID("Created"),
                    new string[] { request.AssistantName, request.IDNumber, request.AssignedUser, request.HostName });

            string Version = GetMostRecentVersion();

            return StatusCode(201, new
            {
                assistantname = request.AssistantName,
                assistantid = request.IDNumber,
                designateduser = request.AssignedUser,
                hostname = request.HostName,
                deletion = false,
                version = Version
            });
        }

        // Formats the returned data.
        private static List<object> FormatData(string[] AssistantNames, string[] AssistantIDs, string[] UserNames, string[] HostNames, bool[] Deletions, string[] Versions)
        {
            var AssistantConfigs = new List<object>();

            for (int x = 0; x < AssistantNames.Length; x++)
            {
                var AssistantConfigRecord = new
                {
                    assistantname = AssistantNames[x],
                    assistantid = AssistantIDs[x],
                    designateduser = UserNames[x],
                    hostname = HostNames[x],
                    deletion = Deletions[x],
                    version = Versions[x]
                };

                AssistantConfigs.Add(AssistantConfigRecord);
            }

            return AssistantConfigs;
        }

        //* SQL *//

        // Gets the config(s) from the database.
        private static (string[], string[], string[], string[], bool[], string[], int, string) GetAssistantConfig(string assistantName, string assistantID)
        {
            try
            {
                string[] AssistantNames = Array.Empty<string>();
                string[] AssistantIDs = Array.Empty<string>();
                string[] UserNames = Array.Empty<string>();
                string[] HostNames = Array.Empty<string>();
                bool[] Deletions = Array.Empty<bool>();
                string[] Versions = Array.Empty<string>();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = $@"{DatabaseModel.AssistantQueries[0]}
where AI.Name is not null";

                if (!string.IsNullOrEmpty(assistantName))
                {
                    sqlQuery += "\nand AI.Name = @AssistantName";
                }

                if (!string.IsNullOrEmpty(assistantID))
                {
                    sqlQuery += "\nand AI.IDNumber = @AssistantID";
                }

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);

                if (sqlQuery.Contains("@AssistantName"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                }

                if (sqlQuery.Contains("@AssistantID"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantID", assistantID));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    AssistantNames = AssistantNames.Append(dataReader.GetString(0)).ToArray();
                    AssistantIDs = AssistantIDs.Append(dataReader.GetString(1)).ToArray();
                    UserNames = UserNames.Append(dataReader.GetString(2)).ToArray();
                    HostNames = HostNames.Append(dataReader.GetString(3)).ToArray();
                    Deletions = Deletions.Append(Convert.ToBoolean(dataReader.GetString(4))).ToArray();
                    Versions = Versions.Append(dataReader.GetString(5)).ToArray();
                }

                dataReader.Close();
                connection.Close();

                return (AssistantNames, AssistantIDs, UserNames, HostNames, Deletions, Versions, GetTotalConfigs(command), GetMostRecentVersion());
            }

            catch (Exception ex)
            {
                return (Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<bool>(), Array.Empty<string>(), 0, string.Empty);
            }
        }

        // Gets the total number of records in the AssistantInformation table.
        private static int GetTotalConfigs(SqlCommand command)
        {
            try
            {
                int totalRecords = 0;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlDataReader dataReader;

                // Obtaines and returns the number of rows in the AuditHistory table.
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command.Connection = connection;
                command.CommandText = command.CommandText.Replace(DatabaseModel.AssistantQueries[0], "select count(*) from AssistantInformation AI");
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    totalRecords = dataReader.GetInt32(0);
                }

                dataReader.Close();
                connection.Close();

                return totalRecords;
            }

            catch (Exception ex)
            {
                return 0;
            }
        }

        // Gets the most recent release version in the Versions table.
        private static string GetMostRecentVersion()
        {
            try
            {
                string version = string.Empty;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns the latest version.
                string sqlQuery = @"select top 1 Value from [Version]
order by VersionID desc";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    version = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();

                return version;
            }

            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        // Checks whether the given assistant already exists in the table.
        public static bool AssistantExists(string assistantName, string assistantID)
        {
            try
            {
                string? assistant = null;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = $@"{DatabaseModel.AssistantQueries[0]}
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
                    assistant = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();

                if (string.IsNullOrEmpty(assistant))
                {
                    return false;
                }

                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        // Inserts the new config into the relevant tables.
        private static bool AssistantConfigCreated(string assistantName, string idNumber, string assignedUser, string hostName)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Inserts the values into the relevant tables to create the config.
                string sqlQuery = DatabaseModel.AssistantControlPanelQueries[0];
                int rowsAffected;

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Hostname", hostName));
                command.Parameters.Add(new SqlParameter("@IPAddress", "PlaceHolder"));
                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    connection.Close();
                    return false;
                }

                sqlQuery = DatabaseModel.AssistantControlPanelQueries[2];
                rowsAffected = 0;

                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Name", assignedUser));
                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    connection.Close();
                    return false;
                }

                sqlQuery = DatabaseModel.AssistantControlPanelQueries[3];
                rowsAffected = 0;

                command = new SqlCommand(sqlQuery, connection);
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
