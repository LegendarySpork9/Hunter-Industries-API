// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Filters;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace HunterIndustriesAPI
{
    /// <summary>
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Sets up the application configuration.
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            config.SuppressDefaultHostAuthentication();

            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.MapHttpAttributeRoutes(new VersionedDirectRouteProvider());

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFileSystem, FileSystemWrapper>();
            services.AddSingleton<IDatabaseOptions, DatabaseOptionsProvider>();
            services.AddSingleton<IDatabase, DatabaseWrapper>();
            services.AddSingleton<IClock, SystemClockProvider>();
            services.AddTransient<ILoggerService, LoggerServiceWrapper>();

            ServiceProvider provider = services.BuildServiceProvider();
            config.DependencyResolver = new DependencyResolver(provider);

            DatabaseModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            DatabaseModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];
            ValidationModel.Issuer = ConfigurationManager.AppSettings["Issuer"];
            ValidationModel.Audience = ConfigurationManager.AppSettings["Audience"];
            ValidationModel.SecretKey = ConfigurationManager.AppSettings["SecretKey"];

            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
