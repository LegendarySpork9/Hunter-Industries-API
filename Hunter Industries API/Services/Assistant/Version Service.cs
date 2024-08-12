// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class VersionService
    {
        // Gets the version number of the given assistant.
        public VersionResponseModel GetAssistantVersion(string assistantName, string assistantId)
        {
            try
            {
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

                return version;
            }

            catch (Exception ex)
            {
                return new VersionResponseModel();
            }
        }

        // Updates the version number of the given assistant.
        public bool AssistantVersionUpdated(string assistantName, string idNumber, string version)
        {
            try
            {
                bool updated = true;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Updates the VersionID column on the Assistant_Information table.
                string sqlQuery = @"update Assistant_Information set VersionID = (select VersionID from [Version] where Value = @Version)
where Name = @AssistantName
and IDNumber = @IDNumber";
                int rowsAffected;

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
                return updated;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
