using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class UserService
    {
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public UserService(LoggerService _logger)
        {
            Logger = _logger;
        }

        /// <summary>
        /// Returns all user records that match the parameters.
        /// </summary>
        public List<UserRecord> GetUsers(int id, string username)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUsers called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), username })}.");
            
            List<UserRecord> users = new List<UserRecord>();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\GetUsers.sql");

            if (id != 0)
            {
                sqlQuery += "\nand UserID = @id";
            }

            if (!string.IsNullOrEmpty(username))
            {
                sqlQuery += "\nand Username = @Username";
            }

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);

                if (sqlQuery.Contains("@id"))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                }

                if (sqlQuery.Contains("@Username"))
                {
                    command.Parameters.Add(new SqlParameter("@Username", username));
                }

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    UserRecord user = new UserRecord
                    {
                        Id = dataReader.GetInt32(0),
                        Username = dataReader.GetString(1),
                        Password = dataReader.GetString(2)
                    };

                    users.Add(user);
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.GetUsers.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUsers returned {users.Count} records.");
            return users;
        }

        /// <summary>
        /// Returns whether a user already exists with the given details.
        /// </summary>
        public bool UserExists(string username)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists called with the parameters {_parameterFunction.FormatParameters(new string[] { username })}.");

            bool exists = false;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\UserExists.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Username", username));
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
                string message = "An error occured when trying to run UserService.UserExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        public (bool, int) UserCreated(string username, string password)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            HashFunction _hashFunction = new HashFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserCreated called with the parameters {_parameterFunction.FormatParameters(new string[] { username, password })}.");

            bool created = true;
            int userId = 0;

            SqlConnection connection;
            SqlCommand command;

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\CreateUser.sql"), connection);
                command.Parameters.Add(new SqlParameter("@Username", username));
                command.Parameters.Add(new SqlParameter("@Password", _hashFunction.HashString(password)));
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    connection.Close();
                    created = false;
                }

                else
                {
                    userId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserCreated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserCreated returned {created}.");
            return (created, userId);
        }
    }
}