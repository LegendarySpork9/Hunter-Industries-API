using System.Collections.Generic;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class UserFunction
    {
        /// <summary>
        /// Compares the scopes and generates the update list.
        /// </summary>
        public List<KeyValuePair<string, string>> GetScopesUpdateList(List<string> currentScopes, List<string> requiredScopes)
        {
            List<KeyValuePair<string, string>> updateScopes = new List<KeyValuePair<string, string>>();

            if (requiredScopes != null)
            {
                foreach (string scope in requiredScopes)
                {
                    if (!currentScopes.Contains(scope))
                    {
                        updateScopes.Add(new KeyValuePair<string, string>("Add", scope));
                    }
                }

                foreach (string scope in currentScopes)
                {
                    if (!requiredScopes.Contains(scope))
                    {
                        updateScopes.Add(new KeyValuePair<string, string>("Remove", scope));
                    }
                }
            }

            return updateScopes;
        }
    }
}