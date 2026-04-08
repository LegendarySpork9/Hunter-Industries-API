// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Objects.User;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using HunterIndustriesAPICommon.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.User
{
    /// <summary>
    /// </summary>
    public class UserService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public UserService(ILoggerService _logger,
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
        /// Returns all user records that match the parameters.
        /// </summary>
        public async Task<List<UserRecord>> GetUsers(int id, string username)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUsers called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), username })}.");

            List<UserRecord> users = new List<UserRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\GetUsers.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (id != 0)
                {
                    sql += "\nand UserID = @id";
                    parameterList.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand Username = @username";
                    parameterList.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                }

                (List<(int, string, string)> results, Exception ex) = await _Database.Query(sql, reader => (reader.GetInt32(0), reader.GetString(1), reader.GetString(2)), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.GetUsers.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                foreach (var result in results)
                {
                    UserRecord user = new UserRecord
                    {
                        Id = result.Item1,
                        Username = result.Item2,
                        Password = result.Item3,
                        Scopes = await GetUserScopes(result.Item1)
                    };

                    users.Add(user);
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.GetUsers.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUsers returned {users.Count} records.");
            return users;
        }

        /// <summary>
        /// Returns whether a user already exists with the given username.
        /// </summary>
        public async Task<bool> UserExists(string username)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists called with the parameters {ParameterFunction.FormatParameters(new string[] { username })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\UserExists.sql");
                sql += "\nand Username = @username";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = username }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.UserExists.";
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
                string message = "An error occured when trying to run UserService.UserExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a user already exists with the given id.
        /// </summary>
        public async Task<bool> UserExists(int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\UserExists.sql");
                sql += "\nand UserID = @id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@id", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.UserExists.";
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
                string message = "An error occured when trying to run UserService.UserExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        public async Task<(bool, int)> UserCreated(string username, string password)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserCreated called with the parameters {ParameterFunction.FormatParameters(new string[] { username, HashFunction.HashString(password) })}.");

            bool created = true;
            int userId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\CreateUser.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@username", SqlDbType.VarChar) { Value = username },
                    new SqlParameter("@password", SqlDbType.VarChar) { Value = HashFunction.HashString(password) }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.UserCreated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    created = false;
                }

                if (result == null)
                {
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
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserCreated returned {created}.");
            return (created, userId);
        }

        /// <summary>
        /// Creates the user scopes.
        /// </summary>
        public async Task<bool> UserScopeCreated(int id, List<string> scopes)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeCreated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), ParameterFunction.FormatListParameters(scopes, false) })}.");

            bool created = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\CreateUserScope.sql");

                foreach (string scope in scopes)
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@userID", SqlDbType.Int) { Value = id },
                        new SqlParameter("@scope", SqlDbType.VarChar) { Value = scope }
                    };

                    (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run UserService.UserScopeCreated.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    if (result == null)
                    {
                        created = false;
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserScopeCreated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Gets the scopes assigned to the user.
        /// </summary>
        public async Task<List<string>> GetUserScopes(int id = 0, string username = null)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUserScopes called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), username })}.");

            List<string> scopes = new List<string>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\GetUserScopes.sql");
                List<SqlParameter> parameterList = new List<SqlParameter>();

                if (id != 0)
                {
                    sql += "\nand APIUser.UserID = @id";
                    parameterList.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                }

                if (!string.IsNullOrEmpty(username))
                {
                    sql += "\nand APIUser.Username = @username";
                    parameterList.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                }

                (List<string> results, Exception ex) = await _Database.Query(sql, reader => reader.GetString(0), parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.GetUserScopes.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                scopes = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.GetUserScopes.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.GetUserScopes returned {scopes.Count} records.");
            return scopes;
        }

        /// <summary>
        /// Updates the details of the given user.
        /// </summary>
        public async Task<bool> UserUpdated(int id, string username, string password, List<KeyValuePair<string, string>> scopes)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), username, password, ParameterFunction.FormatListParameters(scopes, true) })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\UserUpdated.sql");

                if (string.IsNullOrEmpty(username))
                {
                    sql = sql.Replace("Username = @username,", "");
                }

                if (string.IsNullOrEmpty(password))
                {
                    sql = sql.Replace(", [Password] = @password", "");
                }

                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@userId", SqlDbType.Int) { Value = id }
                };

                if (!string.IsNullOrEmpty(username))
                {
                    parameterList.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
                }

                if (!string.IsNullOrEmpty(password))
                {
                    parameterList.Add(new SqlParameter("@password", SqlDbType.VarChar) { Value = password });
                }

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.UserUpdated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    updated = false;
                }

                if (rowsAffected != 1)
                {
                    updated = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                updated = false;
            }

            updated = await UserScopeCreated(id, scopes.Where(c => c.Key == "Add").Select(c => c.Value).ToList());
            updated = await UserScopeDeleted(id, scopes.Where(c => c.Key == "Remove").Select(c => c.Value).ToList());

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Deletes the user scopes.
        /// </summary>
        private async Task<bool> UserScopeDeleted(int id, List<string> scopes)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeDeleted called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), ParameterFunction.FormatListParameters(scopes, false) })}.");

            bool deleted = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\DeleteUserScope.sql");

                foreach (string scope in scopes)
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@userID", SqlDbType.Int) { Value = id },
                        new SqlParameter("@scope", SqlDbType.VarChar) { Value = scope }
                    };

                    (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run UserService.UserScopeDeleted.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    if (rowsAffected != 1)
                    {
                        deleted = false;
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserScopeDeleted.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                deleted = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserScopeDeleted returned {deleted}.");
            return deleted;
        }

        /// <summary>
        /// Sets the user to deleted.
        /// </summary>
        public async Task<bool> UserDeleted(int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserDeleted called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString() })}.");

            bool deleted = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\User\UserDeleted.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@userID", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run UserService.UserDeleted.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (rowsAffected != 1)
                {
                    deleted = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run UserService.UserDeleted.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                deleted = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"UserService.UserDeleted returned {deleted}.");
            return deleted;
        }
    }
}
