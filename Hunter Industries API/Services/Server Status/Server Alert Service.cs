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
    public class ServerAlertService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ServerAlertService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns all server alert records that match the parameters.
        /// </summary>
        public (List<ServerAlertRecord>, int) GetServerAlerts(int pageSize, int pageNumber)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlerts called with the parameters {_parameterFunction.FormatParameters(new string[] { pageSize.ToString(), pageNumber.ToString() })}.");

            List<ServerAlertRecord> serverAlerts = new List<ServerAlertRecord>();

            int totalRecords = 0;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\GetServerAlerts.sql"), connection);
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    serverAlerts.Add(new ServerAlertRecord()
                    {
                        AlertId = dataReader.GetInt32(0),
                        Reporter = dataReader.GetString(1),
                        Component = dataReader.GetString(2),
                        ComponentStatus = dataReader.GetString(3),
                        AlertStatus = dataReader.GetString(4),
                        AlertDate = dataReader.GetDateTime(5),
                        server = new RelatedServerRecord()
                        {
                            HostName = dataReader.GetString(6),
                            Game = dataReader.GetString(7),
                            GameVersion = dataReader.GetString(8)
                        }
                    });
                }

                dataReader.Close();
                connection.Close();

                totalRecords = GetTotalServerAlerts();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetServerAlerts.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlerts returned {serverAlerts.Count} records | {totalRecords} total records.");
            return (serverAlerts, totalRecords);
        }

        /// <summary>
        /// Returns the server alert record for the given id.
        /// </summary>
        public ServerAlertRecord GetServerAlert(int id)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlert called with the parameters \"{id}\".");

            ServerAlertRecord serverAlert = new ServerAlertRecord();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\GetServerAlert.sql"), connection);
                command.Parameters.Add(new SqlParameter("@AlertID", id));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    serverAlert = new ServerAlertRecord()
                    {
                        AlertId = dataReader.GetInt32(0),
                        Reporter = dataReader.GetString(1),
                        Component = dataReader.GetString(2),
                        ComponentStatus = dataReader.GetString(3),
                        AlertStatus = dataReader.GetString(4),
                        AlertDate = dataReader.GetDateTime(5),
                        server = new RelatedServerRecord()
                        {
                            HostName = dataReader.GetString(6),
                            Game = dataReader.GetString(7),
                            GameVersion = dataReader.GetString(8)
                        }
                    };
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetServerAlert.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.GetServerAlert returned {serverAlert.AlertId.ToString() ?? "0"}.");
            return serverAlert;
        }

        /// <summary>
        /// Returns the number of server alert records in the table.
        /// </summary>
        private int GetTotalServerAlerts()
        {
            int totalRecords = 0;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\GetTotalServerAlerts.sql"), connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    totalRecords = dataReader.GetInt32(0);
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.GetTotalServerAlerts.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return totalRecords;
        }

        /// <summary>
        /// Logs the server alert.
        /// </summary>
        public (bool, int) LogServerAlert(ServerAlertModel serverAlert)
        {
            ServerInformationService _serverInformationService = new ServerInformationService(Logger);
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.LogServerAlert called with the parameters {_parameterFunction.FormatParameters(serverAlert)}.");

            bool logged = true;
            int serverAlertId = 0;

            SqlConnection connection;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\LogServerAlert.sql"), connection);
                command.Parameters.Add(new SqlParameter("@ServerID", _serverInformationService.GetServer(serverAlert.HostName, serverAlert.Game, serverAlert.GameVersion)));
                command.Parameters.Add(new SqlParameter("@Reporter", serverAlert.Reporter));
                command.Parameters.Add(new SqlParameter("@Component", serverAlert.Component));
                command.Parameters.Add(new SqlParameter("@ComponentStatus", serverAlert.ComponentStatus));
                command.Parameters.Add(new SqlParameter("@AlertStatus", serverAlert.AlertStatus));
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    connection.Close();
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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                logged = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.LogServerAlert returned {logged}.");
            return (logged, serverAlertId);
        }

        /// <summary>
        /// Returns whether a server alert exists with the given id.
        /// </summary>
        public bool ServerAlertExists(int id)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertExists called with the parameters \"{id}\".");

            bool exists = false;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\ServerAlertExists.sql"), connection);
                command.Parameters.Add(new SqlParameter("@AlertID", id));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    exists = true;
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerAlertService.ServerAlertExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Updates the status of the server alert.
        /// </summary>
        public bool ServerAlertUpdated(int id, string value)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertUpdated called with the parameters \"{id}\", \"{value}\".");

            bool updated = true;

            SqlConnection connection;
            SqlCommand command;

            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Alert\ServerAlertUpdated.sql"), connection);
                command.Parameters.Add(new SqlParameter("@AlertStatus", value));
                command.Parameters.Add(new SqlParameter("@AlertID", id));
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
                string message = "An error occured when trying to run ServerAlertService.ServerAlertUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerAlertService.ServerAlertUpdated returned {updated}.");
            return updated;
        }
    }
}