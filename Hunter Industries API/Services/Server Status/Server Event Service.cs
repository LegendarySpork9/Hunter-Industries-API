// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerEventService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;
        private readonly ServerInformationService _ServerInformationService;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerEventService(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database,
            ServerInformationService _serverInformationService)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
            _ServerInformationService = _serverInformationService;
        }

        /// <summary>
        /// Returns the latest component information record that matches the parameters.
        /// </summary>
        public async Task<List<ServerEventRecord>> GetServerEvents(string component)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.GetServerEvents called with the parameters \"{component}\".");

            List<ServerEventRecord> serverEvents = new List<ServerEventRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Event\GetServerEvents.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Component", SqlDbType.VarChar) { Value = component }
                };

                (List<ServerEventRecord> results, Exception ex) = await _Database.Query(sql, reader => new ServerEventRecord()
                {
                    Component = reader.GetString(0),
                    Status = reader.GetString(1),
                    DateOccured = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc),
                    Server = new RelatedServerRecord()
                    {
                        HostName = reader.GetString(2),
                        Game = reader.GetString(3),
                        GameVersion = reader.GetString(4)
                    }
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerEventService.GetServerEvents.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                serverEvents = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerEventService.GetServerEvents.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.GetServerEvents returned {serverEvents.Count} records.");
            return serverEvents;
        }

        /// <summary>
        /// Logs the server event.
        /// </summary>
        public async Task<(bool, int)> LogServerEvent(ServerEventModel serverEvent)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.LogServerEvent called with the parameters {_parameterFunction.FormatParameters(serverEvent)}.");

            bool logged = true;
            int componentInformationId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Event\LogServerEvent.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ServerID", SqlDbType.Int) { Value = await _ServerInformationService.GetServer(serverEvent.HostName, serverEvent.Game, serverEvent.GameVersion) },
                    new SqlParameter("@Component", SqlDbType.VarChar) { Value = serverEvent.Component },
                    new SqlParameter("@Status", SqlDbType.VarChar) { Value = serverEvent.Status }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerEventService.LogServerEvent.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    logged = false;
                }

                if (result == null)
                {
                    logged = false;
                }

                else
                {
                    componentInformationId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerEventService.LogServerEvent.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                logged = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.LogServerEvent returned {logged}.");
            return (logged, componentInformationId);
        }
    }
}
