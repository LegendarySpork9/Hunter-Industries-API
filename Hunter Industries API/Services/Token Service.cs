using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class TokenService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;
        private readonly string Phrase;

        private string ProgramName;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public TokenService(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database,
            string phrase)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
            Phrase = phrase;
        }

        /// <summary>
        /// Returns the name of the application making the call.
        /// </summary>
        public async Task<string> ApplicationName()
        {
            if (ProgramName == null)
            {
                ProgramName = await GetApplicationName(Phrase);
            }

            return ProgramName;
        }

        /// <summary>
        /// Returns all username and passwords.
        /// </summary>
        public async Task<(string[], string[])> GetUsers()
        {
            string[] usernames = Array.Empty<string>();
            string[] passwords = Array.Empty<string>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Token\GetUsers.SQL");
                (List<(string, string)> results, Exception ex) = await _Database.Query(sql, reader => (reader.GetString(1), reader.GetString(2)));

                if (ex != null)
                {
                    string message = "An error occured when trying to run TokenService.GetUsers.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                usernames = results.Select(r => r.Item1).ToArray();
                passwords = results.Select(r => r.Item2).ToArray();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetUsers.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return (usernames, passwords);
        }

        /// <summary>
        /// Returns all application phrases.
        /// </summary>
        public async Task<string[]> GetAuthorisationPhrases()
        {
            string[] phrases = Array.Empty<string>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Token\GetAuthorisationPhrases.SQL");
                (List<string> results, Exception ex) = await _Database.Query(sql, reader => reader.GetString(1));

                if (ex != null)
                {
                    string message = "An error occured when trying to run TokenService.GetAuthorisationPhrases.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                phrases = results.ToArray();
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetAuthorisationPhrases.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return phrases;
        }

        /// <summary>
        /// Gets the application name from the database.
        /// </summary>
        private async Task<string> GetApplicationName(string phrase)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName called with authorisation phrase {_parameterFunction.FormatParameters(new[] { phrase })}.");

            string name = string.Empty;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Token\GetApplicationName.SQL");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = phrase }
                };

                (string result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetString(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run TokenService.GetApplicationName.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                name = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetApplicationName.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName returned {name}.");
            return name;
        }
    }
}
