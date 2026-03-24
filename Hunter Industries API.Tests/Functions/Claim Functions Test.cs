// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace HunterIndustriesAPI.Tests.Functions
{
    [TestClass]
    public class ClaimFunctionsTest
    {
        #region GetUsername

        /// <summary>
        /// Tests whether the GetUsername method returns the username from a principal with a Name claim.
        /// </summary>
        [TestMethod]
        public void TestGetUsername()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim("username", "TestUser") }, "TestAuth");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            string expected = "TestUser";
            string actual = ClaimFunctions.GetUsername(principal);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetUsername method returns null when the principal is null.
        /// </summary>
        [TestMethod]
        public void TestGetUsernameNullPrincipal()
        {
            string actual = ClaimFunctions.GetUsername(null);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetUsername method returns null when the principal has no Name claim.
        /// </summary>
        [TestMethod]
        public void TestGetUsernameNoClaim()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim("scope", "User") }, "TestAuth");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            string actual = ClaimFunctions.GetUsername(principal);

            Assert.IsNull(actual);
        }

        #endregion

        #region GetApplicationName

        /// <summary>
        /// Tests whether the GetApplicationName method returns the application name from a principal with an application claim.
        /// </summary>
        [TestMethod]
        public void TestGetApplicationName()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim("application", "TestApp") }, "TestAuth");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            string expected = "TestApp";
            string actual = ClaimFunctions.GetApplicationName(principal);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetApplicationName method returns null when the principal is null.
        /// </summary>
        [TestMethod]
        public void TestGetApplicationNameNullPrincipal()
        {
            string actual = ClaimFunctions.GetApplicationName(null);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetApplicationName method returns null when the principal has no application claim.
        /// </summary>
        [TestMethod]
        public void TestGetApplicationNameNoClaim()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim("scope", "User") }, "TestAuth");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            string actual = ClaimFunctions.GetApplicationName(principal);

            Assert.IsNull(actual);
        }

        #endregion
    }
}
