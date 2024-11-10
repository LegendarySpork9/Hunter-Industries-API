using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using System;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class ChangeService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ChangeService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Creates a record in the Change table.
        /// </summary>
        public bool LogChange(int endpointId, int auditId, string field, string oldValue, string newValue)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange called with the parameters {_parameterFunction.FormatParameters(new string[] { endpointId.ToString(), auditId.ToString(), field, oldValue, newValue })}.");

            bool successful = false;

            SqlConnection connection;
            SqlCommand command;

            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\LogChange.sql"), connection);
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
                string message = "An error occured when trying to run ChangeService.LogChange.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange returned {successful}.");
            return successful;
        }
    }
}