// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class DeletionService
    {
        // Gets the deletion status of the given assistant.
        public DeletionResponseModel GetAssistantDeletion(string assistantName, string assistantID)
        {
            try
            {
                DeletionResponseModel deletion = new();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the rows in the AssistantInformation table.
                string sqlQuery = @"select AI.Name, AI.IDNumber, D.Value from Assistant_Information AI
join Deletion D on AI.DeletionStatusID = D.StatusID
where AI.Name = @AssistantName
and AI.IDNumber = @AssistantID";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantID));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    deletion = new()
                    {
                        AssistantName = dataReader.GetString(0),
                        AssistantId = dataReader.GetString(1),
                        Deletion = bool.Parse(dataReader.GetString(2))
                    };
                }

                dataReader.Close();
                connection.Close();

                return deletion;
            }

            catch (Exception ex)
            {
                return  new DeletionResponseModel();
            }
        }

        // Updates the deletion status of the given assistant.
        public bool AssistantDeletionUpdated(string assistantName, string idNumber, bool deletion)
        {
            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Updates the deletionStatusID column on the AssistantInformation table.
                string sqlQuery = @"update Assistant_Information set DeletionStatusID = (select StatusID from [Deletion] where Value = @Deletion)
where Name = @AssistantName
and IDNumber = @IDNumber";
                int rowsAffected;

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
                    return false;
                }

                connection.Close();
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
