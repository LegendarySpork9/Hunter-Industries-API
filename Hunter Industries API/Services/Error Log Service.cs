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
    public class ErrorLogService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;
        private readonly IClock _Clock;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ErrorLogService(ILoggerService _logger,
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
        /// Returns all error log records that match the parameters.
        /// </summary>
        public async Task<(List<ErrorLogRecord>, int)> GetErrorLog(int errorId, string ipAddress, string summary, DateTime fromDate, int pageSize, int pageNumber)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ErrorLogService.GetErrorLog called with the parameters {ParameterFunction.FormatParameters(new string[] { errorId.ToString(), ipAddress, summary, fromDate.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<ErrorLogRecord> errorLogs = new List<ErrorLogRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Error Log\GetErrorLog.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                };

                if (errorId != 0)
                {
                    sql += "\nand errorId = @ErrorId";
                    parameterList.Add(new SqlParameter("@ErrorId", SqlDbType.Int) { Value = errorId });
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand IPAddress = @IPAddress";
                    parameterList.Add(new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(summary))
                {
                    sql += "\nand Summary = @Summary";
                    parameterList.Add(new SqlParameter("@Summary", SqlDbType.VarChar) { Value = summary });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                sql += @"
order by errorId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";

                (List<ErrorLogRecord> results, Exception ex) = await _Database.Query(sql, reader =>
                {
                    ErrorLogRecord errorLog = new ErrorLogRecord()
                    {
                        Id = reader.GetInt32(0),
                        DateOccured = DateTime.SpecifyKind(reader.GetDateTime(1), DateTimeKind.Utc),
                        IPAddress = reader.GetString(2),
                        Summary = reader.GetString(3),
                        Message = reader.GetString(4)
                    };

                    return errorLog;
                }, parameterList.ToArray());

                errorLogs = results;

                if (ex != null)
                {
                    string message = "An error occured when trying to run ErrorLogService.GetErrorLog.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = await GetTotalErrorLog(ipAddress, summary, fromDate);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ErrorLogService.GetErrorLog.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ErrorLogService.GetErrorLog returned {errorLogs.Count} records | {totalRecords} total records.");
            return (errorLogs, totalRecords);
        }

        /// <summary>
        /// Returns the number of error log records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalErrorLog(string ipAddress, string summary, DateTime fromDate)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ErrorLogService.GetTotalErrorLog called with the parameters {ParameterFunction.FormatParameters(new string[] { ipAddress, summary, fromDate.ToString() })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Error Log\GetTotalErrorLog.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    sql += "\nand IPAddress = @IPAddress";
                    parameterList.Add(new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                if (!string.IsNullOrEmpty(summary))
                {
                    sql += "\nand Summary = @Summary";
                    parameterList.Add(new SqlParameter("@Summary", SqlDbType.VarChar) { Value = summary });
                }

                if (!string.IsNullOrEmpty(fromDate.ToString()) && fromDate != _Clock.DefaultDate)
                {
                    sql += "\nand DateOccured >= cast(@FromDate as datetime)";
                    parameterList.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate });
                }

                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ErrorLogService.GetTotalErrorLog.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ErrorLogService.GetTotalErrorLog.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ErrorLogService.GetTotalErrorLog returned {totalRecords}.");
            return totalRecords;
        }
    }
}