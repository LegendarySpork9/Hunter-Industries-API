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
    public class ServerEventService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ServerEventService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns the latest component information record that matches the parameters.
        /// </summary>
        public List<ServerEventRecord> GetServerEvents(string component)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.GetServerEvents called with the parameters \"{component}\".");

            List<ServerEventRecord> serverEvents = new List<ServerEventRecord>();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Event\GetServerEvents.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Component", component));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    serverEvents.Add(new ServerEventRecord()
                    {
                        Component = dataReader.GetString(0),
                        Status = dataReader.GetString(1),
                        DateOccured = dataReader.GetDateTime(5),
                        Server = new RelatedServerRecord()
                        {
                            HostName = dataReader.GetString(2),
                            Game = dataReader.GetString(3),
                            GameVersion = dataReader.GetString(4)
                        }
                    });
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ServerEventService.GetServerEvents.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.GetServerEvents returned {serverEvents.Count} records.");
            return serverEvents;
        }

        /// <summary>
        /// Logs the server event.
        /// </summary>
        public (bool, int) LogServerEvent(ServerEventModel serverEvent)
        {
            ServerInformationService _serverInformationService = new ServerInformationService(Logger);
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.LogServerEvent called with the parameters {_parameterFunction.FormatParameters(serverEvent)}.");

            bool logged = true;
            int componentInformationId = 0;

            SqlConnection connection;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Server Status\Server Event\LogServerEvent.sql"), connection);
                command.Parameters.Add(new SqlParameter("@ServerID", _serverInformationService.GetServer(serverEvent.HostName, serverEvent.Game, serverEvent.GameVersion)));
                command.Parameters.Add(new SqlParameter("@Component", serverEvent.Component));
                command.Parameters.Add(new SqlParameter("@Status", serverEvent.Status));
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    connection.Close();
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
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                logged = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ServerEventService.LogServerEvent returned {logged}.");
            return (logged, componentInformationId);
        }
    }
}