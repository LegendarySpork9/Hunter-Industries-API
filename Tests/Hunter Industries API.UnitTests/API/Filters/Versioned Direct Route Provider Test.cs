// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Http;

namespace HunterIndustriesAPI.UnitTests.API.Filters
{
    [TestClass]
    public class VersionedDirectRouteProviderTest
    {
        /// <summary>
        /// Checks whether the provider can be instantiated.
        /// </summary>
        [TestMethod]
        public void TestProviderCanBeCreated()
        {
            VersionedDirectRouteProvider provider = new();

            Assert.IsNotNull(provider);
        }

        /// <summary>
        /// Checks whether the provider expands versioned routes when registered with an HttpConfiguration.
        /// </summary>
        [TestMethod]
        public void TestProviderExpandsRoutes()
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes(new VersionedDirectRouteProvider());

            config.EnsureInitialized();

            int routeCount = config.Routes.Count;

            Assert.IsTrue(
                routeCount >= 0);
        }

        /// <summary>
        /// Checks whether a VersionedRouteAttribute spanning two versions produces the correct min and max.
        /// </summary>
        [TestMethod]
        public void TestVersionedRouteSpan()
        {
            VersionedRouteAttribute attribute = new(
                "user",
                "1.0",
                "1.1");

            int minIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MinVersion);
            int maxIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MaxVersion);

            int expectedVersionCount = maxIndex - minIndex + 1;

            Assert.AreEqual(
                2,
                expectedVersionCount);
        }

        /// <summary>
        /// Checks whether a VersionedRouteAttribute spanning all versions produces the correct count.
        /// </summary>
        [TestMethod]
        public void TestVersionedRouteSpanAll()
        {
            VersionedRouteAttribute attribute = new(
                "errorlog",
                "1.0");

            int minIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MinVersion);
            int maxIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MaxVersion);

            int expectedVersionCount = maxIndex - minIndex + 1;

            Assert.AreEqual(
                VersionedRouteAttribute.ApiVersions.Length,
                expectedVersionCount);
        }

        /// <summary>
        /// Checks whether a VersionedRouteAttribute with a single version produces one route.
        /// </summary>
        [TestMethod]
        public void TestVersionedRouteSingleVersion()
        {
            VersionedRouteAttribute attribute = new(
                "portfolio",
                "2.2",
                "2.2");

            int minIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MinVersion);
            int maxIndex = System.Array.IndexOf(
                VersionedRouteAttribute.ApiVersions,
                attribute.MaxVersion);

            int expectedVersionCount = maxIndex - minIndex + 1;

            Assert.AreEqual(
                1,
                expectedVersionCount);
        }
    }
}
