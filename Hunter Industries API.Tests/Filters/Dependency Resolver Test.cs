// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace HunterIndustriesAPI.Tests.Filters
{
    [TestClass]
    public class DependencyResolverTest
    {
        #region GetService

        /// <summary>
        /// Tests whether the GetService method returns null when given an unregistered type.
        /// </summary>
        [TestMethod]
        public void TestGetService()
        {
            ServiceCollection services = new ServiceCollection();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            object actual = resolver.GetService(typeof(IDisposable));

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetService method returns the correct service when given a registered type.
        /// </summary>
        [TestMethod]
        public void TestGetServiceRegistered()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFormatProvider, TestFormatProvider>();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            object actual = resolver.GetService(typeof(IFormatProvider));

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(TestFormatProvider));
        }

        #endregion

        #region BeginScope

        /// <summary>
        /// Tests whether the BeginScope method returns a new dependency scope.
        /// </summary>
        [TestMethod]
        public void TestBeginScope()
        {
            ServiceCollection services = new ServiceCollection();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            IDependencyScope actual = resolver.BeginScope();

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the BeginScope method returns a scope that can resolve a registered service.
        /// </summary>
        [TestMethod]
        public void TestBeginScopeResolvesService()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFormatProvider, TestFormatProvider>();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            IDependencyScope scope = resolver.BeginScope();
            object actual = scope.GetService(typeof(IFormatProvider));

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(TestFormatProvider));
        }

        #endregion

        #region GetServices

        /// <summary>
        /// Tests whether the GetServices method returns an empty collection when given an unregistered type.
        /// </summary>
        [TestMethod]
        public void TestGetServices()
        {
            ServiceCollection services = new ServiceCollection();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            IEnumerable<object> actual = resolver.GetServices(typeof(IDisposable));

            Assert.AreEqual(0, actual.Count());
        }

        /// <summary>
        /// Tests whether the GetServices method returns all registered services when given a registered type.
        /// </summary>
        [TestMethod]
        public void TestGetServicesRegistered()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFormatProvider, TestFormatProvider>();
            services.AddSingleton<IFormatProvider, TestFormatProviderTwo>();
            DependencyResolver resolver = new DependencyResolver(services.BuildServiceProvider());

            IEnumerable<object> actual = resolver.GetServices(typeof(IFormatProvider));

            Assert.AreEqual(2, actual.Count());
        }

        #endregion

        /// <summary>
        /// A test implementation of IFormatProvider.
        /// </summary>
        private class TestFormatProvider : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return null;
            }
        }

        /// <summary>
        /// A second test implementation of IFormatProvider.
        /// </summary>
        private class TestFormatProviderTwo : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return null;
            }
        }
    }
}
