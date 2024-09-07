// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class LocationService
    {
        private readonly LoggerService Logger;

        public LocationService(LoggerService _logger)
        {
            Logger = _logger;
        }

        // Gets the host name and ip address of the given assistant.
        public LocationResponseModel GetAssistantLocation(string assistantName, string assistantId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId })}.");

            LocationResponseModel location = new();

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            // Obtaines and returns all the rows in the Assistant_Information table.
            string sqlQuery = @"select AI.Name, AI.IDNumber, L.HostName, L.IPAddress from Assistant_Information AI
join Location L on AI.LocationID = L.LocationID
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
                    location = new()
                    {
                        AssistantName = dataReader.GetString(0),
                        IdNumber = dataReader.GetString(1),
                        HostName = dataReader.GetString(2),
                        IPAddress = dataReader.GetString(3)
                    };
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to run LocationService.GetAssistantLocation.");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation returned {Logger.FormatParameters(location)}.");
            return location;
        }

        // Updates the host name or ip address of the given assistant.
        public bool AssistantLocationUpdated(string assistantName, string assistantId, string? hostName, string? ipAddress)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId, hostName, ipAddress })}.");

            bool updated = true;

            // Creates the variables for the SQL queries.
            SqlConnection connection;
            SqlCommand command;

            // Updates the HostName or the IPAddress on the Location table.
            string sqlQuery = @"update [Location] set HostName = @HostName, IPAddress = @IPAddress
where LocationID = (
	select LocationID from Assistant_Information
	where Name = @AssistantName
	and IDNumber = @IDNumber
)";
            int rowsAffected;

            if (string.IsNullOrEmpty(hostName))
            {
                sqlQuery = sqlQuery.Replace("HostName = @HostName, ", "");
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                sqlQuery = sqlQuery.Replace(", IPAddress = @IPAddress", "");
            }

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@IDNumber", assistantId));

                if (!string.IsNullOrEmpty(hostName))
                {
                    command.Parameters.Add(new SqlParameter("@HostName", hostName));
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    command.Parameters.Add(new SqlParameter("@IPAddress", ipAddress));
                }

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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to run LocationService.AssistantLocationUpdated.");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated returned {updated}.");
            return updated;
        }
    }
}
