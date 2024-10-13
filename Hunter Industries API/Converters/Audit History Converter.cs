using System;
using System.Linq;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public class AuditHistoryConverter
    {
        /// <summary>
        /// Returns the endpoint id number.
        /// </summary>
        public int GetEndpointID(string endpoint)
        {
            switch (endpoint)
            {
                case "token": return 1;
                case "audithistory": return 2;
                case "assistant/config": return 3;
                case "assistant/version": return 4;
                case "assistant/deletion": return 5;
                case "assistant/location": return 6;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns the method id number.
        /// </summary>
        public int GetMethodID(string method)
        {
            switch (method)
            {
                case "GET": return 1;
                case "POST": return 2;
                case "PATCH": return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns the status id number.
        /// </summary>
        public int GetStatusID(string status)
        {
            switch (status)
            {
                case "OK": return 1;
                case "Created": return 2;
                case "BadRequest": return 3;
                case "Unauthorized": return 4;
                case "Forbidden": return 5;
                case "NotFound": return 6;
                case "InternalServerError": return 7;
                default: return 0;
            }
        }

        /// <summary>
        /// Converts the parameters from the stored SQL format to the output format.
        /// </summary>
        public string[] FormatParameters(string parameters)
        {
            string[] splitParams = parameters.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            string[] formattedParams = Array.Empty<string>();

            if (parameters != String.Empty)
            {
                foreach (string parameter in splitParams)
                {
                    string param = parameter.Replace("\"", "");

                    formattedParams = formattedParams.Append(param).ToArray();
                }
            }

            return formattedParams;
        }
    }
}