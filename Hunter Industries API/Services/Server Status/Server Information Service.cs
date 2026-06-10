// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
        public ServerInformationService(
            ILoggerService _logger,
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
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.GetServers called with the parameter \"{isActive}\".");

            List<ServerInformationRecord> servers = new List<ServerInformationRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "GetServers.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (isActive)
                {
                    sql += "\nwhere IsActive = @isActive";
                    parameterList.Add(new SqlParameter("@isActive", SqlDbType.Bit) { Value = isActive });
                }

                (List<ServerInformationRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader =>
                    {
                        DowntimeRecord downtime = null;

                        if (!reader.IsDBNull(7) && !string.IsNullOrWhiteSpace(reader.GetString(7)))
                        {
                            downtime = new DowntimeRecord()
                            {
                                Time = reader.GetString(7),
                                Duration = reader.GetInt32(8)
                            };
                        }

                        return new ServerInformationRecord()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            HostName = reader.GetString(2),
                            Game = reader.GetString(3),
                            GameVersion = reader.GetString(4),
                            Connection = new ConnectionRecord()
                            {
                                IPAddress = reader.GetString(5),
                                Port = reader.GetInt32(6),
                            },
                            Downtime = downtime,
                            EventInterval = reader.GetInt32(9),
                            IsActive = reader.GetBoolean(10)
                        };
                    },
                    parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.GetServers.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                servers = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServers.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.GetServers returned {servers.Count} records.");
            return servers;
        }

        /// <summary>
        /// Returns the server record for the given id..
        /// </summary>
        public async Task<ServerInformationRecord> GetServer(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.GetServer called with the parameter \"{id}\".");

            ServerInformationRecord server = new ServerInformationRecord();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "GetServers.sql"));
                sql += @"
where ServerInformationId = @serverId";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@serverId", SqlDbType.Int) { Value = id }
                };

                (ServerInformationRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader =>
                    {
                        DowntimeRecord downtime = null;

                        if (!reader.IsDBNull(7) && !string.IsNullOrWhiteSpace(reader.GetString(7)))
                        {
                            downtime = new DowntimeRecord()
                            {
                                Time = reader.GetString(7),
                                Duration = reader.GetInt32(8)
                            };
                        }

                        return new ServerInformationRecord()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            HostName = reader.GetString(2),
                            Game = reader.GetString(3),
                            GameVersion = reader.GetString(4),
                            Connection = new ConnectionRecord()
                            {
                                IPAddress = reader.GetString(5),
                                Port = reader.GetInt32(6),
                            },
                            Downtime = downtime,
                            EventInterval = reader.GetInt32(9),
                            IsActive = reader.GetBoolean(10)
                        };
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.GetServer.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                server = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServer.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            int serverId = 0;

            if (server != null)
            {
                serverId = server.Id;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.GetServer returned {serverId}.");
            return server;
        }

        /// <summary>
        /// Returns whether a server already exists with the given values.
        /// </summary>
        public async Task<bool> ServerExists(string name)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerExists called with the parameter \"{name}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "ServerExists.sql"));
                sql += @"
where ServerInformation.[Name] = @name";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = name }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a server already exists with the given values.
        /// </summary>
        public async Task<bool> ServerExists(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerExists called with the parameter \"{id}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "ServerExists.sql"));
                sql += @"
where ServerInformationId = @serverId";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@serverId", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Adds the server.
        /// </summary>
        public async Task<(bool, int)> ServerAdded(ServerInformationModel server)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerAdded called with the parameters {ParameterFunction.FormatParameters(server)}.");

            bool added = true;
            int serverId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "ServerAdded.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = server.Name },
                    new SqlParameter("@eventInterval", SqlDbType.Int) { Value = server.EventInterval },
                    new SqlParameter("@hostName", SqlDbType.VarChar) { Value = server.HostName },
                    new SqlParameter("@game", SqlDbType.VarChar) { Value = server.Game },
                    new SqlParameter("@gameVersion", SqlDbType.VarChar) { Value = server.GameVersion },
                    new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = server.IPAddress },
                    new SqlParameter("@port", SqlDbType.Int) { Value = server.Port },
                    new SqlParameter("@time", SqlDbType.VarChar) { Value = (object)server.Time ?? DBNull.Value },
                    new SqlParameter("@duration", SqlDbType.Int) { Value = (object)server.Duration ?? DBNull.Value }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerAdded.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);

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
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error, 
                    ex.ToString(),
                    message);

                added = false;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerAdded returned {added}.");
            return (
                added,
                serverId);
        }

        /// <summary>
        /// Updates the details of the server.
        /// </summary>
        public async Task<bool> ServerUpdated(
            int id,
            ServerUpdateModel server)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerUpdated called with the parameters \"{id}\", {ParameterFunction.FormatParameters(server)}.");

            bool updated = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Server Status",
                    "Server Information",
                    "ServerUpdated.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>()
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = server.Name },
                    new SqlParameter("@eventInterval", SqlDbType.Int) { Value = server.EventInterval },
                    new SqlParameter("@active", SqlDbType.Bit) { Value = server.IsActive ?? false },
                    new SqlParameter("@hostName", SqlDbType.VarChar) { Value = server.HostName },
                    new SqlParameter("@game", SqlDbType.VarChar) { Value = server.Game },
                    new SqlParameter("@gameVersion", SqlDbType.VarChar) { Value = server.GameVersion },
                    new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = server.IPAddress },
                    new SqlParameter("@port", SqlDbType.Int) { Value = server.Port },
                    new SqlParameter("@time", SqlDbType.VarChar) { Value = server.Time },
                    new SqlParameter("@duration", SqlDbType.Int) { Value = server.Duration },
                    new SqlParameter("@serverId", SqlDbType.Int) { Value = id }
                };

                if (string.IsNullOrWhiteSpace(server.Name))
                {
                    sql = sql.Replace(@"
	[Name] = @name,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@name"));

                }

                if (server.EventInterval == 0)
                {
                    sql = sql.Replace(@"
	EventInterval = @eventInterval,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@eventInterval"));
                }

                if (!server.IsActive.HasValue)
                {
                    sql = sql.Replace(@"
	IsActive = @active,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@active"));
                }

                if (string.IsNullOrWhiteSpace(server.HostName))
                {
                    sql = sql.Replace(@"
	MachineId = M.MachineId,", "")
                        .Replace(@"
join Machine M with (nolock) on M.HostName = @hostName", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@hostName"));
                }

                if (string.IsNullOrWhiteSpace(server.Game) || string.IsNullOrWhiteSpace(server.GameVersion))
                {
                    sql = sql.Replace(@"
	GameId = G.GameId,", "")
                        .Replace(@"
join Game G with (nolock) on G.[Name] = @game
    and G.[Version] = @gameVersion", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@game"));
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@gameVersion"));
                }

                if (string.IsNullOrWhiteSpace(server.IPAddress) || server.Port == 0)
                {
                    sql = sql.Replace(@"
	ConnectionId = C.ConnectionId,", "")
                        .Replace(@"
join [Connection] C with (nolock) on C.IPAddress = @ipAddress
    and C.[Port] = @port", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@ipAddress"));
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@port"));
                }

                if (server.ClearDowntime.HasValue && server.ClearDowntime.Value)
                {
                    sql = sql.Replace(@"D.DowntimeId", "null")
                        .Replace(@"
left join Downtime D with (nolock) on D.[Time] = @time
    and D.Duration = @duration", "");

                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@time"));
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@duration"));
                }

                else
                {
                    if (string.IsNullOrWhiteSpace(server.Time) || server.Duration == 0)
                    {
                        sql = sql.Replace(@"
	DowntimeId = D.DowntimeId", "")
                            .Replace(@"
left join Downtime D with (nolock) on D.[Time] = @time
    and D.Duration = @duration", "");
                        parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@time"));
                        parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@duration"));
                    }
                }

                List<string> sqlLines = sql.Split(new[]
                {
                    Environment.NewLine,
                    "\n",
                    "\r\n"
                },
                StringSplitOptions.None)
                    .ToList();
                string lastSet = sqlLines.LastOrDefault(s => s.Contains(','));

                if (lastSet != null)
                {
                    int index = sqlLines.IndexOf(lastSet);
                    sqlLines[index] = sqlLines[index].Replace(",", "");
                }

                sql = string.Join(
                    Environment.NewLine,
                    sqlLines);

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ServerInformationService.ServerUpdated.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (rowsAffected == 1)
                {
                    updated = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerUpdated.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"ServerInformationService.ServerUpdated returned {updated}.");
            return updated;
        }
    }
}
