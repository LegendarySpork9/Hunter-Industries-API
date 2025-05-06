using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class ParameterFunction
    {
        /// <summary>
        /// Converts the parameters from the stored SQL format to the output format or converts the model into a string array.
        /// </summary>
        public string[] FormatParameters(string parameters = null, object model = null)
        {
            string[] formattedParameters = Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                string[] splitParams = parameters.Split(new string[] { "\",\"" }, StringSplitOptions.None);

                if (parameters != String.Empty)
                {
                    foreach (string parameter in splitParams)
                    {
                        string param = parameter.Replace("\"", "");

                        formattedParameters = formattedParameters.Append(param).ToArray();
                    }
                }
            }

            if (model != null)
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    if (property.GetValue(model) != null)
                    {
                        if (property.GetValue(model) is IList list)
                        {
                            foreach (object item in list)
                            {
                                formattedParameters = formattedParameters.Append(item.ToString()).ToArray();
                            }
                        }

                        else
                        {
                            formattedParameters = formattedParameters.Append(property.GetValue(model).ToString()).ToArray();
                        }
                    }
                }
            }

            return formattedParameters;
        }

        /// <summary>
        /// Converts the parameters into a log/SQL friendly format.
        /// </summary>
        public string FormatParameters(string[] parameters = null, bool forSQL = false)
        {
            string formattedParameters = null;

            if (parameters != null)
            {
                foreach (string parameter in parameters)
                {
                    if (!string.IsNullOrWhiteSpace(parameter) && !forSQL)
                    {
                        formattedParameters += $"\"{parameter}\", ";
                    }

                    if (!string.IsNullOrWhiteSpace(parameter) && forSQL)
                    {
                        formattedParameters += $"\"{parameter}\",";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(",")).Replace("\"\"", "\"");
            }

            return formattedParameters;
        }

        /// <summary>
        /// Converts the model into a log friendly format.
        /// </summary>
        public string FormatParameters(object model)
        {
            string formattedParameters = string.Empty;

            if (model != null)
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    if (property.GetValue(model) != null)
                    {
                        if (property.GetValue(model) is IList list)
                        {
                            foreach (object item in list)
                            {
                                formattedParameters += $"\"{item}\", ";
                            }
                        }

                        else
                        {
                            formattedParameters += $"\"{property.GetValue(model)}\", ";
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(","));
            }

            return formattedParameters;
        }
    }
}