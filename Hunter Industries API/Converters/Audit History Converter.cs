// Copyright © - unpublished - Toby Hunter
using HunterIndustriesAPI.Objects;

namespace HunterIndustriesAPI.Converters
{
    public class AuditHistoryConverter
    {
        // Returns the endpoint ID that relates to the given endpoint.
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

        // Returns the method ID that relates to the given method.
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

        // Returns the status ID that relates to the response given.
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

        // Formats the returned data.
        public List<AuditHistoryRecord> FormatData(int[] auditIDs, string[] ipAddresses, string[] endpoints, string[] methods, string[] status, DateTime[] occured, string[] parameters)
        {
            List<AuditHistoryRecord> auditHistory = new();

            for (int x = 0; x < auditIDs.Length; x++)
            {
                AuditHistoryRecord record = new()
                {
                    Id = auditIDs[x],
                    IPAddress = ipAddresses[x],
                    Endpoint = endpoints[x],
                    Method = methods[x],
                    Status = status[x],
                    OccuredAt = occured[x],
                    Paramaters = FormatParameters(parameters[x])
                };

                auditHistory.Add(record);
            }

            return auditHistory;
        }

        // Formats the parameters.
        private string[] FormatParameters(string parameters)
        {
            string[] splitParams = parameters.Split("\",\"");
            string[] formattedParams = Array.Empty<string>();

            foreach (string parameter in splitParams)
            {
                string param = parameter.Replace("\"", "");

                formattedParams = formattedParams.Append(param).ToArray();
            }

            return formattedParams;
        }
    }
}
