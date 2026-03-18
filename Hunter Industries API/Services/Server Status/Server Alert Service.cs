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
    public class ServerAlertService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;
        private readonly ServerInformationService _ServerInformationService;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerAlertService(ILoggerService _logger,
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
        /// Returns all server alert records that match the parameters.
        /// </summary>
        public async Task<(List<ServerAlertRecord>, int)> GetServerAlerts(int pageSize, int pageNumber)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlerts called with the parameters {_parameterFunction.FormatParameters(new string[] { pageSize.ToString(), pageNumber.ToString() })}.");

            List<ServerAlertRecord> serverAlerts = new List<ServerAlertRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\GetServerAlerts.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                };

                (List<ServerAlertRecord> results, Exception ex) = await _Database.Query(sql, reader => new ServerAlertRecord()
                {
                    AlertId = reader.GetInt32(0),
                    Reporter = reader.GetString(1),
                    Component = reader.GetString(2),
                    ComponentStatus = reader.GetString(3),
                    AlertStatus = reader.GetString(4),
                    AlertDate = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc),
                    server = new RelatedServerRecord()
                    {
                        HostName = reader.GetString(6),
                        Game = reader.GetString(7),
                        GameVersion = reader.GetString(8)
                    }
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.GetServerAlerts.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                serverAlerts = results;
                totalRecords = await GetTotalServerAlerts();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetServerAlerts.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlerts returned {serverAlerts.Count} records | {totalRecords} total records.");
            return (serverAlerts, totalRecords);
        }

        /// <summary>
        /// Returns the server alert record for the given id.
        /// </summary>
        public async Task<ServerAlertRecord> GetServerAlert(int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlert called with the parameters \"{id}\".");

            ServerAlertRecord serverAlert = new ServerAlertRecord();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\GetServerAlert.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AlertID", SqlDbType.Int) { Value = id }
                };

                (ServerAlertRecord result, Exception ex) = await _Database.QuerySingle(sql, reader => new ServerAlertRecord()
                {
                    AlertId = reader.GetInt32(0),
                    Reporter = reader.GetString(1),
                    Component = reader.GetString(2),
                    ComponentStatus = reader.GetString(3),
                    AlertStatus = reader.GetString(4),
                    AlertDate = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc),
                    server = new RelatedServerRecord()
                    {
                        HostName = reader.GetString(6),
                        Game = reader.GetString(7),
                        GameVersion = reader.GetString(8)
                    }
                }, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.GetServerAlert.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (result != null)
                {
                    serverAlert = result;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetServerAlert.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlert returned {serverAlert.AlertId.ToString() ?? "0"}.");
            return serverAlert;
        }

        /// <summary>
        /// Returns the number of server alert records in the table.
        /// </summary>
        private async Task<int> GetTotalServerAlerts()
        {
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\GetTotalServerAlerts.sql");
                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0));

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.GetTotalServerAlerts.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetTotalServerAlerts.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return totalRecords;
        }

        /// <summary>
        /// Logs the server alert.
        /// </summary>
        public async Task<(bool, int)> LogServerAlert(ServerAlertModel serverAlert)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.LogServerAlert called with the parameters {_parameterFunction.FormatParameters(serverAlert)}.");

            bool logged = true;
            int serverAlertId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\LogServerAlert.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ServerID", SqlDbType.Int) { Value = await _ServerInformationService.GetServer(serverAlert.HostName, serverAlert.Game, serverAlert.GameVersion) },
                    new SqlParameter("@Reporter", SqlDbType.VarChar) { Value = serverAlert.Reporter },
                    new SqlParameter("@Component", SqlDbType.VarChar) { Value = serverAlert.Component },
                    new SqlParameter("@ComponentStatus", SqlDbType.VarChar) { Value = serverAlert.ComponentStatus },
                    new SqlParameter("@AlertStatus", SqlDbType.VarChar) { Value = serverAlert.AlertStatus }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.LogServerAlert.";
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
                    serverAlertId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.LogServerAlert.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                logged = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.LogServerAlert returned {logged}.");
            return (logged, serverAlertId);
        }

        /// <summary>
        /// Returns whether a server alert exists with the given id.
        /// </summary>
        public async Task<bool> ServerAlertExists(int id)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertExists called with the parameters \"{id}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\ServerAlertExists.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AlertID", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.ServerAlertExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.ServerAlertExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Updates the status of the server alert.
        /// </summary>
        public async Task<bool> ServerAlertUpdated(int id, string value)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertUpdated called with the parameters \"{id}\", \"{value}\".");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Alerts\ServerAlertUpdated.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@AlertStatus", SqlDbType.VarChar) { Value = value },
                    new SqlParameter("@AlertID", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerAlertService.ServerAlertUpdated.";
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
                string message = "An error occured when trying to run ServerAlertService.ServerAlertUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertUpdated returned {updated}.");
            return updated;
        }
    }
}
