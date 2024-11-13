using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class AuditHistoryService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public AuditHistoryService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Logs the call made to the database.
        /// </summary>
        public (bool, int) LogRequest(string ipAddress, int endpointId, int methodId, int statusId, string[] parameters = null)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest called with the parameters {_parameterFunction.FormatParameters(new string[] { ipAddress, endpointId.ToString(), methodId.ToString(), statusId.ToString(), _parameterFunction.FormatParameters(parameters) })}.");

            bool logged = false;
            int auditId = 0;

            string formattedParameters = _parameterFunction.FormatParameters(parameters, true);

            SqlConnection connection;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Audit History\LogRequest.sql"), connection);
                command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointId));
                command.Parameters.Add(new SqlParameter("@MethodID", methodId));
                command.Parameters.Add(new SqlParameter("@StatusID", statusId));
                command.Parameters.Add(new SqlParameter("@Parameters", (object)formattedParameters ?? DBNull.Value));
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    auditId = int.Parse(result.ToString());
                    logged = true;
                }

                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.LogRequest.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest returned {logged} | {auditId}.");
            return (logged, auditId);
        }

        /// <summary>
        /// Logs any authorisation calls made to the database.
        /// </summary>
        public void LogLoginAttempt(int auditId, bool isSuccessful, string username = null, string password = null, string phrase = null)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogLoginAttempt called with the parameters {_parameterFunction.FormatParameters(new string[] { auditId.ToString(), isSuccessful.ToString(), username, password, phrase })}.");

            SqlConnection connection;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Audit History\LogLoginAttempt.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Username", (object)username ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Password", (object)password ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Phrase", (object)phrase ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@AuditID", auditId));
                command.Parameters.Add(new SqlParameter("@IsSuccessful", isSuccessful));
                int rowsAffected = command.ExecuteNonQuery();

                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.LogLoginAttempt.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }
        }

        /// <summary>
        /// Returns all audit history records that match the parameters.
        /// </summary>
        public (List<AuditHistoryRecord>, int) GetAuditHistory(int auditId, string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory called with the parameters {_parameterFunction.FormatParameters(new string[] { auditId.ToString(), ipAddress, endpoint, fromDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<AuditHistoryRecord> auditHistories = new List<AuditHistoryRecord>();

            int totalRecords = 0;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Audit History\GetAuditHistory.sql");

            if (auditId != 0)
            {
                sqlQuery += "\nand AH.AuditID = @AuditId";
            }

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

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));

                if (sqlQuery.Contains("@AuditId"))
                {
                    command.Parameters.Add(new SqlParameter("@AuditId", auditId));
                }

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

                AuditHistoryRecord auditHistory = new AuditHistoryRecord();
                List<ChangeRecord> changes = new List<ChangeRecord>();

                while (dataReader.Read())
                {
                    if (dataReader.GetInt32(0) == auditHistory.Id)
                    {
                        ChangeRecord change = new ChangeRecord()
                        {
                            Id = dataReader.GetInt32(11),
                            Endpoint = dataReader.GetString(12),
                            Field = dataReader.GetString(13),
                            OldValue = dataReader.GetString(14),
                            NewValue = dataReader.GetString(15),
                        };

                        changes.Add(change);
                    }

                    else
                    {
                        if (auditHistory.Id != 0)
                        {
                            auditHistory.Change = changes;
                            auditHistories.Add(auditHistory);

                            changes = new List<ChangeRecord>();
                        }

                        auditHistory = new AuditHistoryRecord()
                        {
                            Id = dataReader.GetInt32(0),
                            IPAddress = dataReader.GetString(1),
                            Endpoint = dataReader.GetString(2),
                            Method = dataReader.GetString(3),
                            Status = dataReader.GetString(4),
                            OccuredAt = dataReader.GetDateTime(5),
                            Paramaters = Array.Empty<string>()
                        };

                        if (!dataReader.IsDBNull(6))
                        {
                            auditHistory.Paramaters = _parameterFunction.FormatParameters(dataReader.GetString(6));
                        }

                        LoginAttemptRecord loginAttempt = null;

                        if (!dataReader.IsDBNull(7))
                        {
                            loginAttempt = new LoginAttemptRecord()
                            {
                                Id = dataReader.GetInt32(7),
                                IsSuccessful = dataReader.GetBoolean(10)
                            };

                            if (!dataReader.IsDBNull(8))
                            {
                                loginAttempt.Username = dataReader.GetString(8);
                            }

                            if (!dataReader.IsDBNull(9))
                            {
                                loginAttempt.Phrase = dataReader.GetString(9);
                            }
                        }

                        auditHistory.LoginAttempt = loginAttempt;

                        if (!dataReader.IsDBNull(11))
                        {
                            ChangeRecord change = new ChangeRecord()
                            {
                                Id = dataReader.GetInt32(11),
                                Endpoint = dataReader.GetString(12),
                                Field = dataReader.GetString(13),
                                OldValue = dataReader.GetString(14),
                                NewValue = dataReader.GetString(15),
                            };

                            changes.Add(change);
                        }
                    }
                }

                auditHistory.Change = changes;
                auditHistories.Add(auditHistory);

                dataReader.Close();
                connection.Close();

                totalRecords = GetTotalAuditHistory(command);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory returned {auditHistories.Count} records | {totalRecords} total records.");
            return (auditHistories, totalRecords);
        }

        /// <summary>
        /// Returns the number of audit history records that match the parameters.
        /// </summary>
        private int GetTotalAuditHistory(SqlCommand command)
        {
            int totalRecords = 0;

            SqlConnection connection;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command.Connection = connection;
                command.CommandText = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Audit History\GetTotalAuditHistory.sql");

                if (command.Parameters.Contains("@IPAddress"))
                {
                    command.CommandText += "\nand AH.IPAddress = @IPAddress";
                }

                if (command.Parameters.Contains("@Endpoint"))
                {
                    command.CommandText += "\nand E.Value = @Endpoint";
                }

                if (command.Parameters.Contains("@FromDate"))
                {
                    command.CommandText += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    totalRecords = dataReader.GetInt32(0);
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetTotalAuditHistory.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return totalRecords;
        }
    }
}