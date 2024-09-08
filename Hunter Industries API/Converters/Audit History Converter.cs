// Copyright © - unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    public class AuditHistoryConverter
    {
        public int GetEndpointID(string endpoint)
        {
            return endpoint switch
            {
                "token" => 1,
                "audithistory" => 2,
                "assistant/config" => 3,
                "assistant/version" => 4,
                "assistant/deletion" => 5,
                "assistant/location" => 6,
                _ => 0
            };
        }

        public int GetMethodID(string method)
        {
            return method switch
            {
                "GET" => 1,
                "POST" => 2,
                "PATCH" => 3,
                _ => 0
            };
        }

        public int GetStatusID(string status)
        {
            return status switch
            {
                "OK" => 1,
                "Created" => 2,
                "BadRequest" => 3,
                "Unauthorized" => 4,
                "Forbidden" => 5,
                "NotFound" => 6,
                "InternalServerError" => 7,
                _ => 0
            };
        }

        public string[] FormatParameters(string parameters)
        {
            string[] splitParams = parameters.Split("\",\"");
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
