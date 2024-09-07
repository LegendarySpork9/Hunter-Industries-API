// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class VersionService
    {
        private readonly LoggerService Logger;

        public VersionService(LoggerService _logger)
        {
            Logger = _logger;
        }

        // Gets the version number of the given assistant.
        public VersionResponseModel GetAssistantVersion(string assistantName, string assistantId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId })}.");

            VersionResponseModel version = new();

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            // Obtaines and returns all the rows in the AssistantInformation table.
            string sqlQuery = @"select AI.Name, AI.IDNumber, V.Value from Assistant_Information AI
join [Version] V on AI.VersionID = V.VersionID
where AI.Name = @AssistantName
and AI.IDNumber = @AssistantID";

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
                    version = new()
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
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"The following error occured when trying to run VersionService.GetAssistantVersion.");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.GetAssistantVersion returned {Logger.FormatParameters(version)}.");
            return version;
        }

        // Updates the version number of the given assistant.
        public bool AssistantVersionUpdated(string assistantName, string idNumber, string version)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated called with the parameters {Logger.FormatParameters(new string[] { assistantName, idNumber, version })}.");

            bool updated = true;

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;

            // Updates the VersionID column on the Assistant_Information table.
            string sqlQuery = @"update Assistant_Information set VersionID = (select VersionID from [Version] where Value = @Version)
where Name = @AssistantName
and IDNumber = @IDNumber";
            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Version", version));
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
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"The following error occured when trying to run VersionService.AssistantVersionUpdated.");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"VersionService.AssistantVersionUpdated returned {updated}.");
            return updated;
        }
    }
}
