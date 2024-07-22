// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services
{
    public class ChangeService
    {
        // Logs any changes to the data.
        public bool LogChange(int endpointId, int auditId, string field, string oldValue, string newValue)
        {
            bool Successful = false;

            try
            {
                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;

                // Inserts the record into the Change table.
                string sqlQuery = @"insert into [Change] (EndpointID, AuditID, Field, OldValue, NewValue)
values (@EndpointID, @AuditID, @Field, @OldValue, @NewValue)";

                int rowsAffected;

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@EndpointID", endpointId));
                command.Parameters.Add(new SqlParameter("@AuditID", auditId));
                command.Parameters.Add(new SqlParameter("@Field", field));
                command.Parameters.Add(new SqlParameter("@OldValue", oldValue));
                command.Parameters.Add(new SqlParameter("@NewValue", newValue));
                rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 1)
                {
                    Successful = true;
                }

                connection.Close();
                return Successful;
            }

            catch (Exception ex)
            {
                return Successful;
            }
        }
    }
}
