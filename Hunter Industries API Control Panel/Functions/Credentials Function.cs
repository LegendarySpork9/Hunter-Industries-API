// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models;
using System.Text;

namespace HunterIndustriesAPIControlPanel.Functions
{
    public static class CredentialsFunction
    {
        /// <summary>
        /// Returns the username used in the authorisation calls.
        /// </summary>
        public static string GetCredentialsUsername(APISettingsModel apiSettings)
        {
            string username = string.Empty;

            const string prefix = "Basic ";
            string? credentials = apiSettings?.Credentials;

            if (!string.IsNullOrWhiteSpace(credentials) && credentials.StartsWith(
                prefix,
                StringComparison.OrdinalIgnoreCase))
            {
                string encoded = credentials[prefix.Length..].Trim();

                try
                {
                    string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    int separatorIndex = decoded.IndexOf(':');

                    if (separatorIndex > 0)
                    {
                        username = decoded[..separatorIndex];
                    }
                }

                catch (FormatException)
                {
                    
                }
            }

            return username;
        }
    }
}
