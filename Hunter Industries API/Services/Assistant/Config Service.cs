using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects.Assistant;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

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
        public (List<AssistantConfiguration>, int, string) GetAssistantConfig(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            List<AssistantConfiguration> assistantConfigurations = new List<AssistantConfiguration>();

            int totalConfigs = 0;
            string mostRecentVersion = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

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
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);

                if (sqlQuery.Contains("@AssistantName"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                }

                if (sqlQuery.Contains("@AssistantID"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
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

                dataReader.Close();
                connection.Close();

                totalConfigs = GetTotalConfigs(command);
                mostRecentVersion = GetMostRecentVersion();
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
        private int GetTotalConfigs(SqlCommand command)
        {
            int totalRecords = 0;

            SqlConnection connection;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command.Connection = connection;
                command.CommandText = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\GetTotalAssistantConfig.sql");
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    totalRecords = dataReader.GetInt32(0);
                }

                dataReader.Close();
                connection.Close();
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
        public string GetMostRecentVersion()
        {
            string version = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\GetMostRecentAssistantVersion.sql");

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    version = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();
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
        public bool AssistantExists(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            bool exists = false;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\AssistantExists.sql");

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    string name = dataReader.GetString(0);
                    string idNumber = dataReader.GetString(1);

                    if ((name == assistantName && idNumber == assistantId) || idNumber == assistantId)
                    {
                        exists = true;
                    }
                }

                dataReader.Close();
                connection.Close();
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
        public bool AssistantConfigCreated(string assistantName, string assistantId, string assignedUser, string hostName)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, assignedUser, hostName })}.");

            bool created = true;

            SqlConnection connection;
            SqlCommand command;

            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateLocation.sql");
            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Hostname", hostName));
                command.Parameters.Add(new SqlParameter("@IPAddress", "PlaceHolder"));
                var locationId = command.ExecuteScalar();

                if (locationId == null)
                {
                    connection.Close();
                    created = false;
                }

                if (created)
                {
                    sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateUser.sql");
                    rowsAffected = 0;

                    command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.Add(new SqlParameter("@Name", assignedUser));
                    var userId = command.ExecuteScalar();

                    if (userId == null)
                    {
                        connection.Close();
                        created = false;
                    }

                    if (created)
                    {
                        sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Configuration\CreateAssistantConfiguration.sql");
                        rowsAffected = 0;

                        command = new SqlCommand(sqlQuery, connection);
                        command.Parameters.Add(new SqlParameter("@LocationID", locationId));
                        command.Parameters.Add(new SqlParameter("@UserID", userId));
                        command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                        command.Parameters.Add(new SqlParameter("@IDNumber", assistantId));
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            connection.Close();
                            created = false;
                        }
                    }
                }

                connection.Close();
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