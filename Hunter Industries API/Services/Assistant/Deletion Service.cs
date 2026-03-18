// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class DeletionService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public DeletionService(ILoggerService _logger,
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
        /// Returns the deletion information about the given assistant.
        /// </summary>
        public async Task<DeletionResponseModel> GetAssistantDeletion(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            DeletionResponseModel deletion = new DeletionResponseModel();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Deletion\GetAssistantDeletion.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AssistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@AssistantID", SqlDbType.VarChar) { Value = assistantId }
                };

                (DeletionResponseModel result, Exception ex) = await _Database.QuerySingle(sql, reader => new DeletionResponseModel()
                {
                    AssistantName = reader.GetString(0),
                    IdNumber = reader.GetString(1),
                    Deletion = bool.Parse(reader.GetString(2))
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run DeletionService.GetAssistantDeletion.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    deletion = result;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run DeletionService.GetAssistantDeletion.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion returned {_parameterFunction.FormatParameters(deletion)}.");
            return deletion;
        }

        /// <summary>
        /// Updates the deletion status of the given assistant.
        /// </summary>
        public async Task<bool> AssistantDeletionUpdated(string assistantName, string assistantId, bool deletion)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.AssistantDeletionUpdated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, deletion.ToString() })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Deletion\AssistantDeletionUpdated.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Deletion", SqlDbType.Bit) { Value = deletion },
                    new SqlParameter("@AssistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@IDNumber", SqlDbType.VarChar) { Value = assistantId }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run DeletionService.AssistantDeletionUpdated.";
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
                string message = "An error occured when trying to run DeletionService.AssistantDeletionUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.AssistantDeletionUpdated returned {updated}.");
            return updated;
        }
    }
}
