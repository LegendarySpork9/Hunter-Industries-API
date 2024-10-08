﻿// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects.Assistant;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Services.Assistant
{
    public class ConfigService
    {
        private readonly LoggerService Logger;

        public ConfigService(LoggerService _logger)
        {
            Logger = _logger;
        }

        public (List<AssistantConfiguration>, int, string) GetAssistantConfig(string? assistantName, string? assistantId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId })}.");

            List<AssistantConfiguration> assistantConfigurations = new();

            int totalConfigs = 0;
            string mostRecentVersion = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"\SQL\GetAssistantConfig.SQL"));

            if (!string.IsNullOrEmpty(assistantName))
            {
                sqlQuery += "\nand AI.Name = @AssistantName";
            }

            if (!string.IsNullOrEmpty(assistantId))
            {
                sqlQuery += "\nand AI.IDNumber = @AssistantID";
            }

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);

                if (sqlQuery.Contains("@AssistantName"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                }

                if (sqlQuery.Contains("@AssistantID"))
                {
                    command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    AssistantConfiguration configuration = new()
                    {
                        AssistantName = dataReader.GetString(0),
                        IdNumber = dataReader.GetString(1),
                        AssignedUser = dataReader.GetString(2),
                        HostName = dataReader.GetString(3),
                        Deletion = bool.Parse(dataReader.GetString(4)),
                        Version = dataReader.GetString(5)
                    };
                    
                    assistantConfigurations.Add(configuration);
                }

                dataReader.Close();
                connection.Close();

                totalConfigs = GetTotalConfigs(command);
                mostRecentVersion = GetMostRecentVersion();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetAssistantConfig.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.GetAssistantConfig returned {Logger.FormatParameters(new string[] { assistantConfigurations.Count.ToString(), totalConfigs.ToString(), mostRecentVersion })}.");
            return (assistantConfigurations, totalConfigs, mostRecentVersion);
        }

        private int GetTotalConfigs(SqlCommand command)
        {
            int totalRecords = 0;

            SqlConnection connection;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select count(*) from AssistantInformation AI with (nolock)";
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
                string message = "An error occured when trying to run ConfigService.GetTotalConfigs.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return totalRecords;
        }

        public string GetMostRecentVersion()
        {
            string version = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = @"select top 1 Value from [Version] with (nolock)
order by VersionID desc";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    version = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.GetMostRecentVersion.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return version;
        }

        public bool AssistantExists(string assistantName, string assistantId)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId })}.");

            string? name = null;
            string? idNumber = null;
            bool exists = false;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, @"\SQL\AssistantExists.SQL"));

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                command.Parameters.Add(new SqlParameter("@AssistantID", assistantId));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    name = dataReader.GetString(0);
                    idNumber = dataReader.GetString(1);

                    if ((name == assistantName && idNumber == assistantId) || idNumber == assistantId)
                    {
                        exists = true;
                    }
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantExists returned {exists}.");
            return exists;
        }

        public bool AssistantConfigCreated(string assistantName, string assistantId, string assignedUser, string hostName)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated called with the parameters {Logger.FormatParameters(new string[] { assistantName, assistantId, assignedUser, hostName })}.");

            bool created = true;

            SqlConnection connection;
            SqlCommand command;

            string sqlQuery = @"insert into [Location] (HostName, IPAddress)
output inserted.LocationID
values (@Hostname, @IPAddress)";
            int rowsAffected;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Hostname", hostName));
                command.Parameters.Add(new SqlParameter("@IPAddress", "PlaceHolder"));
                var locationId = command.ExecuteScalar();

                if (locationId == null)
                {
                    connection.Close();
                    created = false;
                }

                if (created)
                {
                    sqlQuery = @"insert into [User] (Name)
output inserted.UserID
values (@Name)";
                    rowsAffected = 0;

                    command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.Add(new SqlParameter("@Name", assignedUser));
                    var userId = command.ExecuteScalar();

                    if (userId == null)
                    {
                        connection.Close();
                        created = false;
                    }

                    if (created)
                    {
                        sqlQuery = @"insert into [AssistantInformation] (LocationID, DeletionStatusID, VersionID, UserID, Name, IDNumber)
values (@LocationID, 2, (select top 1 VersionID from [Version] with (nolock) order by VersionID desc), @UserID, @AssistantName, @IDNumber)";
                        rowsAffected = 0;

                        command = new SqlCommand(sqlQuery, connection);
                        command.Parameters.Add(new SqlParameter("@LocationID", locationId));
                        command.Parameters.Add(new SqlParameter("@UserID", userId));
                        command.Parameters.Add(new SqlParameter("@AssistantName", assistantName));
                        command.Parameters.Add(new SqlParameter("@IDNumber", assistantId));
                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            connection.Close();
                            created = false;
                        }
                    }
                }

                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigService.AssistantConfigCreated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigService.AssistantConfigCreated returned {created}.");
            return created;
        }
    }
}
