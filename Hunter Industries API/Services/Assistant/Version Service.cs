using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Responses.Assistant;
using HunterIndustriesAPI.Models;
using System;
using System.Data.SqlClient;
using System.IO;
using HunterIndustriesAPI.Functions;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class VersionService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public VersionService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Retrns the version information for the given assistant.
        /// </summary>
        public VersionResponseModel GetAssistantVersion(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            VersionResponseModel version = new VersionResponseModel();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Version\GetAssistantVersion.sql"), connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    version = new VersionResponseModel()
                    {
                        AssistantName = dataReader.GetString(0),
                        IdNumber = dataReader.GetString(1),
                        Version = dataReader.GetString(2)
                    };
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run VersionService.GetAssistantVersion.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion returned {_parameterFunction.FormatParameters(version)}.");
            return version;
        }

        /// <summary>
        /// Updates the version number of the given assistant.
        /// </summary>
        public bool AssistantVersionUpdated(string assistantName, string assistantId, string version)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, version })}.");

            bool updated = true;

            SqlConnection connection;
            SqlCommand command;

            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Version\AssistantVersionUpdated.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Version", version));
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", assistantId));
                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    connection.Close();
                    updated = false;
                }

                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run VersionService.AssistantVersionUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated returned {updated}.");
            return updated;
        }
    }
}