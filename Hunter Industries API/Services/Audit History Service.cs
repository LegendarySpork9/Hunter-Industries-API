// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class AuditHistoryService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public AuditHistoryService(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database,
            IClock clock)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
            _Clock = clock;
        }

        /// <summary>
        /// Logs the call made to the database.
        /// </summary>
        public async Task<(bool, int)> LogRequest(string ipAddress, int endpointId, int endpointVersionId, int methodId, int statusId, string username = null, string applicationName = null, string[] parameters = null)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, endpointId.ToString(), endpointVersionId.ToString(), methodId.ToString(), statusId.ToString(), username, applicationName, ParameterFunction.FormatParameters(parameters) })}.");

            bool logged = false;
            int auditId = 0;
            string formattedParameters = ParameterFunction.FormatParameters(parameters, true);

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Audit History\LogRequest.sql");
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress },
                    new SqlParameter("@EndpointID", SqlDbType.Int) { Value = endpointId },
                    new SqlParameter("@EndpointVersionId", SqlDbType.Int) { Value = endpointVersionId },
                    new SqlParameter("@MethodID", SqlDbType.Int) { Value = methodId },
                    new SqlParameter("@StatusID", SqlDbType.Int) { Value = statusId },
                    new SqlParameter("@Parameters", SqlDbType.VarChar) { Value = (object)formattedParameters ?? DBNull.Value },
                    new SqlParameter("@Username", SqlDbType.VarChar) { Value = (object)username ?? DBNull.Value },
                    new SqlParameter("@ApplicationName", SqlDbType.VarChar) { Value = (object)applicationName ?? DBNull.Value }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, sqlParameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.LogRequest.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    auditId = int.Parse(result.ToString());
                    logged = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.LogRequest.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest returned {logged} | {auditId}.");
            return (logged, auditId);
        }

        /// <summary>
        /// Logs any authorisation calls made to the database.
        /// </summary>
        public async Task LogLoginAttempt(int auditId, bool isSuccessful, string username = null, string password = null, string phrase = null)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogLoginAttempt called with the parameters {ParameterFunction.FormatParameters(new string[] { auditId.ToString(), isSuccessful.ToString(), username, password, phrase })}.");

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Audit History\LogLoginAttempt.sql");
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@Username", SqlDbType.VarChar) { Value = (object)username ?? DBNull.Value },
                    new SqlParameter("@Password", SqlDbType.VarChar) { Value = (object)password ?? DBNull.Value },
                    new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = (object)phrase ?? DBNull.Value },
                    new SqlParameter("@AuditID", SqlDbType.Int) { Value = auditId },
                    new SqlParameter("@IsSuccessful", SqlDbType.Bit) { Value = isSuccessful }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, sqlParameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.LogLoginAttempt.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.LogLoginAttempt.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }
        }

        /// <summary>
        /// Returns all audit history records that match the parameters.
        /// </summary>
        public async Task<(List<AuditHistoryRecord>, int)> GetAuditHistory(int auditId, string ipAddress, string endpoint, string username, string application, DateTime fromDate, DateTime toDate, int pageSize, int pageNumber)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory called with the parameters {ParameterFunction.FormatParameters(new string[] { auditId.ToString(), ipAddress, endpoint, fromDate.ToString(), toDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<AuditHistoryRecord> auditHistories = new List<AuditHistoryRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Audit History\GetAuditHistory.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                };

                if (auditId != 0)
                {
                    sql += "\nand AH.AuditID = @AuditId";
                    parameterList.Add(new SqlParameter("@AuditId", SqlDbType.Int) { Value = auditId });
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand AH.IPAddress = @IPAddress";
                    parameterList.Add(new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sql += "\nand E.Value = @Endpoint";
                    parameterList.Add(new SqlParameter("@Endpoint", SqlDbType.VarChar) { Value = endpoint });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand AU2.Username = @Username";
                    parameterList.Add(new SqlParameter("@Username", SqlDbType.VarChar) { Value = username });
                }

                if (!string.IsNullOrEmpty(application))
                {
                    sql += "\nand App.[Name] = @Application";
                    parameterList.Add(new SqlParameter("@Application", SqlDbType.VarChar) { Value = application });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                if (!string.IsNullOrEmpty(toDate.ToString()) && toDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured < cast(@ToDate as datetime)";
                    parameterList.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate });
                }

                sql += @"
order by AH.AuditID asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";

                (List<AuditHistoryRecord> results, Exception ex) = await _Database.Query(sql, reader =>
                {
                    AuditHistoryRecord auditHistory = new AuditHistoryRecord()
                    {
                        Id = reader.GetInt32(0),
                        IPAddress = reader.GetString(1),
                        Endpoint = reader.GetString(4),
                        EndpointVersion = reader.GetString(5),
                        Method = reader.GetString(6),
                        Status = reader.GetString(7),
                        OccuredAt = DateTime.SpecifyKind(reader.GetDateTime(8), DateTimeKind.Utc),
                        Paramaters = Array.Empty<string>()
                    };

                    if (!reader.IsDBNull(9))
                    {
                        auditHistory.Paramaters = ParameterFunction.FormatParameters(reader.GetString(9));
                    }

                    if (!reader.IsDBNull(2))
                    {
                        auditHistory.Username = reader.GetString(2);
                    }

                    if (!reader.IsDBNull(3))
                    {
                        auditHistory.Application = reader.GetString(3);
                    }

                    LoginAttemptRecord loginAttempt = null;

                    if (!reader.IsDBNull(10))
                    {
                        loginAttempt = new LoginAttemptRecord()
                        {
                            Id = reader.GetInt32(10),
                            IsSuccessful = reader.GetBoolean(13)
                        };

                        if (!reader.IsDBNull(11))
                        {
                            loginAttempt.Username = reader.GetString(11);
                        }

                        if (!reader.IsDBNull(12))
                        {
                            loginAttempt.Phrase = reader.GetString(12);
                        }
                    }

                    auditHistory.LoginAttempt = loginAttempt;
                    auditHistory.Change = new List<ChangeRecord>();

                    if (!reader.IsDBNull(14))
                    {
                        auditHistory.Change.Add(new ChangeRecord()
                        {
                            Id = reader.GetInt32(14),
                            Field = reader.GetString(15),
                            OldValue = reader.GetString(16),
                            NewValue = reader.GetString(17),
                        });
                    }

                    return auditHistory;
                }, parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                AuditHistoryRecord current = null;
                List<ChangeRecord> changes = new List<ChangeRecord>();

                foreach (AuditHistoryRecord record in results)
                {
                    if (current != null && record.Id == current.Id)
                    {
                        changes.AddRange(record.Change);
                    }

                    else
                    {
                        if (current != null)
                        {
                            current.Change = changes;
                            auditHistories.Add(current);
                            changes = new List<ChangeRecord>();
                        }

                        current = record;
                        changes = new List<ChangeRecord>(record.Change);
                    }
                }

                if (current != null)
                {
                    current.Change = changes;
                    auditHistories.Add(current);
                }

                totalRecords = await GetTotalAuditHistory(ipAddress, endpoint, username, application, fromDate, toDate);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory returned {auditHistories.Count} records | {totalRecords} total records.");
            return (auditHistories, totalRecords);
        }

        /// <summary>
        /// Returns the number of audit history records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalAuditHistory(string ipAddress, string endpoint, string username, string application, DateTime fromDate, DateTime toDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetTotalAuditHistory called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, endpoint, username, application, fromDate.ToString(), toDate.ToString() })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Audit History\GetTotalAuditHistory.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand AH.IPAddress = @IPAddress";
                    parameterList.Add(new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sql += "\nand E.Value = @Endpoint";
                    parameterList.Add(new SqlParameter("@Endpoint", SqlDbType.VarChar) { Value = endpoint });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand AU2.Username = @Username";
                    parameterList.Add(new SqlParameter("@Username", SqlDbType.VarChar) { Value = username });
                }

                if (!string.IsNullOrEmpty(application))
                {
                    sql += "\nand App.[Name] = @Application";
                    parameterList.Add(new SqlParameter("@Application", SqlDbType.VarChar) { Value = application });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                if (!string.IsNullOrEmpty(toDate.ToString()) && toDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured < cast(@ToDate as datetime)";
                    parameterList.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate });
                }

                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetTotalAuditHistory.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetTotalAuditHistory.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetTotalAuditHistory returned {totalRecords}.");
            return totalRecords;
        }
    }
}
