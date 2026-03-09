using System;
using System.Web.Http.Routing;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// A route attribute that automatically registers routes for all API versions
    /// from the minimum version up to the current version.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class VersionedRouteAttribute : Attribute, IDirectRouteFactory
    {
        /// <summary>
        /// All known API versions in order. Add new versions to the end of this array.
        /// </summary>
        public static readonly string[] ApiVersions = { "1.0", "2.0" };

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
        /// <param name="path">The route path without the api/vN.N/ prefix.</param>
        /// <param name="minVersion">The minimum API version this route is available from.</param>
        /// <param name="maxVersion">The maximum API version this route is available up to. Defaults to the latest version.</param>
        public VersionedRouteAttribute(string path, string minVersion, string maxVersion = null)
        {
            Path = path;
            MinVersion = minVersion;
            MaxVersion = maxVersion ?? ApiVersions[ApiVersions.Length - 1];
        }

        /// <summary>
        /// </summary>
        RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
        {
            IDirectRouteBuilder builder = context.CreateBuilder($"api/v{MinVersion}/{Path}");
            return builder.Build();
        }
    }
}
