using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class DeletionService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public DeletionService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns the deletion information about the given assistant.
        /// </summary>
        public DeletionResponseModel GetAssistantDeletion(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            DeletionResponseModel deletion = new DeletionResponseModel();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Deletion\GetAssistantDeletion.sql"), connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    deletion = new DeletionResponseModel()
                    {
                        AssistantName = dataReader.GetString(0),
                        IdNumber = dataReader.GetString(1),
                        Deletion = bool.Parse(dataReader.GetString(2))
                    };
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run DeletionService.GetAssistantDeletion.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion returned {_parameterFunction.FormatParameters(deletion)}.");
            return deletion;
        }

        /// <summary>
        /// Updates the deletion status of the given assistant.
        /// </summary>
        public bool AssistantDeletionUpdated(string assistantName, string assistantId, bool deletion)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.AssistantDeletionUpdated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, deletion.ToString() })}.");

            bool updated = true;

            SqlConnection connection;
            SqlCommand command;

            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Deletion\AssistantDeletionUpdated.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Deletion", deletion));
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
                string message = "An error occured when trying to run DeletionService.AssistantDeletionUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.AssistantDeletionUpdated returned {updated}.");
            return updated;
        }
    }
}