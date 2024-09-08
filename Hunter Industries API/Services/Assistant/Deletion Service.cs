// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class DeletionService
    {
        private readonly LoggerService Logger;

        public DeletionService(LoggerService _logger)
        {
            Logger = _logger;
        }

        // Gets the deletion status of the given assistant.
        public DeletionResponseModel GetAssistantDeletion(string assistantName, string assistantId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId })}.");

            DeletionResponseModel deletion = new();

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            // Obtaines and returns all the rows in the AssistantInformation table.
            string sqlQuery = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"\SQL\GetAssistantDeletion.SQL"));

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
                    deletion = new()
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

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.GetAssistantDeletion returned {Logger.FormatParameters(deletion)}.");
            return deletion;
        }

        // Updates the deletion status of the given assistant.
        public bool AssistantDeletionUpdated(string assistantName, string idNumber, bool deletion)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"DeletionService.AssistantDeletionUpdated called with the parameters {Logger.FormatParameters(new string[] { assistantName, idNumber, deletion.ToString() })}.");

            bool updated = true;

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;

            // Updates the deletionStatusID column on the AssistantInformation table.
            string sqlQuery = @"update AssistantInformation set DeletionStatusID = (select StatusID from [Deletion] with (nolock) where Value = @Deletion)
where Name = @AssistantName
and IDNumber = @IDNumber";
            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Deletion", deletion));
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", idNumber));
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
