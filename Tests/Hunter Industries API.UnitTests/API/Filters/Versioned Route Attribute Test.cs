// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.UnitTests.API.Filters
{
    [TestClass]
    public class VersionedRouteAttributeTest
    {
        /// <summary>
        /// Checks whether the constructor sets the Path property.
        /// </summary>
        [TestMethod]
        public void TestConstructorSetsPath()
        {
            VersionedRouteAttribute attribute = new(
                "user",
                "1.0");

            Assert.AreEqual(
                "user",
                attribute.Path);
        }

        /// <summary>
        /// Checks whether the constructor sets the MinVersion property.
        /// </summary>
        [TestMethod]
        public void TestConstructorSetsMinVersion()
        {
            VersionedRouteAttribute attribute = new(
                "user",
                "1.0");

            Assert.AreEqual(
                "1.0",
                attribute.MinVersion);
        }

        /// <summary>
        /// Checks whether the constructor defaults MaxVersion to the latest API version when not specified.
        /// </summary>
        [TestMethod]
        public void TestConstructorDefaultsMaxVersion()
        {
            VersionedRouteAttribute attribute = new(
                "user",
                "1.0");

            string latestVersion = VersionedRouteAttribute.ApiVersions[VersionedRouteAttribute.ApiVersions.Length - 1];

            Assert.AreEqual(
                latestVersion,
                attribute.MaxVersion);
        }

        /// <summary>
        /// Checks whether the constructor sets an explicit MaxVersion when specified.
        /// </summary>
        [TestMethod]
        public void TestConstructorExplicitMaxVersion()
        {
            VersionedRouteAttribute attribute = new(
                "user",
                "1.0",
                "1.1");

            Assert.AreEqual(
                "1.1",
                attribute.MaxVersion);
        }

        /// <summary>
        /// Checks whether the ApiVersions array contains the expected versions in order.
        /// </summary>
        [TestMethod]
        public void TestApiVersionsArray()
        {
            Assert.AreEqual(
                5,
                VersionedRouteAttribute.ApiVersions.Length);
            Assert.AreEqual(
                "1.0",
                VersionedRouteAttribute.ApiVersions[0]);
            Assert.AreEqual(
                "2.2",
                VersionedRouteAttribute.ApiVersions[4]);
        }
    }
}
