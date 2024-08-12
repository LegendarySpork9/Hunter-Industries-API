// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services
{
    public class AuditHistoryService
    {
        // Logs the call to the AuditHistory table.
        public (bool, int) LogRequest(string ipAddress, int endpointId, int methodId, int statusId, string[]? parameters)
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
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointId));
                command.Parameters.Add(new SqlParameter("@MethodID", methodId));
                command.Parameters.Add(new SqlParameter("@StatusID", statusId));
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
        public (List<AuditHistoryRecord>, int) GetAuditHistory(string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
        {
            try
            {
                AuditHistoryConverter _auditHistoryConverter = new();
                List<AuditHistoryRecord> auditHistories = new();

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
                    AuditHistoryRecord auditHistory = new()
                    {
                        Id = dataReader.GetInt32(0),
                        IPAddress = dataReader.GetString(1),
                        Endpoint = dataReader.GetString(2),
                        Method = dataReader.GetString(3),
                        Status = dataReader.GetString(4),
                        OccuredAt = dataReader.GetDateTime(5)
                    };

                    if (!dataReader.IsDBNull(6))
                    {
                        auditHistory.Paramaters = _auditHistoryConverter.FormatParameters(dataReader.GetString(6));
                    }

                    auditHistories.Add(auditHistory);
                }

                dataReader.Close();
                connection.Close();

                return (auditHistories, GetTotalAuditHistory(command));
            }

            catch (Exception ex)
            {
                return (new List<AuditHistoryRecord>(), 0);
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
