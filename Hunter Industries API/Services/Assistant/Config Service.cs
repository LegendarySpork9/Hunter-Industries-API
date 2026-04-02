// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Objects.Assistant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class ConfigService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ConfigService(ILoggerService _logger,
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
        /// Returns all configuration records that match the parameters.
        /// </summary>
        public async Task<(List<AssistantConfiguration>, int, string)> GetAssistantConfig(string assistantName, string assistantId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            List<AssistantConfiguration> assistantConfigurations = new List<AssistantConfiguration>();
            int totalConfigs = 0;
            string mostRecentVersion = string.Empty;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\GetAssistantConfig.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(assistantName))
                {
                    sql += "\nand AI.Name = @assistantName";
                    parameterList.Add(new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName });
                }

                if (!string.IsNullOrEmpty(assistantId))
                {
                    sql += "\nand AI.IDNumber = @assistantID";
                    parameterList.Add(new SqlParameter("@assistantID", SqlDbType.VarChar) { Value = assistantId });
                }

                (List<AssistantConfiguration> results, Exception ex) = await _Database.Query(sql, reader => new AssistantConfiguration()
                {
                    AssistantName = reader.GetString(0),
                    IdNumber = reader.GetString(1),
                    AssignedUser = reader.GetString(2),
                    HostName = reader.GetString(3),
                    Deletion = bool.Parse(reader.GetString(4)),
                    Version = reader.GetString(5)
                }, parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigService.GetAssistantConfig.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                assistantConfigurations = results;
                totalConfigs = await GetTotalConfigs(assistantName, assistantId);
                mostRecentVersion = await GetMostRecentVersion();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetAssistantConfig.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig returned {ParameterFunction.FormatParameters(new string[] { assistantConfigurations.Count.ToString(), totalConfigs.ToString(), mostRecentVersion })}.");
            return (assistantConfigurations, totalConfigs, mostRecentVersion);
        }

        /// <summary>
        /// Returns the number of configuration records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalConfigs(string assistantName, string assistantId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetTotalConfigs called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\GetTotalAssistantConfig.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(assistantName))
                {
                    parameterList.Add(new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName });
                }

                if (!string.IsNullOrEmpty(assistantId))
                {
                    parameterList.Add(new SqlParameter("@assistantID", SqlDbType.VarChar) { Value = assistantId });
                }

                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigService.GetTotalConfigs.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetTotalConfigs.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetTotalConfigs returned {totalRecords}.");
            return totalRecords;
        }

        /// <summary>
        /// Returns the most recent version number.
        /// </summary>
        public async Task<string> GetMostRecentVersion()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetMostRecentVersion called.");

            string version = string.Empty;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\GetMostRecentAssistantVersion.sql");
                (string result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetString(0));

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigService.GetMostRecentVersion.";
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
                string message = "An error occured when trying to run ConfigService.GetMostRecentVersion.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetMostRecentVersion returned {version}.");
            return version;
        }

        /// <summary>
        /// Returns whether a config already exists with the given details.
        /// </summary>
        public async Task<bool> AssistantExists(string assistantName, string assistantId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\AssistantExists.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@assistantID", SqlDbType.VarChar) { Value = assistantId }
                };

                (List<(string, string)> results, Exception ex) = await _Database.Query(sql, reader => (reader.GetString(0), reader.GetString(1)), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigService.AssistantExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                foreach (var result in results)
                {
                    if ((result.Item1 == assistantName && result.Item2 == assistantId) || result.Item2 == assistantId)
                    {
                        exists = true;
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the assistant configuration.
        /// </summary>
        public async Task<bool> AssistantConfigCreated(string assistantName, string assistantId, string assignedUser, string hostName)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId, assignedUser, hostName })}.");

            bool created = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\CreateLocation.sql");
                SqlParameter[] locationParameters =
                {
                    new SqlParameter("@hostname", SqlDbType.VarChar) { Value = hostName },
                    new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = "PlaceHolder" }
                };

                (object locationId, Exception ex) = await _Database.ExecuteScalar(sql, locationParameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    created = false;
                }

                if (locationId == null)
                {
                    created = false;
                }

                if (created)
                {
                    sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\CreateUser.sql");
                    SqlParameter[] userParameters =
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = assignedUser }
                    };

                    (object userId, Exception ex2) = await _Database.ExecuteScalar(sql, userParameters);

                    if (ex2 != null)
                    {
                        string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex2.ToString(), message);

                        created = false;
                    }

                    if (userId == null)
                    {
                        created = false;
                    }

                    if (created)
                    {
                        sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Configuration\CreateAssistantConfiguration.sql");
                        SqlParameter[] configParameters =
                        {
                            new SqlParameter("@locationID", SqlDbType.Int) { Value = locationId },
                            new SqlParameter("@userID", SqlDbType.Int) { Value = userId },
                            new SqlParameter("@assistantName", SqlDbType.VarChar) { Value = assistantName },
                            new SqlParameter("@idNumber", SqlDbType.VarChar) { Value = assistantId }
                        };

                        (int rowsAffected, Exception ex3) = await _Database.Execute(sql, configParameters);

                        if (ex3 != null)
                        {
                            string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                            _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                            _Logger.LogMessage(StandardValues.LoggerValues.Error, ex3.ToString(), message);

                            created = false;
                        }

                        if (rowsAffected != 1)
                        {
                            created = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated returned {created}.");
            return created;
        }
    }
}
