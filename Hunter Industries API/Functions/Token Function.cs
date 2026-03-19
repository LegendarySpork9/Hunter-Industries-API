// Copyright © - Unpublished - Toby Hunter
using System;
using System.Text;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class TokenFunction
    {
        /// <summary>
        /// Returns the username and password from the decoded header.
        /// </summary>
        public static (string, string) ExtractCredentialsFromBasicAuth(string authHeaderValue)
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
                    password = HashFunction.HashString(credentialsArray[1]);
                }
            }

            catch (Exception)
            {
            }

            return (username, password);
        }

        /// <summary>
        /// Returns whether the details passed are valid.
        /// </summary>
        public static bool IsValidUser(string[] usernames, string[] passwords, string[] phrases, string usernameInput, string passwordInput, string phraseInput)
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
