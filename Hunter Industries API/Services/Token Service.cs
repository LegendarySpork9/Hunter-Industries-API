using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;

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
        /// Returns the username and password from the decoded header.
        /// </summary>
        public (string, string) ExtractCredentialsFromBasicAuth(string authHeaderValue)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.ExtractCredentialsFromBasicAuth called with the header value \"{authHeaderValue}\".");

            string username = string.Empty;
            string password = string.Empty;

            try
            {
                var encodedCredentials = authHeaderValue.Replace("Basic ", string.Empty);
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var credentialsArray = decodedCredentials.Split(':');

                if (credentialsArray.Length == 2)
                {
                    username = credentialsArray[0];
                    password = credentialsArray[1];
                }
            }

            catch (Exception ex)
            {
                string message = "Failed to extract the username and password from the basic header.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.ExtractCredentialsFromBasicAuth returned {username} | {password}.");
            return (username, password);
        }

        /// <summary>
        /// Returns whether the details passed are valid.
        /// </summary>
        public bool IsValidUser(string usernameInput, string passwordInput, string phraseInput)
        {
            bool valid = false;

            var result = GetUsers();
            string[] usernames = result.Item1;
            string[] passwords = result.Item2;
            string[] phrases = GetAuthorisationPhrases();

            foreach (string username in usernames)
            {
                if (username == usernameInput)
                {
                    foreach (string password in passwords)
                    {
                        if (password == passwordInput)
                        {
                            foreach (string phrase in phrases)
                            {
                                if (phrase == phraseInput)
                                {
                                    valid = true;
                                }
                            }
                        }
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// Converts the scope into a claim.
        /// </summary>
        public Claim[] GetClaims(string username)
        {
            switch (IsAdmin())
            {
                case true:

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("scope", "Assistant API"),
                        new Claim("scope", "Book Reader API"),
                        new Claim("scope", "Control Panel API")
                    };

                    return claims;

                default:

                    claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("scope", GetScope())
                    };

                    return claims;
            }
        }

        /// <summary>
        /// Returns whether the user is an admin.
        /// </summary>
        private bool IsAdmin()
        {
            switch (ProgramName)
            {
                case "API Admin": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns the scope based on the application name.
        /// </summary>
        private string GetScope()
        {
            switch (ProgramName)
            {
                case "Virtual Assistant": return "Assistant API";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Returns all username and passwords.
        /// </summary>
        private (string[], string[]) GetUsers()
        {
            string[] usernames = Array.Empty<string>();
            string[] passwords = Array.Empty<string>();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = "select * from APIUser with (nolock)";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    usernames = usernames.Append(dataReader.GetString(1)).ToArray();
                    passwords = passwords.Append(dataReader.GetString(2)).ToArray();
                }

                dataReader.Close();
                connection.Close();
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
        private string[] GetAuthorisationPhrases()
        {
            string[] phrases = Array.Empty<string>();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = "select * from Authorisation with (nolock)";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    phrases = phrases.Append(dataReader.GetString(1)).ToArray();
                }

                dataReader.Close();
                connection.Close();
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
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName called with authorisation phrase \"{phrase}\".");

            string name = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = @"select Name from Application with (nolock)
join Authorisation with (nolock) on Application.PhraseID = Authorisation.PhraseID
where Phrase = @Phrase";

            try
            {
                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@Phrase", phrase));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    name = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();
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