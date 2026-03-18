// Copyright © - Unpublished - Toby Hunter
using System;
using System.Web.Http.Routing;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class VersionedRouteAttribute : Attribute, IDirectRouteFactory
    {
        /// <summary>
        /// All known API versions in order.
        /// </summary>
        public static readonly string[] ApiVersions = { "1.0", "1.1" };

        /// <summary>
        /// The route path without the api/vN.N/ prefix.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The minimum API version this route is available from.
        /// </summary>
        public string MinVersion { get; }

        /// <summary>
        /// The maximum API version this route is available up to.
        /// </summary>
        public string MaxVersion { get; }

        /// <summary>
        /// </summary>
        public VersionedRouteAttribute(string path, string minVersion, string maxVersion = null)
        {
            Path = path;
            MinVersion = minVersion;
            MaxVersion = maxVersion ?? ApiVersions[ApiVersions.Length - 1];
        }

        /// <summary>
        /// Creates the route for the given context.
        /// </summary>
        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            IDirectRouteBuilder builder = context.CreateBuilder($"api/v{MinVersion}/{Path}");
            return builder.Build();
        }
    }
}
