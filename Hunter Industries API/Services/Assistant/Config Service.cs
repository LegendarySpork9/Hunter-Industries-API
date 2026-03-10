using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects.Assistant;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class ConfigService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ConfigService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns all configuration records that match the parameters.
        /// </summary>
        public async Task<(List<AssistantConfiguration>, int, string)> GetAssistantConfig(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            List<AssistantConfiguration> assistantConfigurations = new List<AssistantConfiguration>();

            int totalConfigs = 0;
            string mostRecentVersion = string.Empty;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\GetAssistantConfig.sql");

            if (!string.IsNullOrEmpty(assistantName))
            {
                sqlQuery += "\nand AI.Name = @AssistantName";
            }

            if (!string.IsNullOrEmpty(assistantId))
            {
                sqlQuery += "\nand AI.IDNumber = @AssistantID";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        if (sqlQuery.Contains("@AssistantName"))
                        {
                            command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                        }

                        if (sqlQuery.Contains("@AssistantID"))
                        {
                            command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                        }

                        using (SqlDataReader dataReader = (SqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await dataReader.ReadAsync())
                            {
                                AssistantConfiguration configuration = new AssistantConfiguration()
                                {
                                    AssistantName = dataReader.GetString(0),
                                    IdNumber = dataReader.GetString(1),
                                    AssignedUser = dataReader.GetString(2),
                                    HostName = dataReader.GetString(3),
                                    Deletion = bool.Parse(dataReader.GetString(4)),
                                    Version = dataReader.GetString(5)
                                };

                                assistantConfigurations.Add(configuration);
                            }
                        }

                        connection.Close();

                        totalConfigs = await GetTotalConfigs(command);
                        mostRecentVersion = await GetMostRecentVersion();
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetAssistantConfig.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig returned {_parameterFunction.FormatParameters(new string[] { assistantConfigurations.Count.ToString(), totalConfigs.ToString(), mostRecentVersion })}.");
            return (assistantConfigurations, totalConfigs, mostRecentVersion);
        }

        /// <summary>
        /// Returns the number of configuration records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalConfigs(SqlCommand command)
        {
            int totalRecords = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandText = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\GetTotalAssistantConfig.sql");

                    using (SqlDataReader dataReader = (SqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await dataReader.ReadAsync())
                        {
                            totalRecords = dataReader.GetInt32(0);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetTotalConfigs.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return totalRecords;
        }

        /// <summary>
        /// Returns the most recent version number.
        /// </summary>
        public async Task<string> GetMostRecentVersion()
        {
            string version = string.Empty;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\GetMostRecentAssistantVersion.sql");

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader dataReader = (SqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await dataReader.ReadAsync())
                            {
                                version = dataReader.GetString(0);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetMostRecentVersion.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return version;
        }

        /// <summary>
        /// Returns whether a config already exists with the given details.
        /// </summary>
        public async Task<bool> AssistantExists(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            bool exists = false;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\AssistantExists.sql");

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                        command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));

                        using (SqlDataReader dataReader = (SqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await dataReader.ReadAsync())
                            {
                                string name = dataReader.GetString(0);
                                string idNumber = dataReader.GetString(1);

                                if ((name == assistantName && idNumber == assistantId) || idNumber == assistantId)
                                {
                                    exists = true;
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the assistant configuration.
        /// </summary>
        public async Task<bool> AssistantConfigCreated(string assistantName, string assistantId, string assignedUser, string hostName)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, assignedUser, hostName })}.");

            bool created = true;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateLocation.sql");
            int rowsAffected;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Hostname", hostName));
                        command.Parameters.Add(new SqlParameter("@IPAddress", "PlaceHolder"));
                        var locationId = await command.ExecuteScalarAsync();

                        if (locationId == null)
                        {
                            created = false;
                        }

                        if (created)
                        {
                            sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateUser.sql");
                            rowsAffected = 0;

                            using (SqlCommand commandTwo = new SqlCommand(sqlQuery, connection))
                            {
                                commandTwo.Parameters.Add(new SqlParameter("@Name", assignedUser));
                                var userId = await commandTwo.ExecuteScalarAsync();

                                if (userId == null)
                                {
                                    created = false;
                                }

                                if (created)
                                {
                                    sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateAssistantConfiguration.sql");
                                    rowsAffected = 0;

                                    using (SqlCommand commandThree = new SqlCommand(sqlQuery, connection))
                                    {
                                        commandThree.Parameters.Add(new SqlParameter("@LocationID", locationId));
                                        commandThree.Parameters.Add(new SqlParameter("@UserID", userId));
                                        commandThree.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                                        commandThree.Parameters.Add(new SqlParameter("@IDNumber", assistantId));
                                        rowsAffected = await commandThree.ExecuteNonQueryAsync();

                                        if (rowsAffected != 1)
                                        {
                                            created = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated returned {created}.");
            return created;
        }
    }
}