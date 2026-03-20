// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Claims;

namespace Hunter_Industries_API.Tests.Converters
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
            int expected = 0;

            Claim[] actual = TokenConverter.GetClaims(new List<string>());

            Assert.AreEqual(expected, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetClaims method returns the correct claim when given a single scope.
        /// </summary>
        [TestMethod]
        public void TestGetClaimsSingleScope()
        {
            Claim[] actual = TokenConverter.GetClaims(new List<string> { "User" });

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("scope", actual[0].Type);
            Assert.AreEqual("User", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetClaims method returns the correct claims when given multiple scopes.
        /// </summary>
        [TestMethod]
        public void TestGetClaimsMultipleScopes()
        {
            List<string> expected = new List<string> { "User", "Assistant API", "Server Status API" };

            Claim[] actual = TokenConverter.GetClaims(new List<string> { "User", "Assistant API", "Server Status API" });

            Assert.AreEqual(expected.Count, actual.Length);

            for (int x = 0; x < expected.Count; x++)
            {
                Assert.AreEqual("scope", actual[x].Type);
                Assert.AreEqual(expected[x], actual[x].Value);
            }
        }
    }
}
