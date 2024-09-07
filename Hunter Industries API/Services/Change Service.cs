// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services
{
    public class ChangeService
    {
        private readonly LoggerService Logger;

        public ChangeService(LoggerService _logger)
        {
            Logger = _logger;
        }

        // Logs any changes to the data.
        public bool LogChange(int endpointId, int auditId, string field, string oldValue, string newValue)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange called with the parameters {Logger.FormatParameters(new string[] { endpointId.ToString(), auditId.ToString(), field, oldValue, newValue })}.");

            bool successful = false;

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;

            // Inserts the record into the Change table.
            string sqlQuery = @"insert into [Change] (EndpointID, AuditID, Field, OldValue, NewValue)
values (@EndpointID, @AuditID, @Field, @OldValue, @NewValue)";

            int rowsAffected;

            try
            {
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
                    successful = true;
                }

                connection.Close();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Error, $"The following error occured when trying to run ChangeService.LogChange.");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange returned {successful}.");
            return successful;
        }
    }
}
