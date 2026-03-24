// Copyright © - Unpublished - Toby Hunter
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class ParameterFunction
    {
        /// <summary>
        /// Converts the parameters from the stored SQL format to the output format or converts the model into a string array.
        /// </summary>
        public static string[] FormatParameters(string parameters = null, object model = null, bool hashString = false)
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

                        else if (property.Name == "Password" && hashString)
                        {
                            formattedParameters = formattedParameters.Append(HashFunction.HashString(property.GetValue(model).ToString())).ToArray();
                        }

                        else
                        {
                            formattedParameters = formattedParameters.Append(property.GetValue(model).ToString()).ToArray();
                        }
                    }

                    else
                    {
                        formattedParameters = formattedParameters.Append("").ToArray();
                    }
                }
            }

            return formattedParameters;
        }

        /// <summary>
        /// Converts the parameters into a log/SQL friendly format.
        /// </summary>
        public static string FormatParameters(string[] parameters = null, bool forSQL = false)
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

                    if (string.IsNullOrWhiteSpace(parameter))
                    {
                        if (forSQL)
                        {
                            formattedParameters += "\"null\",";
                        }

                        else
                        {
                            formattedParameters += "\"null\", ";
                        }
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
        public static string FormatParameters(object model, bool hashString = false)
        {
            string formattedParameters = string.Empty;

            if (model != null)
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object value = property.GetValue(model);

                    if (value != null)
                    {
                        if (value is IList list)
                        {
                            foreach (object item in list)
                            {
                                formattedParameters += $"\"{item}\", ";
                            }
                        }

                        else if (property.Name == "Password" && hashString)
                        {
                            formattedParameters += $"\"{HashFunction.HashString(value.ToString())}\", ";
                        }

                        else
                        {
                            formattedParameters += $"\"{value}\", ";
                        }
                    }

                    else
                    {
                        formattedParameters += "\"null\", ";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(","));
            }

            return formattedParameters;
        }

        /// <summary>
        /// Converts the list into a log friendly format.
        /// </summary>
        public static string FormatListParameters(object listObject, bool isKeyPair)
        {
            string formattedParameters = string.Empty;

            if (listObject != null)
            {
                if (isKeyPair)
                {
                    if (listObject is IList<KeyValuePair<string, string>> list)
                    {
                        foreach (KeyValuePair<string, string> item in list)
                        {
                            formattedParameters += $"\"{item.Value}\", ";
                        }
                    }
                }

                else
                {
                    if (listObject is IList list)
                    {
                        foreach (object item in list)
                        {
                            formattedParameters += $"\"{item}\", ";
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

        /// <summary>
        /// Converts the list into a SQL friendly format.
        /// </summary>
        public static string FormatParameters(List<object> list, bool forAudit = false)
        {
            string formattedParameters = null;

            if (list != null)
            {
                foreach (object item in list)
                {
                    if (forAudit)
                    {
                        formattedParameters += $"\"{item}\",";
                    }

                    else
                    {
                        formattedParameters += $"{item}, ";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(",")).Replace("\"\"", "\"");
            }

            return formattedParameters;
        }
    }
}