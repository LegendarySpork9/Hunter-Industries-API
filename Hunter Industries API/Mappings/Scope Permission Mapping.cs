// Copyright © - Unpublished - Toby Hunter
using System.Collections.Generic;
using System.Linq;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// Maps broad database scopes to granular internal permissions.
    /// </summary>
    public static class ScopePermissionMapping
    {
        private static readonly Dictionary<string, List<string>> ScopePermissions = new Dictionary<string, List<string>>()
        {
            {
                "Control Panel API", new List<string>
                {
                    "Assistant.Config",
                    "Assistant.Deletion",
                    "Assistant.Location",
                    "Assistant.Version",
                    "AuditHistory",
                    "Configuration",
                    "ErrorLog",
                    "ServerStatus.Alert",
                    "ServerStatus.Event",
                    "ServerStatus.Information",
                    "User",
                    "UserSettings"

                }
            },
            {
                "Assistant API", new List<string>
                {
                    "Assistant.Config",
                    "Assistant.Deletion",
                    "Assistant.Location",
                    "Assistant.Version"
                }
            },
            {
                "Server Status API", new List<string>
                {
                    "ServerStatus.Alert",
                    "ServerStatus.Event",
                    "ServerStatus.Information.Read",
                    "User.Read",
                    "User.Update",
                    "UserSettings.Read",
                    "UserSettings.Update"
                }
            }
        };

        /// <summary>
        /// Returns all granular permissions granted by the given broad scopes.
        /// </summary>
        public static List<string> GetPermissions(IEnumerable<string> scopes)
        {
            List<string> permissions = new List<string>();

            foreach (string scope in scopes)
            {
                if (ScopePermissions.ContainsKey(scope))
                {
                    permissions.AddRange(ScopePermissions[scope]);
                }
            }

            return permissions.Distinct().ToList();
        }

        /// <summary>
        /// Checks if the given permissions contain any that match the required permission.
        /// </summary>
        public static bool HasPermission(List<string> grantedPermissions, string requiredPermission)
        {
            return grantedPermissions.Any(p => p == requiredPermission || p.StartsWith(requiredPermission + ".") || requiredPermission.StartsWith(p + "."));
        }
    }
}
