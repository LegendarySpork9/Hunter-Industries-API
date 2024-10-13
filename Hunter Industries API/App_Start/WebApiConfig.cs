using HunterIndustriesAPI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultAPI",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                });

            DatabaseModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            ValidationModel.Issuer = ConfigurationManager.AppSettings["Issuer"];
            ValidationModel.Audience = ConfigurationManager.AppSettings["Audience"];
            ValidationModel.SecretKey = ConfigurationManager.AppSettings["SecretKey"];

            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
