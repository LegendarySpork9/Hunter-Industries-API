// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AssistantControlPanel")]
    [Route("api/audithistory")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestAuditHistory([FromQuery] AuditHistoryFilterModel filters)
        {
            // Checks if there are filter values.
            if (filters.PageSize == 0 || filters.PageNumber == 0)
            {
                LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("audithistory"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("BadRequest"), null);

                return BadRequest(new
                {
                    error = "Invalid Filters Provided."
                });
            }

            LogRequest(HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), AuditHistoryConverter.GetEndpointID("audithistory"), AuditHistoryConverter.GetMethodID("GET"), AuditHistoryConverter.GetStatusID("OK"),
                    new string[] { filters.IPAddress, filters.Endpoint, filters.FromDate.ToString(), filters.PageSize.ToString(), filters.PageNumber.ToString() });

            // Gets the data from the AuditHistory table.
            var result = GetAuditHistory(filters.IPAddress, filters.Endpoint, Convert.ToDateTime(filters.FromDate), filters.PageSize, filters.PageNumber);
            int[] auditIDs = result.Item1;

            // Checks if data was returned.
            if (auditIDs == Array.Empty<int>())
            {
                return Ok(new
                {
                    information = "No data returned by given parameters."
                });
            }

            var FormattedRecords = FormatData(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7);
            int TotalRecords = result.Item8;
            decimal TotalPages = (decimal)TotalRecords / (decimal)filters.PageSize;

            return Ok(new
            {
                entries = FormattedRecords,
                entrycount = result.Item1.Length,
                pagenumber = filters.PageNumber,
                pagesize = filters.PageSize,
                totalpagecount = Math.Ceiling(TotalPages),
                totalcount = TotalRecords
            });
        }

        // Formats the returned data.
        private static List<object> FormatData(int[] auditIDs, string[] ipAddresses, string[] endpoints, string[] methods, string[] status, DateTime[] occured, string[] parameters)
        {
            var auditHistoryrecords = new List<object>();

            for (int x = 0; x < ipAddresses.Length; x++)
            {
                var AuditHistoryRecord = new
                {
                    id = auditIDs[x],
                    ipaddress = ipAddresses[x],
                    endpoint = endpoints[x],
                    method = methods[x],
                    status = status[x],
                    occuredat = occured[x],
                    parameters = AuditHistoryConverter.FormatParameters(parameters[x])
                };

                auditHistoryrecords.Add(AuditHistoryRecord);
            }

            return auditHistoryrecords;
        }

        //* SQL *//

        // Logs the call to the AuditHistory table.
        public static (bool, int) LogRequest(string ipAddress, int endpointID, int methodID, int statusID, string[]? parameters)
        {
            try
            {
                string formattedParameters = DatabaseConverter.FormatParameters(parameters);

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Inserts the record into the AuditHistory table.
                string sqlQuery = @"insert into AuditHistory (IPAddress, EndpointID, MethodID, StatusID, DateOccured, [Parameters])
output inserted.AuditID
values (@IPAddress, @EndpointID, @MethodID, @StatusID, GetDate(), @Parameters)";

                if (formattedParameters == null)
                {
                    sqlQuery = @"insert into AuditHistory (IPAddress, EndpointID, MethodID, StatusID, DateOccured, [Parameters])
output inserted.AuditID
values (@IPAddress, @EndpointID, @MethodID, @StatusID, GetDate(), null)";
                }

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointID));
                command.Parameters.Add(new SqlParameter("@MethodID", methodID));
                command.Parameters.Add(new SqlParameter("@StatusID", statusID));

                if (formattedParameters != null)
                {
                    command.Parameters.Add(new SqlParameter("@Parameters", formattedParameters));
                }

                var result = command.ExecuteScalar();

                if (result == null)
                {
                    connection.Close();
                    return (false, 0);
                }

                connection.Close();
                return (true, (int)result);
            }

            catch (Exception ex)
            {
                return (false, 0);
            }
        }

        // Gets the audit history data from the database.
        private static (int[], string[], string[], string[], string[], DateTime[], string[], int) GetAuditHistory(string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
        {
            try
            {
                int[] auditIDs = Array.Empty<int>();
                string[] ipAddresses = Array.Empty<string>();
                string[] endpoints = Array.Empty<string>();
                string[] methods = Array.Empty<string>();
                string[] status = Array.Empty<string>();
                DateTime[] occured = Array.Empty<DateTime>();
                string[] parameters = Array.Empty<string>();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AuditHistory table.
                string sqlQuery = $@"{DatabaseModel.AssistantControlPanelQueries[5]}
where AH.AuditID is not null";

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sqlQuery += "\nand AH.IPAddress = @IPAddress";
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sqlQuery += "\nand E.Value = @Endpoint";
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate.ToString() != "01/01/0001 00:00:00")
                {
                    sqlQuery += "\nand AH.DateOccured > CAST(@FromDate as datetime)";
                }

                sqlQuery += @"
order by AH.AuditID asc
OFFSET (@PageSize * (@PageNumber - 1)) ROWS
FETCH NEXT @PageSize ROWS ONLY";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));

                if (sqlQuery.Contains("@IPAddress"))
                {
                    command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                }

                if (sqlQuery.Contains("@Endpoint"))
                {
                    command.Parameters.Add(new SqlParameter("@Endpoint", endpoint));
                }

                if (sqlQuery.Contains("@FromDate"))
                {
                    command.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    auditIDs = auditIDs.Append(dataReader.GetInt32(0)).ToArray();
                    ipAddresses = ipAddresses.Append(dataReader.GetString(1)).ToArray();
                    endpoints = endpoints.Append(dataReader.GetString(2)).ToArray();
                    methods = methods.Append(dataReader.GetString(3)).ToArray();
                    status = status.Append(dataReader.GetString(4)).ToArray();
                    occured = occured.Append(dataReader.GetDateTime(5)).ToArray();
                    parameters = parameters.Append(dataReader.GetString(6)).ToArray();
                }

                dataReader.Close();
                connection.Close();

                return (auditIDs, ipAddresses, endpoints, methods, status, occured, parameters, GetTotalAuditHistory(command));
            }

            catch (Exception ex)
            {
                return (Array.Empty<int>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<DateTime>(), Array.Empty<string>(), 0);
            }
        }

        // Gets the total number of records in the AuditHistory table.
        private static int GetTotalAuditHistory(SqlCommand command)
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
                command.CommandText = command.CommandText.Replace(DatabaseModel.AssistantControlPanelQueries[5], DatabaseModel.AssistantControlPanelQueries[6]);
                command.CommandText = command.CommandText.Replace(@"
order by AH.AuditID asc
OFFSET (@PageSize * (@PageNumber - 1)) ROWS
FETCH NEXT @PageSize ROWS ONLY", string.Empty);
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
    }
}
