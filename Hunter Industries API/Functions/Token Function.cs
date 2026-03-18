// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using System;
using System.Text;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class TokenFunction
    {
        private readonly ILoggerService _Logger;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public TokenFunction(
            ILoggerService _logger)
        {
            _Logger = _logger;
        }

        /// <summary>
        /// Returns the username and password from the decoded header.
        /// </summary>
        public (string, string) ExtractCredentialsFromBasicAuth(string authHeaderValue)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenFunction.ExtractCredentialsFromBasicAuth called with the header value \"{authHeaderValue}\".");

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
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"TokenFunction.ExtractCredentialsFromBasicAuth returned {username} | {password}.");
            return (username, password);
        }

        /// <summary>
        /// Returns whether the details passed are valid.
        /// </summary>
        public bool IsValidUser(string[] usernames, string[] passwords, string[] phrases, string usernameInput, string passwordInput, string phraseInput)
        {
            bool valid = false;

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