// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class LocationService
    {
        // Gets the host name and ip address of the given assistant.
        public LocationResponseModel GetAssistantLocation(string assistantName, string assistantId)
        {
            try
            {
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
                        AssistantId = dataReader.GetString(1),
                        HostName = dataReader.GetString(2),
                        IPAddress = dataReader.GetString(3)
                    };
                }

                dataReader.Close();
                connection.Close();

                return location;
            }

            catch (Exception ex)
            {
                return new LocationResponseModel();
            }
        }

        // Updates the host name or ip address of the given assistant.
        public bool AssistantLocationUpdated(string assistantName, string assistantId, string? hostName, string? ipAddress)
        {
            try
            {
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
