// Copyright © - Unpublished - Toby Hunter
using System.Web.Mvc;
using System.Web.Routing;

namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Setus up the routing.
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
