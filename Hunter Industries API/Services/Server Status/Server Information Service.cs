using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerInformationService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ServerInformationService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns all the servers that match the parameters.
        /// </summary>
        public List<ServerInformationRecord> GetServers(bool isActive)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServers called with the parameters \"{isActive}\".");

            List<ServerInformationRecord> servers = new List<ServerInformationRecord>();
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Information\GetServers.sql");

            if (isActive)
            {
                sqlQuery += "\nwhere IsActive = @IsActive";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        if (isActive)
                        {
                            command.Parameters.Add(new SqlParameter("@IsActive", isActive));
                        }

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                DowntimeRecord downtime = null;

                                if (!dataReader.IsDBNull(6) && !string.IsNullOrWhiteSpace(dataReader.GetString(6)))
                                {
                                    downtime = new DowntimeRecord()
                                    {
                                        Time = dataReader.GetString(6)
                                    };
                                }

                                servers.Add(new ServerInformationRecord()
                                {
                                    Id = dataReader.GetInt32(0),
                                    HostName = dataReader.GetString(1),
                                    Game = dataReader.GetString(2),
                                    GameVersion = dataReader.GetString(3),
                                    Connection = new ConnectionRecord()
                                    {
                                        IPAddress = dataReader.GetString(4),
                                        Port = dataReader.GetInt32(5),
                                    },
                                    Downtime = downtime,
                                    IsActive = dataReader.GetBoolean(7)
                                });
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServers.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServers returned {servers.Count} records.");
            return servers;
        }

        /// <summary>
        /// Returns the id of the server with the given values.
        /// </summary>
        public int GetServer(string hostName, string game, string gameVersion)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServer called with the parameters {_parameterFunction.FormatParameters(new string[] { hostName, game, gameVersion })}.");

            int serverId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Information\GetServer.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@HostName", hostName));
                        command.Parameters.Add(new SqlParameter("@Game", game));
                        command.Parameters.Add(new SqlParameter("@GameVersion", gameVersion));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                serverId = dataReader.GetInt32(0);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.GetServer.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.GetServer returned {serverId}.");
            return serverId;
        }

        /// <summary>
        /// Returns whether a server already exists with the given values.
        /// </summary>
        public bool ServerExists(string hostName, string game, string gameVersion)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerExists called with the parameters {_parameterFunction.FormatParameters(new string[] { hostName, game, gameVersion })}.");

            bool exists = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Information\ServerExists.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@HostName", hostName));
                        command.Parameters.Add(new SqlParameter("@Game", game));
                        command.Parameters.Add(new SqlParameter("@GameVersion", gameVersion));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                exists = true;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Adds the server.
        /// </summary>
        public (bool, int) ServerAdded(ServerInformationModel server)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerAdded called with the parameters {_parameterFunction.FormatParameters(null, server)}.");

            bool added = true;
            int serverId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Information\ServerAdded.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@HostName", server.HostName));
                        command.Parameters.Add(new SqlParameter("@Game", server.Game));
                        command.Parameters.Add(new SqlParameter("@GameVersion", server.GameVersion));
                        command.Parameters.Add(new SqlParameter("@IPAddress", server.IPAddress));
                        command.Parameters.Add(new SqlParameter("@Port", server.Port));
                        command.Parameters.Add(new SqlParameter("@Time", server.Time));
                        var result = command.ExecuteScalar();

                        if (result == null)
                        {
                            added = false;
                        }

                        else
                        {
                            serverId = int.Parse(result.ToString());
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerInformationService.ServerAdded.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                added = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerInformationService.ServerAdded returned {added}.");
            return (added, serverId);
        }
    }
}