// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    public class DatabaseConverter
    {
        public string? FormatParameters(string[]? parameters)
        {
            string? data = null;

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
