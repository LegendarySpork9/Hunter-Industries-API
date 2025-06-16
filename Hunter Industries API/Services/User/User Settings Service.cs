using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests.Bodies.User;
using HunterIndustriesAPI.Objects.User;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services.User
{
    /// <summary>
    /// </summary>
    public class UserSettingsService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public UserSettingsService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns all user settings that match the parameters.
        /// </summary>
        public List<UserSettingRecord> GetUserSettings(int id, string application)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSettings called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), application })}.");

            List<UserSettingRecord> userSettings = new List<UserSettingRecord>();
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\GetUserSettings.sql");
            string currentApplication = string.Empty;
            UserSettingRecord tempRecord = new UserSettingRecord();

            if (id != 0)
            {
                sqlQuery += "\nand UserID = @Id";
            }

            if (!string.IsNullOrEmpty(application))
            {
                sqlQuery += "\nand [Application].[Name] = @Application";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        if (sqlQuery.Contains("@Id"))
                        {
                            command.Parameters.Add(new SqlParameter("@Id", id));
                        }

                        if (sqlQuery.Contains("@Application"))
                        {
                            command.Parameters.Add(new SqlParameter("@Application", application));
                        }

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                if (string.IsNullOrEmpty(currentApplication) || currentApplication != dataReader.GetString(0))
                                {
                                    if (string.IsNullOrEmpty(currentApplication))
                                    {
                                        tempRecord.Application = dataReader.GetString(0);
                                    }

                                    else
                                    {
                                        userSettings.Add(tempRecord);

                                        tempRecord.Application = dataReader.GetString(0);
                                        tempRecord.Settings.Clear();
                                    }

                                    currentApplication = dataReader.GetString(0);
                                }

                                tempRecord.Settings.Add(new SettingRecord()
                                {
                                    Id = dataReader.GetInt32(1),
                                    Name = dataReader.GetString(2),
                                    Value = dataReader.GetString(3)
                                });
                            }

                            if (!string.IsNullOrEmpty(currentApplication))
                            {
                                userSettings.Add(tempRecord);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.GetUserSettings.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSettings returned {userSettings.Count} records.");
            return userSettings;
        }

        /// <summary>
        /// Returns the setting that matches the id.
        /// </summary>
        public SettingRecord GetUserSetting(int id)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSetting called with the parameters \"{id}\".");

            SettingRecord setting = new SettingRecord();
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\GetUserSetting.sql");

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Id", id));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                setting.Id = dataReader.GetInt32(0);
                                setting.Name = dataReader.GetString(1);
                                setting.Value = dataReader.GetString(2);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.GetUserSetting.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.GetUserSetting returned record {setting.Id}.");
            return setting;
        }

        /// <summary>
        /// Returns whether a user setting already exists with the given values.
        /// </summary>
        public bool UserSettingExists(string username, string application, string settingName)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists called with the parameters {_parameterFunction.FormatParameters(new string[] { username, application, settingName })}.");

            bool exists = false;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\UserSettingExists.sql");

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", username));
                        command.Parameters.Add(new SqlParameter("@Application", application));
                        command.Parameters.Add(new SqlParameter("@Name", settingName));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                exists = true;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a user setting already exists with the given id.
        /// </summary>
        public bool UserSettingExists(int id)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists called with the parameters \"{id}\".");

            bool exists = false;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\UserSettingExistsById.sql");

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Id", id));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                exists = true;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Adds the user setting.
        /// </summary>
        public bool UserSettingAdded(UserSettingsModel userSetting)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingAdded called with the parameters {_parameterFunction.FormatParameters(null, userSetting)}.");

            bool added = true;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\UserSettingAdded.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", userSetting.Username));
                        command.Parameters.Add(new SqlParameter("@Application", userSetting.Application));
                        command.Parameters.Add(new SqlParameter("@Name", userSetting.SettingName));
                        command.Parameters.Add(new SqlParameter("@Value", userSetting.SettingValue));
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            added = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingAdded.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                added = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingAdded returned {added}.");
            return added;
        }

        /// <summary>
        /// Updates the value of the given setting.
        /// </summary>
        public bool UserSettingUpdated(int id, string value)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingUpdated called with the parameters \"{id}\", \"{value}\".");

            bool updated = true;
            int rowsAffected;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\User Settings\UserSettingUpdated.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Value", value));
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            updated = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserSettingsService.UserSettingUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserSettingsService.UserSettingUpdated returned {updated}.");
            return updated;
        }
    }
}