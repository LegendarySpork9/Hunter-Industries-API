// Copyright © 11/06/2026 Toby Hunter
using System.Web.Mvc;

namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Sets up the global filters.
        /// </summary>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
