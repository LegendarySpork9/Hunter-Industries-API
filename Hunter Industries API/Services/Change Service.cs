// Copyright © 11/06/2026 Toby Hunter
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
    public class ChangeService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ChangeService(
            ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
        }

        /// <summary>
        /// Creates a record in the Change table.
        /// </summary>
        public async Task<bool> LogChange(
            int auditId,
            string field,
            string oldValue,
            string newValue)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ChangeService.LogChange called with the parameters {ParameterFunction.FormatParameters(new string[] { auditId.ToString(), field, oldValue, newValue })}.");

            bool successful = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Change",
                    "LogChange.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@auditID", SqlDbType.Int) { Value = auditId },
                    new SqlParameter("@field", SqlDbType.VarChar) { Value = field },
                    new SqlParameter("@oldValue", SqlDbType.VarChar) { Value = oldValue },
                    new SqlParameter("@newValue", SqlDbType.VarChar) { Value = newValue }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ChangeService.LogChange.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (rowsAffected == 1)
                {
                    successful = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ChangeService.LogChange.";
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
                $"ChangeService.LogChange returned {successful}.");
            return successful;
        }

        /// <summary>
        /// Returns the change records attached to the audit history record that matches the id.
        /// </summary>
        public async Task<List<ChangeRecord>> GetAuditHistoryChanges(int auditId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ChangeService.GetAuditHistoryChanges called with the parameters \"{auditId}\".");

            List<ChangeRecord> changes = new List<ChangeRecord>();

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@auditId", SqlDbType.Int) { Value = auditId }
                };

                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Change",
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
                    string message = "An error occured when trying to run ChangeService.GetAuditHistoryChanges.";
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
                string message = "An error occured when trying to run ChangeService.GetAuditHistoryChanges.";
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
                    $"ChangeService.GetAuditHistoryChanges returned {changes} record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "ChangeService.GetAuditHistoryChanges returned 0 records");
            }

            return changes;
        }
    }
}
