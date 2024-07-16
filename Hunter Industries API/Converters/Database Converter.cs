// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    public static class DatabaseConverter
    {
        // Formats the given parameters.
        public static string? FormatParameters(string[] parameters)
        {
            string? data;

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

                formattedParameters = formattedParameters.Remove(formattedParameters.LastIndexOf(","), 1);

                data = formattedParameters;
            }

            else
            {
                data = $"\"{parameters}\"";
            }

            return data;
        }
    }
}
