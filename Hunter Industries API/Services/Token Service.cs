// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;

namespace HunterIndustriesAPI.Services
{
    public class TokenService
    {
        private readonly string ProgramName;
        private readonly LoggerService Logger;

        public TokenService(string phrase, LoggerService _logger)
        {
            Logger = _logger;
            ProgramName = GetApplicationName(phrase);
        }

        public string ApplicationName()
        {
            return ProgramName;
        }

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

        private bool IsAdmin()
        {
            return ProgramName switch
            {
                "API Admin" => true,
                _ => false
            };
        }

        private string GetScope()
        {
            return ProgramName switch
            {
                "Virtual Assistant" => "Assistant API",
                _ => string.Empty
            };
        }

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

        private string GetApplicationName(string phrase)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenService.GetApplicationName called with authorisation phrase \"{phrase}\".");

            string name = string.Empty;

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sqlQuery = @"select Name from Application with (nolock)
join Authorisation with (nolock) on Applications.PhraseID = Authorisation.PhraseID
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
