// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Converters
{
    public static class GraphConverter
    {
        /// <summary>
        /// Returns the badge class for the given method.
        /// </summary>
        public static string GetMethodBadgeClass(string method)
        {
            return method switch
            {
                "GET" => "badge-method-get",
                "POST" => "badge-method-post",
                "PATCH" => "badge-method-patch",
                "DELETE" => "badge-method-delete",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Returns the badge class for the given status.
        /// </summary>
        public static string GetStatusBadgeClass(string status)
        {
            string className = "bg-secondary";

            if (status.StartsWith("200"))
            {
                className = "badge-status-200";
            }

            else if (status.StartsWith("201"))
            {
                className = "badge-status-201";
            }

            else if (status.StartsWith("400"))
            {
                className = "badge-status-400";
            }

            else if (status.StartsWith("401"))
            {
                className = "badge-status-401";
            }

            else if (status.StartsWith("403"))
            {
                className = "badge-status-403";
            }

            else if (status.StartsWith("404"))
            {
                className = "badge-status-404";
            }

            else if (status.StartsWith("500"))
            {
                className = "badge-status-500";
            }

            return className;
        }
    }
}
