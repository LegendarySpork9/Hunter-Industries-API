using System;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public class DatabaseConverter
    {
        /// <summary>
        /// Converts parameters from the input format to the stored SQL format.
        /// </summary>
        public string FormatParameters(string[] parameters = null)
        {
            string data = null;

            if (parameters != null)
            {
                if (parameters.Length > 1)
                {
                    string formattedParameters = string.Empty;

                    for (int x = 0; x < parameters.Length; x++)
                    {
                        if (!String.IsNullOrEmpty(parameters[x]))
                        {
                            formattedParameters += $"\"{parameters[x]}\",";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(formattedParameters))
                    {
                        formattedParameters = formattedParameters.Remove(formattedParameters.LastIndexOf(","), 1);
                    }

                    data = formattedParameters;
                }

                else
                {
                    data = $"\"{parameters}\"";
                }
            }

            return data;
        }
    }
}