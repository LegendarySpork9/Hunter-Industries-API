﻿// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services
{
    public class AuditHistoryService
    {
        private readonly LoggerService Logger;

        public AuditHistoryService(LoggerService _logger)
        {
            Logger = _logger;
        }

        public (bool, int) LogRequest(string ipAddress, int endpointId, int methodId, int statusId, string[]? parameters)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest called with the parameters {Logger.FormatParameters(new string[] { ipAddress, endpointId.ToString(), methodId.ToString(), statusId.ToString(), Logger.FormatParameters(parameters) })}.");

            bool logged = false;
            int auditId = 0;

            DatabaseConverter _databaseConverter = new();

            string? formattedParameters = _databaseConverter.FormatParameters(parameters);

            SqlConnection connection;
            SqlCommand command;

            string sqlQuery = @"insert into AuditHistory (IPAddress, EndpointID, MethodID, StatusID, DateOccured, [Parameters])
output inserted.AuditID
values (@IPAddress, @EndpointID, @MethodID, @StatusID, GetDate(), @Parameters)";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointId));
                command.Parameters.Add(new SqlParameter("@MethodID", methodId));
                command.Parameters.Add(new SqlParameter("@StatusID", statusId));
                command.Parameters.Add(new SqlParameter("@Parameters", formattedParameters != null ? formattedParameters : DBNull.Value));
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

        public void LogLoginAttempt(int auditId, bool isSuccessful, string? username = null, string? password = null, string? phrase = null)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogLoginAttempt called with the parameters {Logger.FormatParameters(new string[] { auditId.ToString(), isSuccessful.ToString(), username, password, phrase })}.");

            SqlConnection connection;
            SqlCommand command;

            string sqlQuery = @"insert into LoginAttempt (UserID, PhraseID, AuditID, DateOccured, IsSuccessful)
values ((select UserID from APIUser with (nolock) where Username = @Username and Password = @Password), (select PhraseID from Authorisation with (nolock) where Phrase = @Phrase), @AuditID, GetDate(), @IsSuccessful)";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Username", username != null ? username : DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Password", password != null ? password : DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Phrase", phrase != null ? phrase : DBNull.Value));
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

        public (List<AuditHistoryRecord>, int) GetAuditHistory(string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory called with the parameters {Logger.FormatParameters(new string[] { ipAddress, endpoint, fromDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            AuditHistoryConverter _auditHistoryConverter = new();
            List<AuditHistoryRecord> auditHistories = new();

            int totalRecords = 0;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"\SQL\GetAuditHistory.SQL"));

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

                totalRecords = GetTotalAuditHistory(command);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory returned {auditHistories.Count} records | {totalRecords}");
            return (auditHistories, totalRecords);
        }

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
                command.CommandText = "select count(*) from AuditHistory AH with (nolock)";
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
