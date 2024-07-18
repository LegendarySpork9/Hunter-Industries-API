// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services
{
    public class AuditHistoryService
    {
        // Logs the call to the AuditHistory table.
        public (bool, int) LogRequest(string ipAddress, int endpointID, int methodID, int statusID, string[]? parameters)
        {
            try
            {
                DatabaseConverter _databaseConverter = new();

                string? formattedParameters = _databaseConverter.FormatParameters(parameters);

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Inserts the record into the AuditHistory table.
                string sqlQuery = @"insert into Audit_History (IPAddress, EndpointID, MethodID, StatusID, DateOccured, [Parameters])
output inserted.AuditID
values (@IPAddress, @EndpointID, @MethodID, @StatusID, GetDate(), @Parameters)";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointID));
                command.Parameters.Add(new SqlParameter("@MethodID", methodID));
                command.Parameters.Add(new SqlParameter("@StatusID", statusID));
                command.Parameters.Add(new SqlParameter("@Parameters", formattedParameters));
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
        public (int[], string[], string[], string[], string[], DateTime[], string[], int) GetAuditHistory(string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
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
                string sqlQuery = @"select AuditID, IPAddress, E.Value, M.Value, SC.Value, DateOccured, [Parameters] from Audit_History AH
join [Endpoint] E on AH.EndpointID = E.EndpointID
join Methods M on AH.MethodID = M.MethodID
join Status_Code SC on AH.StatusID = SC.StatusID
where AH.AuditID is not null";

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sqlQuery += "\nand AH.IPAddress = @IPAddress";
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sqlQuery += "\nand E.Value = @Endpoint";
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate.ToString() != "01/01/1900 00:00:00")
                {
                    sqlQuery += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                }

                sqlQuery += @"
order by AH.AuditID asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";

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

                    if (dataReader.IsDBNull(6))
                    {
                        parameters = parameters.Append(String.Empty).ToArray();
                    }

                    else
                    {
                        parameters = parameters.Append(dataReader.GetString(6)).ToArray();
                    }
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
        private int GetTotalAuditHistory(SqlCommand command)
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
                command.CommandText = command.CommandText.Replace(@"select AuditID, IPAddress, E.Value, M.Value, SC.Value, DateOccured, [Parameters] from Audit_History AH
join [Endpoint] E on AH.EndpointID = E.EndpointID
join Methods M on AH.MethodID = M.MethodID
join Status_Code SC on AH.StatusID = SC.StatusID", @"select count(*) from Audit_History AH
join [Endpoint] E on AH.EndpointID = E.EndpointID").Replace(@"order by AH.AuditID asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only", "");
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
