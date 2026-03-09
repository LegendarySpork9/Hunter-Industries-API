using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// Expands VersionedRouteAttribute instances into individual version-specific routes.
    /// </summary>
    public class VersionedDirectRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>
        /// </summary>
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return ExpandVersionedFactories(base.GetActionRouteFactories(actionDescriptor));
        }

        /// <summary>
        /// </summary>
        protected override IReadOnlyList<IDirectRouteFactory> GetControllerRouteFactories(HttpControllerDescriptor controllerDescriptor)
        {
            return ExpandVersionedFactories(base.GetControllerRouteFactories(controllerDescriptor));
        }

        private IReadOnlyList<IDirectRouteFactory> ExpandVersionedFactories(IReadOnlyList<IDirectRouteFactory> factories)
        {
            List<IDirectRouteFactory> expanded = new List<IDirectRouteFactory>();

            foreach (IDirectRouteFactory factory in factories)
            {
                if (factory is VersionedRouteAttribute versioned)
                {
                    int minIndex = Array.IndexOf(VersionedRouteAttribute.ApiVersions, versioned.MinVersion);
                    int maxIndex = Array.IndexOf(VersionedRouteAttribute.ApiVersions, versioned.MaxVersion);

                    for (int i = minIndex; i <= maxIndex; i++)
                    {
                        expanded.Add(new VersionSpecificRouteFactory($"api/v{VersionedRouteAttribute.ApiVersions[i]}/{versioned.Path}"));
                    }
                }
                else
                {
                    expanded.Add(factory);
                }
            }

            return expanded;
        }
    }

    /// <summary>
    /// Creates a route entry for a specific versioned route template.
    /// </summary>
    internal class VersionSpecificRouteFactory : IDirectRouteFactory
    {
        private readonly string _template;

        /// <summary>
        /// </summary>
        public VersionSpecificRouteFactory(string template)
        {
            _template = template;
        }

        /// <summary>
        /// </summary>
        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            IDirectRouteBuilder builder = context.CreateBuilder(_template);
            return builder.Build();
        }
    }
}
