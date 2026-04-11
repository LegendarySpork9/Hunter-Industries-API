// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPICommon.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Claims;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class TokenConverterTest
    {
        /// <summary>
        /// Tests whether the GetClaims method returns an empty array when given an empty list.
        /// </summary>
        [TestMethod]
        public void TestGetClaims()
        {
            int expected = 2;
            Claim[] actual = TokenConverter.GetClaims("TestUser", "TestApp", new List<string>());

            Assert.AreEqual(expected, actual.Length);
            Assert.AreEqual("username", actual[0].Type);
            Assert.AreEqual("TestUser", actual[0].Value);
            Assert.AreEqual("application", actual[1].Type);
            Assert.AreEqual("TestApp", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetClaims method returns the correct claim when given a single scope.
        /// </summary>
        [TestMethod]
        public void TestGetClaimsSingleScope()
        {
            Claim[] actual = TokenConverter.GetClaims("TestUser", "TestApp", new List<string> { "User" });

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("username", actual[0].Type);
            Assert.AreEqual("TestUser", actual[0].Value);
            Assert.AreEqual("application", actual[1].Type);
            Assert.AreEqual("TestApp", actual[1].Value);
            Assert.AreEqual("scope", actual[2].Type);
            Assert.AreEqual("User", actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetClaims method returns the correct claims when given multiple scopes.
        /// </summary>
        [TestMethod]
        public void TestGetClaimsMultipleScopes()
        {
            List<string> expected = new List<string> { "User", "Assistant API", "Server Status API" };
            Claim[] actual = TokenConverter.GetClaims("TestUser", "TestApp", new List<string> { "User", "Assistant API", "Server Status API" });

            Assert.AreEqual(expected.Count + 2, actual.Length);
            Assert.AreEqual("username", actual[0].Type);
            Assert.AreEqual("TestUser", actual[0].Value);
            Assert.AreEqual("application", actual[1].Type);
            Assert.AreEqual("TestApp", actual[1].Value);

            for (int x = 0; x < expected.Count; x++)
            {
                Assert.AreEqual("scope", actual[x + 2].Type);
                Assert.AreEqual(expected[x], actual[x + 2].Value);
            }
        }
    }
}
