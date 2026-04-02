// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Objects.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.User
{
    /// <summary>
    /// </summary>
    public class UserSettingsService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public UserSettingsService(ILoggerService _logger,
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
        /// Returns all user settings that match the parameters.
        /// </summary>
        public async Task<List<UserSettingRecord>> GetUserSettings(int id, string application)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSettings called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), application })}.");

            List<UserSettingRecord> userSettings = new List<UserSettingRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\GetUserSettings.sql");
                string currentApplication = string.Empty;
                UserSettingRecord tempRecord = new UserSettingRecord();
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (id != 0)
                {
                    sql += "\nand UserID = @id";
                    parameterList.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                }

                if (!string.IsNullOrEmpty(application))
                {
                    sql += "\nand [Application].[Name] = @application";
                    parameterList.Add(new SqlParameter("@application", SqlDbType.VarChar) { Value = application });
                }

                (List<(string, int, string, string)> results, Exception ex) = await _Database.Query(sql, reader => (reader.GetString(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3)), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.GetUserSettings.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                foreach (var result in results)
                {
                    if (string.IsNullOrEmpty(currentApplication) || currentApplication != result.Item1)
                    {
                        if (string.IsNullOrEmpty(currentApplication))
                        {
                            tempRecord.Application = result.Item1;
                        }

                        else
                        {
                            userSettings.Add(tempRecord);

                            tempRecord.Application = result.Item1;
                            tempRecord.Settings.Clear();
                        }

                        currentApplication = result.Item1;
                    }

                    tempRecord.Settings.Add(new SettingRecord()
                    {
                        Id = result.Item2,
                        Name = result.Item3,
                        Value = result.Item4
                    });
                }

                if (!string.IsNullOrEmpty(currentApplication))
                {
                    userSettings.Add(tempRecord);
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.GetUserSettings.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSettings returned {userSettings.Count} records.");
            return userSettings;
        }

        /// <summary>
        /// Returns the setting that matches the id.
        /// </summary>
        public async Task<SettingRecord> GetUserSetting(int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSetting called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            SettingRecord setting = new SettingRecord();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\GetUserSetting.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@id", SqlDbType.Int) { Value = id }
                };

                (SettingRecord result, Exception ex) = await _Database.QuerySingle(sql, reader => new SettingRecord()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Value = reader.GetString(2)
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.GetUserSetting.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    setting = result;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.GetUserSetting.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSetting returned record {setting.Id}.");
            return setting;
        }

        /// <summary>
        /// Returns whether a user setting already exists with the given values.
        /// </summary>
        public async Task<bool> UserSettingExists(string username, string application, string settingName)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists called with the parameters {ParameterFunction.FormatParameters(new string[] { username, application, settingName })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\UserSettingExists.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = username },
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = application },
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = settingName }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a user setting already exists with the given id.
        /// </summary>
        public async Task<bool> UserSettingExists(int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\UserSettingExistsById.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@id", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Adds the user setting.
        /// </summary>
        public async Task<bool> UserSettingAdded(UserSettingsModel userSetting)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingAdded called with the parameters {ParameterFunction.FormatParameters(null, userSetting)}.");

            bool added = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\UserSettingAdded.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = userSetting.Username },
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = userSetting.Application },
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = userSetting.SettingName },
                    new SqlParameter("@value", SqlDbType.VarChar) { Value = userSetting.SettingValue }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.UserSettingAdded.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    added = false;
                }

                if (rowsAffected != 1)
                {
                    added = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingAdded.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                added = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingAdded returned {added}.");
            return added;
        }

        /// <summary>
        /// Updates the value of the given setting.
        /// </summary>
        public async Task<bool> UserSettingUpdated(int id, string value)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), value })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\User Settings\UserSettingUpdated.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@value", SqlDbType.VarChar) { Value = value },
                    new SqlParameter("@id", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserSettingsService.UserSettingUpdated.";
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
                string message = "An error occured when trying to run UserSettingsService.UserSettingUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingUpdated returned {updated}.");
            return updated;
        }
    }
}
