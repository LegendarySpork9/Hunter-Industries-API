// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Functions;
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
            string rawPassword = string.Empty;

            try
            {
                string encodedCredentials = authHeaderValue.Replace("Basic ", string.Empty);
                string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                string[] credentialsArray = decodedCredentials.Split(':');

                if (credentialsArray.Length == 2)
                {
                    username = credentialsArray[0];
                    rawPassword = credentialsArray[1];
                }
            }

            catch (Exception)
            {
            }

            return (username, rawPassword);
        }

        /// <summary>
        /// Returns the salt for the given username.
        /// </summary>
        public static string FindUserSalt(
            string[] usernames,
            string[] salts,
            string usernameInput)
        {
            for (int i = 0; i < usernames.Length && i < salts.Length; i++)
            {
                if (usernames[i] == usernameInput)
                {
                    return salts[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns whether the details passed are valid.
        /// </summary>
        public static bool IsValidUser(
            string[] usernames,
            string[] passwords,
            string[] phrases,
            string usernameInput,
            string passwordInput,
            string phraseInput)
        {
            bool credentialsValid = false;

            for (int i = 0; i < usernames.Length && i < passwords.Length; i++)
            {
                if (usernames[i] == usernameInput && passwords[i] == passwordInput)
                {
                    credentialsValid = true;
                    break;
                }
            }

            if (!credentialsValid)
            {
                return false;
            }

            foreach (string phrase in phrases)
            {
                if (phrase == phraseInput)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
