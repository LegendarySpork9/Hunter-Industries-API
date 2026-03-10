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
        /// Sets the class's global variables.
        /// </summary>
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
        public async Task<(bool, int)> LogRequest(string ipAddress, int endpointId, int methodId, int statusId, string[] parameters = null)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogRequest called with the parameters {_parameterFunction.FormatParameters(new string[] { ipAddress, endpointId.ToString(), methodId.ToString(), statusId.ToString(), _parameterFunction.FormatParameters(parameters) })}.");

            bool logged = false;
            int auditId = 0;
            string formattedParameters = _parameterFunction.FormatParameters(parameters, true);

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Audit History\LogRequest.sql");
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress },
                    new SqlParameter("@EndpointID", SqlDbType.Int) { Value = endpointId },
                    new SqlParameter("@MethodID", SqlDbType.Int) { Value = methodId },
                    new SqlParameter("@StatusID", SqlDbType.Int) { Value = statusId },
                    new SqlParameter("@Parameters", SqlDbType.VarChar) { Value = (object)formattedParameters ?? DBNull.Value }
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
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.LogLoginAttempt called with the parameters {_parameterFunction.FormatParameters(new string[] { auditId.ToString(), isSuccessful.ToString(), username, password, phrase })}.");

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
        public async Task<(List<AuditHistoryRecord>, int)> GetAuditHistory(int auditId, string ipAddress, string endpoint, DateTime fromDate, int pageSize, int pageNumber)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"AuditHistoryService.GetAuditHistory called with the parameters {_parameterFunction.FormatParameters(new string[] { auditId.ToString(), ipAddress, endpoint, fromDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

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

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate.ToString() != "01/01/1900 00:00:00")
                {
                    sql += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
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
                        Endpoint = reader.GetString(2),
                        Method = reader.GetString(3),
                        Status = reader.GetString(4),
                        OccuredAt = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc),
                        Paramaters = Array.Empty<string>()
                    };

                    if (!reader.IsDBNull(6))
                    {
                        auditHistory.Paramaters = _parameterFunction.FormatParameters(reader.GetString(6));
                    }

                    LoginAttemptRecord loginAttempt = null;

                    if (!reader.IsDBNull(7))
                    {
                        loginAttempt = new LoginAttemptRecord()
                        {
                            Id = reader.GetInt32(7),
                            IsSuccessful = reader.GetBoolean(10)
                        };

                        if (!reader.IsDBNull(8))
                        {
                            loginAttempt.Username = reader.GetString(8);
                        }

                        if (!reader.IsDBNull(9))
                        {
                            loginAttempt.Phrase = reader.GetString(9);
                        }
                    }

                    auditHistory.LoginAttempt = loginAttempt;
                    auditHistory.Change = new List<ChangeRecord>();

                    if (!reader.IsDBNull(11))
                    {
                        auditHistory.Change.Add(new ChangeRecord()
                        {
                            Id = reader.GetInt32(11),
                            Endpoint = reader.GetString(12),
                            Field = reader.GetString(13),
                            OldValue = reader.GetString(14),
                            NewValue = reader.GetString(15),
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

                totalRecords = await GetTotalAuditHistory(auditId, ipAddress, endpoint, fromDate);
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
        private async Task<int> GetTotalAuditHistory(int auditId, string ipAddress, string endpoint, DateTime fromDate)
        {
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

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand AH.DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
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

            return totalRecords;
        }
    }
}
