// Copyright © - Unpublished - Toby Hunter
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace HunterIndustriesAPI.Filters
{
    /// <summary>
    /// </summary>
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _Provider;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public DependencyResolver(IServiceProvider _provider)
        {
            _Provider = _provider;
        }

        /// <summary>
        /// Creates a new dependency scope.
        /// </summary>
        public IDependencyScope BeginScope()
        {
            return new DependencyResolver(_Provider.CreateScope().ServiceProvider);
        }

        /// <summary>
        /// Returns the service for the given type.
        /// </summary>
        public object GetService(Type serviceType)
        {
            try
            {
                object service = _Provider.GetService(serviceType);

                if (service != null)
                {
                    return service;
                }

                if (typeof(ApiController).IsAssignableFrom(serviceType))
                {
                    return ActivatorUtilities.CreateInstance(_Provider, serviceType);
                }

                return null;
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all services for the given type.
        /// </summary>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _Provider.GetServices(serviceType);
        }

        /// <summary>
        /// </summary>
        public void Dispose() { }
    }
}
