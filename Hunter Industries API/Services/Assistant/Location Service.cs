// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Responses.Assistant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Assistant
{
    /// <summary>
    /// </summary>
    public class LocationService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public LocationService(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
        }

        /// <summary>
        /// Returns the location information about the given assistant.
        /// </summary>
        public async Task<LocationResponseModel> GetAssistantLocation(string assistantName, string assistantId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId })}.");

            LocationResponseModel location = new LocationResponseModel();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Location\GetAssistantLocation.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AssistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@AssistantID", SqlDbType.VarChar) { Value = assistantId }
                };

                (LocationResponseModel result, Exception ex) = await _Database.QuerySingle(sql, reader => new LocationResponseModel()
                {
                    AssistantName = reader.GetString(0),
                    IdNumber = reader.GetString(1),
                    HostName = reader.GetString(2),
                    IPAddress = reader.GetString(3)
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run LocationService.GetAssistantLocation.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    location = result;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run LocationService.GetAssistantLocation.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.GetAssistantLocation returned {ParameterFunction.FormatParameters(location)}.");
            return location;
        }

        /// <summary>
        /// Updates the location information of the given assistant.
        /// </summary>
        public async Task<bool> AssistantLocationUpdated(string assistantName, string assistantId, string hostName, string ipAddress)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { assistantName, assistantId, hostName, ipAddress })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Assistant\Location\AssistantLocationUpdated.sql");

                if (string.IsNullOrEmpty(hostName))
                {
                    sql = sql.Replace("HostName = @HostName, ", "");
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    sql = sql.Replace(", IPAddress = @IPAddress", "");
                }

                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@AssistantName", SqlDbType.VarChar) { Value = assistantName },
                    new SqlParameter("@IDNumber", SqlDbType.VarChar) { Value = assistantId }
                };

                if (!string.IsNullOrEmpty(hostName))
                {
                    parameterList.Add(new SqlParameter("@HostName", SqlDbType.VarChar) { Value = hostName });
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    parameterList.Add(new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = ipAddress });
                }

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run LocationService.AssistantLocationUpdated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (rowsAffected != 1)
                {
                    updated = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run LocationService.AssistantLocationUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"LocationService.AssistantLocationUpdated returned {updated}.");
            return updated;
        }
    }
}
