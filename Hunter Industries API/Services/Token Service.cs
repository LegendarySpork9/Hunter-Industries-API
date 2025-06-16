using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class TokenService
    {
        private readonly string ProgramName;
        private readonly LoggerService Logger;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public TokenService(string phrase, LoggerService _logger)
        {
            Logger = _logger;
            ProgramName = GetApplicationName(phrase);
        }

        /// <summary>
        /// Returns the name of the application making the call.
        /// </summary>
        public string ApplicationName()
        {
            return ProgramName;
        }

        /// <summary>
        /// Returns all username and passwords.
        /// </summary>
        public (string[], string[]) GetUsers()
        {
            string[] usernames = Array.Empty<string>();
            string[] passwords = Array.Empty<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Token\GetUsers.SQL"), connection))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                usernames = usernames.Append(dataReader.GetString(1)).ToArray();
                                passwords = passwords.Append(dataReader.GetString(2)).ToArray();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetUsers.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return (usernames, passwords);
        }

        /// <summary>
        /// Returns all application phrases.
        /// </summary>
        public string[] GetAuthorisationPhrases()
        {
            string[] phrases = Array.Empty<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Token\GetAuthorisationPhrases.SQL"), connection))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                phrases = phrases.Append(dataReader.GetString(1)).ToArray();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetAuthorisationPhrases.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            return phrases;
        }

        /// <summary>
        /// Gets the application name from the database.
        /// </summary>
        private string GetApplicationName(string phrase)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName called with authorisation phrase {_parameterFunction.FormatParameters(new[] { phrase })}.");

            string name = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseModel.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(File.ReadAllText($@"{DatabaseModel.SQLFiles}\Token\GetApplicationName.SQL"), connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Phrase", phrase));

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                name = dataReader.GetString(0);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run TokenService.GetApplicationName.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName returned {name}.");
            return name;
        }
    }
}