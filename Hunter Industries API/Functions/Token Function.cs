using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Services;
using System;
using System.Text;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class TokenFunction
    {
        private readonly LoggerService Logger;
        private readonly TokenService TokenService;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public TokenFunction(TokenService _tokenService, LoggerService _logger)
        {
            TokenService = _tokenService;
            Logger = _logger;
        }

        /// <summary>
        /// Returns the username and password from the decoded header.
        /// </summary>
        public (string, string) ExtractCredentialsFromBasicAuth(string authHeaderValue)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenFunction.ExtractCredentialsFromBasicAuth called with the header value \"{authHeaderValue}\".");

            HashFunction _hashFunction = new HashFunction();

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
                    password = _hashFunction.HashString(credentialsArray[1]);
                }
            }

            catch (Exception ex)
            {
                string message = "Failed to extract the username and password from the basic header.";
                Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenFunction.ExtractCredentialsFromBasicAuth returned {username} | {password}.");
            return (username, password);
        }

        /// <summary>
        /// Returns whether the details passed are valid.
        /// </summary>
        public bool IsValidUser(string usernameInput, string passwordInput, string phraseInput)
        {
            bool valid = false;

            var result = TokenService.GetUsers();
            string[] usernames = result.Item1;
            string[] passwords = result.Item2;
            string[] phrases = TokenService.GetAuthorisationPhrases();

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
    }
}