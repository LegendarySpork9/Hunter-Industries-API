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
    public class LocationService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public LocationService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns the location information about the given assistant.
        /// </summary>
        public LocationResponseModel GetAssistantLocation(string assistantName, string assistantId)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            LocationResponseModel location = new LocationResponseModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Location\GetAssistantLocation.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                        command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                location = new LocationResponseModel()
                                {
                                    AssistantName = dataReader.GetString(0),
                                    IdNumber = dataReader.GetString(1),
                                    HostName = dataReader.GetString(2),
                                    IPAddress = dataReader.GetString(3)
                                };
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run LocationService.GetAssistantLocation.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation returned {_parameterFunction.FormatParameters(location)}.");
            return location;
        }

        /// <summary>
        /// Updates the location information of the given assistant.
        /// </summary>
        public bool AssistantLocationUpdated(string assistantName, string assistantId, string hostName, string ipAddress)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated called with the parameters {_parameterFunction.FormatParameters(new string[] { assistantName, assistantId, hostName, ipAddress })}.");

            bool updated = true;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Assistant\Location\AssistantLocationUpdated.sql");
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
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
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
                            updated = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run LocationService.AssistantLocationUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated returned {updated}.");
            return updated;
        }
    }
}