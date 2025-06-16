using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using HunterIndustriesAPI.Objects.User;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace HunterIndustriesAPI.Services.User
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
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\GetUsers.sql");

            if (id != 0)
            {
                sqlQuery += "\nand UserID = @Id";
            }

            if (!string.IsNullOrEmpty(username))
            {
                sqlQuery += "\nand Username = @Username";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        if (sqlQuery.Contains("@Id"))
                        {
                            command.Parameters.Add(new SqlParameter("@Id", id));
                        }

                        if (sqlQuery.Contains("@Username"))
                        {
                            command.Parameters.Add(new SqlParameter("@Username", username));
                        }

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                UserRecord user = new UserRecord
                                {
                                    Id = dataReader.GetInt32(0),
                                    Username = dataReader.GetString(1),
                                    Password = dataReader.GetString(2),
                                    Scopes = GetUserScopes(dataReader.GetInt32(0))
                                };

                                users.Add(user);
                            }
                        }
                    }
                }
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
        /// Returns whether a user already exists with the given username.
        /// </summary>
        public bool UserExists(string username)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists called with the parameters {_parameterFunction.FormatParameters(new string[] { username })}.");

            bool exists = false;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\UserExists.sql");
            sqlQuery += "\nand Username = @Username";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", username));

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
                string message = "An error occured when trying to run UserService.UserExists.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a user already exists with the given id.
        /// </summary>
        public bool UserExists(int id)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            bool exists = false;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\UserExists.sql");
            sqlQuery += "\nand UserID = @Id";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Id", id));

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

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\CreateUser.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", username));
                        command.Parameters.Add(new SqlParameter("@Password", _hashFunction.HashString(password)));
                        var result = command.ExecuteScalar();

                        if (result == null)
                        {
                            created = false;
                        }

                        else
                        {
                            userId = int.Parse(result.ToString());
                        }
                    }
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

        /// <summary>
        /// Creates the user scopes.
        /// </summary>
        public bool UserScopeCreated(int id, List<string> scopes)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            HashFunction _hashFunction = new HashFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeCreated called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), _parameterFunction.FormatParameters(scopes, false) })}.");

            bool created = true;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    foreach (string scope in scopes)
                    {
                        using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\CreateUserScope.sql"), connection))
                        {
                            command.Parameters.Add(new SqlParameter("@UserID", id));
                            command.Parameters.Add(new SqlParameter("@Scope", scope));
                            var result = command.ExecuteScalar();

                            if (result == null)
                            {
                                created = false;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserScopeCreated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Gets the scopes assigned to the user.
        /// </summary>
        public List<string> GetUserScopes(int id = 0, string username = null)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUserScopes called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), username })}.");

            List<string> scopes = new List<string>();
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\GetUserScopes.sql");

            if (id != 0)
            {
                sqlQuery += "\nand APIUser.UserID = @Id";
            }

            if (!string.IsNullOrEmpty(username))
            {
                sqlQuery += "\nand APIUser.Username = @Username";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        if (sqlQuery.Contains("@Id"))
                        {
                            command.Parameters.Add(new SqlParameter("@Id", id));
                        }

                        if (sqlQuery.Contains("@Username"))
                        {
                            command.Parameters.Add(new SqlParameter("@Username", username));
                        }

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                scopes.Add(dataReader.GetString(0));
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.GetUserScopes.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUserScopes returned {scopes.Count} records.");
            return scopes;
        }

        /// <summary>
        /// Updates the details of the given user.
        /// </summary>
        public bool UserUpdated(int id, string username, string password, List<KeyValuePair<string, string>> scopes)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            HashFunction _hashFunction = new HashFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserUpdated called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), username, password, _parameterFunction.FormatParameters(scopes, true) })}.");

            bool updated = true;
            string sqlQuery = File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\UserUpdated.sql");
            int rowsAffected;

            if (string.IsNullOrEmpty(username))
            {
                sqlQuery = sqlQuery.Replace("Username = @username,", "");
            }

            if (string.IsNullOrEmpty(password))
            {
                sqlQuery = sqlQuery.Replace(", [Password] = @password", "");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@userId", id));

                        if (!string.IsNullOrEmpty(username))
                        {
                            command.Parameters.Add(new SqlParameter("@username", username));
                        }

                        if (!string.IsNullOrEmpty(password))
                        {
                            command.Parameters.Add(new SqlParameter("@password", password));
                        }

                        rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            updated = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserUpdated.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                updated = false;
            }

            updated = UserScopeCreated(id, scopes.Where(c => c.Key == "Add").Select(c => c.Value).ToList());
            updated = UserScopeDeleted(id, scopes.Where(c => c.Key == "Remove").Select(c => c.Value).ToList());

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Deletes the user scopes.
        /// </summary>
        private bool UserScopeDeleted(int id, List<string> scopes)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();
            HashFunction _hashFunction = new HashFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeDeleted called with the parameters {_parameterFunction.FormatParameters(new string[] { id.ToString(), _parameterFunction.FormatParameters(scopes, false) })}.");

            bool deleted = true;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    foreach (string scope in scopes)
                    {
                        using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\DeleteUserScope.sql"), connection))
                        {
                            command.Parameters.Add(new SqlParameter("@UserID", id));
                            command.Parameters.Add(new SqlParameter("@Scope", scope));
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected != 1)
                            {
                                deleted = false;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserScopeDeleted.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                deleted = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeDeleted returned {deleted}.");
            return deleted;
        }

        /// <summary>
        /// Sets the user to deleted.
        /// </summary>
        public bool UserDeleted(int id)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserDeleted called with the parameters \"{id}\".");

            bool deleted = true;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\User\UserDeleted.sql"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            deleted = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserDeleted.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                deleted = false;
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserDeleted returned {deleted}.");
            return deleted;
        }
    }
}