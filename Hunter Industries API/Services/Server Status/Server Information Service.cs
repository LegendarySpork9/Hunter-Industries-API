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
    public class ServerInformationService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ServerInformationService(ILoggerService _logger,
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
        /// Returns all the servers that match the parameters.
        /// </summary>
        public async Task<List<ServerInformationRecord>> GetServers(bool isActive)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServers called with the parameters {ParameterFunction.FormatParameters(new string[] { isActive.ToString() })}.");

            List<ServerInformationRecord> servers = new List<ServerInformationRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Information\GetServers.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (isActive)
                {
                    sql += "\nwhere IsActive = @IsActive";
                    parameterList.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive });
                }

                (List<ServerInformationRecord> results, Exception ex) = await _Database.Query(sql, reader =>
                {
                    DowntimeRecord downtime = null;

                    if (!reader.IsDBNull(6) && !string.IsNullOrWhiteSpace(reader.GetString(6)))
                    {
                        downtime = new DowntimeRecord()
                        {
                            Time = reader.GetString(6)
                        };
                    }

                    return new ServerInformationRecord()
                    {
                        Id = reader.GetInt32(0),
                        HostName = reader.GetString(1),
                        Game = reader.GetString(2),
                        GameVersion = reader.GetString(3),
                        Connection = new ConnectionRecord()
                        {
                            IPAddress = reader.GetString(4),
                            Port = reader.GetInt32(5),
                        },
                        Downtime = downtime,
                        IsActive = reader.GetBoolean(7)
                    };
                }, parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.GetServers.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                servers = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServers.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServers returned {servers.Count} records.");
            return servers;
        }

        /// <summary>
        /// Returns the id of the server with the given values.
        /// </summary>
        public async Task<int> GetServer(string hostName, string game, string gameVersion)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServer called with the parameters {ParameterFunction.FormatParameters(new string[] { hostName, game, gameVersion })}.");

            int serverId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Information\GetServer.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@HostName", SqlDbType.VarChar) { Value = hostName },
                    new SqlParameter("@Game", SqlDbType.VarChar) { Value = game },
                    new SqlParameter("@GameVersion", SqlDbType.VarChar) { Value = gameVersion }
                };

                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.GetServer.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                serverId = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServer.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServer returned {serverId}.");
            return serverId;
        }

        /// <summary>
        /// Returns whether a server already exists with the given values.
        /// </summary>
        public async Task<bool> ServerExists(string hostName, string game, string gameVersion)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerExists called with the parameters {ParameterFunction.FormatParameters(new string[] { hostName, game, gameVersion })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Information\ServerExists.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@HostName", SqlDbType.VarChar) { Value = hostName },
                    new SqlParameter("@Game", SqlDbType.VarChar) { Value = game },
                    new SqlParameter("@GameVersion", SqlDbType.VarChar) { Value = gameVersion }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerExists.";
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
                string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Adds the server.
        /// </summary>
        public async Task<(bool, int)> ServerAdded(ServerInformationModel server)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerAdded called with the parameters {ParameterFunction.FormatParameters(null, server)}.");

            bool added = true;
            int serverId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Server Status\Server Information\ServerAdded.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@HostName", SqlDbType.VarChar) { Value = server.HostName },
                    new SqlParameter("@Game", SqlDbType.VarChar) { Value = server.Game },
                    new SqlParameter("@GameVersion", SqlDbType.VarChar) { Value = server.GameVersion },
                    new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = server.IPAddress },
                    new SqlParameter("@Port", SqlDbType.Int) { Value = server.Port },
                    new SqlParameter("@Time", SqlDbType.VarChar) { Value = server.Time ?? "null" }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerAdded.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    added = false;
                }

                if (result == null)
                {
                    added = false;
                }

                else
                {
                    serverId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerAdded.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                added = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerAdded returned {added}.");
            return (added, serverId);
        }
    }
}
