// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Objects;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
        public AuditHistoryService(
            ILoggerService _logger,
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
        public async Task<(bool, int)> LogRequest(
            string ipAddress,
            int endpointId,
            int endpointVersionId,
            int methodId,
            int statusId,
            string username = null,
            string applicationName = null,
            string[] parameters = null,
            string requestBody = null,
            string responseBody = null)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.LogRequest called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, endpointId.ToString(), endpointVersionId.ToString(), methodId.ToString(), statusId.ToString(), username, applicationName, ParameterFunction.FormatParameters(parameters) })}.");

            bool logged = false;
            int auditId = 0;
            string formattedParameters = ParameterFunction.FormatParameters(
                parameters,
                true);

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "LogRequest.sql"));
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = ipAddress },
                    new SqlParameter("@endpointID", SqlDbType.Int) { Value = endpointId },
                    new SqlParameter("@endpointVersionId", SqlDbType.Int) { Value = endpointVersionId },
                    new SqlParameter("@methodID", SqlDbType.Int) { Value = methodId },
                    new SqlParameter("@statusID", SqlDbType.Int) { Value = statusId },
                    new SqlParameter("@parameters", SqlDbType.VarChar) { Value = (object)formattedParameters ?? DBNull.Value },
                    new SqlParameter("@requestBody", SqlDbType.VarChar) { Value = (object)requestBody ?? DBNull.Value },
                    new SqlParameter("@responseBody", SqlDbType.VarChar) { Value = (object)responseBody ?? DBNull.Value },
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = (object)username ?? DBNull.Value },
                    new SqlParameter("@applicationName", SqlDbType.VarChar) { Value = (object)applicationName ?? DBNull.Value }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    sqlParameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.LogRequest.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.LogRequest returned {logged} | {auditId}.");
            return (
                logged,
                auditId);
        }

        /// <summary>
        /// Logs any authorisation calls made to the database.
        /// </summary>
        public async Task LogLoginAttempt(
            int auditId,
            bool isSuccessful,
            string username = null,
            string password = null,
            string phrase = null)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.LogLoginAttempt called with the parameters {ParameterFunction.FormatParameters(new string[] { auditId.ToString(), isSuccessful.ToString(), username, password, phrase })}.");

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "LogLoginAttempt.sql"));
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = (object)username ?? DBNull.Value },
                    new SqlParameter("@password", SqlDbType.VarChar) { Value = (object)password ?? DBNull.Value },
                    new SqlParameter("@phrase", SqlDbType.VarChar) { Value = (object)phrase ?? DBNull.Value },
                    new SqlParameter("@auditID", SqlDbType.Int) { Value = auditId },
                    new SqlParameter("@isSuccessful", SqlDbType.Bit) { Value = isSuccessful }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    sqlParameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.LogLoginAttempt.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.LogLoginAttempt.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }
        }

        /// <summary>
        /// Returns all audit history records that match the parameters.
        /// </summary>
        public async Task<(List<AuditHistoryRecord>, int)> GetAuditHistory(
            string ipAddress,
            string endpoint,
            string username,
            string application,
            DateTime fromDate,
            DateTime toDate,
            int pageSize,
            int pageNumber)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetAuditHistory called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, endpoint, fromDate.ToString(), toDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<AuditHistoryRecord> auditHistories = new List<AuditHistoryRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "GetAuditHistory.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                };
                
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand AH.IPAddress = @ipAddress";
                    parameterList.Add(new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sql += "\nand E.Value = @endpoint";
                    parameterList.Add(new SqlParameter("@endpoint", SqlDbType.VarChar) { Value = endpoint });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand AU.Username = @username";
                    parameterList.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                }

                if (!string.IsNullOrEmpty(application))
                {
                    sql += "\nand App.[Name] = @application";
                    parameterList.Add(new SqlParameter("@application", SqlDbType.VarChar) { Value = application });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured >= cast(@fromDate as datetime)";
                    parameterList.Add(new SqlParameter("@fromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                if (!string.IsNullOrEmpty(toDate.ToString()) && toDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured < cast(@toDate as datetime)";
                    parameterList.Add(new SqlParameter("@toDate", SqlDbType.DateTime) { Value = toDate });
                }

                sql += @"
order by AH.AuditID desc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";

                (List<AuditHistoryRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader =>
                    {
                        AuditHistoryRecord auditHistory = new AuditHistoryRecord()
                        {
                            Id = reader.GetInt32(0),
                            IPAddress = reader.GetString(1),
                            Endpoint = reader.GetString(4),
                            EndpointVersion = reader.GetString(5),
                            Method = reader.GetString(6),
                            Status = reader.GetString(7),
                            OccuredAt = DateTime.SpecifyKind(
                                reader.GetDateTime(8),
                                DateTimeKind.Utc)
                        };

                        if (!reader.IsDBNull(2))
                        {
                            auditHistory.Username = reader.GetString(2);
                        }

                        if (!reader.IsDBNull(3))
                        {
                            auditHistory.Application = reader.GetString(3);
                        }

                        return auditHistory;
                    },
                    parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (results.Count > 0)
                {
                    auditHistories.AddRange(results);
                }

                totalRecords = await GetTotalAuditHistory(
                    ipAddress,
                    endpoint,
                    username,
                    application,
                    fromDate,
                    toDate);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistory.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetAuditHistory returned {auditHistories.Count} records | {totalRecords} total records.");
            return (
                auditHistories,
                totalRecords);
        }

        /// <summary>
        /// Returns the number of audit history records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalAuditHistory(
            string ipAddress,
            string endpoint,
            string username,
            string application,
            DateTime fromDate,
            DateTime toDate)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetTotalAuditHistory called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, endpoint, username, application, fromDate.ToString(), toDate.ToString() })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "GetTotalAuditHistory.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand AH.IPAddress = @ipAddress";
                    parameterList.Add(new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(endpoint))
                {
                    sql += "\nand E.Value = @endpoint";
                    parameterList.Add(new SqlParameter("@endpoint", SqlDbType.VarChar) { Value = endpoint });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand AU.Username = @username";
                    parameterList.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                }

                if (!string.IsNullOrEmpty(application))
                {
                    sql += "\nand App.[Name] = @application";
                    parameterList.Add(new SqlParameter("@application", SqlDbType.VarChar) { Value = application });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured >= cast(@fromDate as datetime)";
                    parameterList.Add(new SqlParameter("@fromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                if (!string.IsNullOrEmpty(toDate.ToString()) && toDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured < cast(@toDate as datetime)";
                    parameterList.Add(new SqlParameter("@toDate", SqlDbType.DateTime) { Value = toDate });
                }

                (int result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader => reader.GetInt32(0),
                    parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetTotalAuditHistory.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetTotalAuditHistory.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetTotalAuditHistory returned {totalRecords}.");
            return totalRecords;
        }

        /// <summary>
        /// Returns the audit history record that matches the id.
        /// </summary>
        public async Task<AuditHistoryRecord> GetAuditHistoryId(int auditId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetAuditHistoryId called with the parameters \"{auditId}\".");

            AuditHistoryRecord auditHistory = null;

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@auditId", SqlDbType.Int) { Value = auditId }
                };

                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "GetAuditHistoryId.sql"));

                (AuditHistoryRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader =>
                    {
                        AuditHistoryRecord audit = new AuditHistoryRecord()
                        {
                            Id = reader.GetInt32(0),
                            IPAddress = reader.GetString(1),
                            Endpoint = reader.GetString(4),
                            EndpointVersion = reader.GetString(5),
                            Method = reader.GetString(6),
                            Status = reader.GetString(7),
                            OccuredAt = DateTime.SpecifyKind(
                                reader.GetDateTime(8),
                                DateTimeKind.Utc),
                            Paramaters = Array.Empty<string>()
                        };

                        if (!reader.IsDBNull(9))
                        {
                            audit.Paramaters = ParameterFunction.FormatParameters(reader.GetString(9));
                        }

                        if (!reader.IsDBNull(10))
                        {
                            audit.RequestBody = reader.GetString(10);
                        }

                        if (!reader.IsDBNull(11))
                        {
                            audit.ResponseBody = reader.GetString(11);
                        }

                        if (!reader.IsDBNull(2))
                        {
                            audit.Username = reader.GetString(2);
                        }

                        if (!reader.IsDBNull(3))
                        {
                            audit.Application = reader.GetString(3);
                        }

                        return audit;
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryId.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                auditHistory = result;
                auditHistory.LoginAttempt = await GetAuditHistoryLoginAttempt(auditId);
                auditHistory.Change = await GetAuditHistoryChanges(auditId);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryId.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (auditHistory != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryId returned 1 record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryId returned 0 records");
            }

            return auditHistory;
        }

        /// <summary>
        /// Returns the login attempt record attached to the audit history record that matches the id.
        /// </summary>
        private async Task<LoginAttemptRecord> GetAuditHistoryLoginAttempt(int auditId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetAuditHistoryLoginAttempt called with the parameters \"{auditId}\".");

            LoginAttemptRecord loginAttempt = null;

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@auditId", SqlDbType.Int) { Value = auditId }
                };

                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "GetAuditHistoryLoginAttempt.sql"));

                (LoginAttemptRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader =>
                    {
                        LoginAttemptRecord attempt = new LoginAttemptRecord()
                        {
                            Id = reader.GetInt32(0),
                            IsSuccessful = reader.GetBoolean(3)
                        };

                        if (!reader.IsDBNull(1))
                        {
                            attempt.Username = reader.GetString(1);
                        }

                        if (!reader.IsDBNull(2))
                        {
                            attempt.Phrase = reader.GetString(2);
                        }

                        return attempt;
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryLoginAttempt.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                loginAttempt = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryLoginAttempt.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (loginAttempt != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryLoginAttempt returned 1 record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryLoginAttempt returned 0 records");
            }

            return loginAttempt;
        }

        /// <summary>
        /// Returns the change records attached to the audit history record that matches the id.
        /// </summary>
        private async Task<List<ChangeRecord>> GetAuditHistoryChanges(int auditId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"AuditHistoryService.GetAuditHistoryChanges called with the parameters \"{auditId}\".");

            List<ChangeRecord> changes = new List<ChangeRecord>();

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@auditId", SqlDbType.Int) { Value = auditId }
                };

                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Audit History",
                    "GetAuditHistoryChanges.sql"));

                (List<ChangeRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => new ChangeRecord()
                    {
                        Id = reader.GetInt32(0),
                        Field = reader.GetString(1),
                        OldValue = reader.GetString(2),
                        NewValue = reader.GetString(3)
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryChanges.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                changes = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run AuditHistoryService.GetAuditHistoryChanges.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (changes != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryChanges returned 1 record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    $"AuditHistoryService.GetAuditHistoryChanges returned 0 records");
            }

            return changes;
        }
    }
}
