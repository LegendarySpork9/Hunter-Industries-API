// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;

namespace HunterIndustriesAPI.Services
{
    public class TokenService
    {
        private readonly string ProgramName;

        // Sets the application name upon initialisation. 
        public TokenService(string phrase)
        {
            ProgramName = GetApplicationName(phrase);
        }

        // Returns the application name.
        public string ApplicationName()
        {
            return ProgramName;
        }

        // Obtains the header data from the request.
        public (string username, string password) ExtractCredentialsFromBasicAuth(string authHeaderValue)
        {
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

            catch (Exception)
            {
                
            }

            return (username, password);
        }

        // Checks if the details are provided are valid.
        public bool IsValidUser(string usernameInput, string passwordInput, string phraseInput)
        {
            bool valid = false;

            // Gets the APIUser table data.
            var result = GetUsers();
            string[] usernames = result.Item1;
            string[] passwords = result.Item2;

            // Gets the Authorisation table data.
            string[] phrases = GetAuthorisationPhrases();

            // Checks the details provided against the accepted details.
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

        // Gets the claims for the token generation.
        public Claim[] GetClaims(string username)
        {
            switch (IsAdmin())
            {
                case true:

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("scope", "Assistant API"),
                        new Claim("scope", "Assistant Control Panel API"),
                        new Claim("scope", "Book Reader API")
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

        // Checks if the user is the admin.
        private bool IsAdmin()
        {
            return ProgramName switch
            {
                "API Admin" => true,
                _ => false
            };
        }

        // Gets the scope for the given phrase.
        private string GetScope()
        {
            return ProgramName switch
            {
                "Virtual Assistant" => "Assistant API",
                _ => string.Empty
            };
        }

        //* SQL *//

        // Gets the users from the database.
        private static (string[], string[]) GetUsers()
        {
            try
            {
                string[] usernames = Array.Empty<string>();
                string[] passwords = Array.Empty<string>();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the users in the API_User table.
                string sqlQuery = "select * from API_User";

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

                return (usernames, passwords);
            }

            catch (Exception ex)
            {
                return (Array.Empty<string>(), Array.Empty<string>());
            }
        }

        // Gets the phrases used for authorisation from the database.
        private static string[] GetAuthorisationPhrases()
        {
            try
            {
                string[] phrases = Array.Empty<string>();

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns all the phrases in the Authorisation table.
                string sqlQuery = "select * from Authorisation";

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

                return phrases;
            }

            catch (Exception ex)
            {
                return Array.Empty<string>();
            }
        }

        // Gets the application names from the database.
        private static string GetApplicationName(string phrase)
        {
            try
            {
                string name = string.Empty;

                // Creates the variables for the SQL queries.
                SqlConnection connection;
                SqlCommand command;
                SqlDataReader dataReader;

                // Obtaines and returns the application name for the given phrase.
                string sqlQuery = $"select Name from Applications where PhraseID = (select PhraseID from Authorisation where phrase = @phrase)";

                connection = new SqlConnection(DatabaseModel.ConnectionString);
                connection.Open();
                command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add(new SqlParameter("@phrase", phrase));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    name = dataReader.GetString(0);
                }

                dataReader.Close();
                connection.Close();

                return name;
            }

            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
