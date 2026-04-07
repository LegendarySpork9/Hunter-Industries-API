// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class AuditHistoryConverter
    {
        /// <summary>
        /// Returns the endpoint id number.
        /// </summary>
        public static int GetEndpointID(string endpoint)
        {
            switch (endpoint)
            {
                case "token": return 1;
                case "audithistory": return 2;
                case "assistant/config": return 3;
                case "assistant/version": return 4;
                case "assistant/deletion": return 5;
                case "assistant/location": return 6;
                case "user": return 7;
                case "usersettings": return 8;
                case "serverstatus/serverinformation": return 9;
                case "serverstatus/serverevent": return 10;
                case "serverstatus/serveralert": return 11;
                case "errorlog": return 12;
                case "configuration": return 13;
                case "statistic": return 14;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns the endpoint version id number.
        /// </summary>
        public static int GetEndpointVersionID(string version)
        {
            switch (version)
            {
                case "1.0": return 1;
                case "1.1": return 2;
                case "2.0": return 3;
                default: return 1;
            }
        }

        /// <summary>
        /// Returns the method id number.
        /// </summary>
        public static int GetMethodID(string method)
        {
            switch (method)
            {
                case "GET": return 1;
                case "POST": return 2;
                case "PATCH": return 3;
                case "DELETE": return 4;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns the status id number.
        /// </summary>
        public static int GetStatusID(string status)
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
    }
}