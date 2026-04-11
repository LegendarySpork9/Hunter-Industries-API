// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class VersionService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public VersionService(ILoggerService _logger,
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
        /// Retrns the version information for the given assistant.
        /// </summary>
        public async Task<VersionResponseModel> GetAssistantVersion(string assistantName, string assistantId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            VersionResponseModel version = new VersionResponseModel();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Version\GetAssistantVersion.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@assistantID", SqlDbType.VarChar) { Value = assistantId }
                };

                (VersionResponseModel result, Exception ex) = await _Database.QuerySingle(sql, reader => new VersionResponseModel()
                {
                    AssistantName = reader.GetString(0),
                    IdNumber = reader.GetString(1),
                    Version = reader.GetString(2)
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run VersionService.GetAssistantVersion.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    version = result;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run VersionService.GetAssistantVersion.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion returned {ParameterFunction.FormatParameters(version)}.");
            return version;
        }

        /// <summary>
        /// Updates the version number of the given assistant.
        /// </summary>
        public async Task<bool> AssistantVersionUpdated(string assistantName, string assistantId, string version)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId, version })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Version\AssistantVersionUpdated.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@version", SqlDbType.VarChar) { Value = version },
                    new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@idNumber", SqlDbType.VarChar) { Value = assistantId }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run VersionService.AssistantVersionUpdated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (rowsAffected != 1)
                {
                    updated = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run VersionService.AssistantVersionUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated returned {updated}.");
            return updated;
        }
    }
}
