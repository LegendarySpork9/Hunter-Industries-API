// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Functions;
using HunterIndustriesAPIControlPanel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace HunterIndustriesAPI.Tests.ControlPanel.Functions
{
    [TestClass]
    public class CredentialsFunctionTest
    {
        #region GetCredentialsUsername

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns the username from valid credentials.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsername()
        {
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("TestUser:TestPassword"));
            APISettingsModel settings = new() { Credentials = $"Basic {encoded}" };

            string actual = CredentialsFunction.GetCredentialsUsername(settings);

            Assert.AreEqual(
                "TestUser",
                actual);
        }

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns an empty string when credentials are null.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsernameNull()
        {
            APISettingsModel settings = new() { Credentials = null };

            string actual = CredentialsFunction.GetCredentialsUsername(settings);

            Assert.AreEqual(
                string.Empty,
                actual);
        }

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns an empty string when settings are null.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsernameNullSettings()
        {
            string actual = CredentialsFunction.GetCredentialsUsername(null);

            Assert.AreEqual(
                string.Empty,
                actual);
        }

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns an empty string for invalid Base64.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsernameInvalidBase64()
        {
            APISettingsModel settings = new() { Credentials = "Basic !!!invalid!!!" };

            string actual = CredentialsFunction.GetCredentialsUsername(settings);

            Assert.AreEqual(
                string.Empty,
                actual);
        }

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns an empty string when no colon separator exists.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsernameNoColon()
        {
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("TestUserNoPassword"));
            APISettingsModel settings = new() { Credentials = $"Basic {encoded}" };

            string actual = CredentialsFunction.GetCredentialsUsername(settings);

            Assert.AreEqual(
                string.Empty,
                actual);
        }

        /// <summary>
        /// Tests whether the GetCredentialsUsername method returns an empty string when the prefix is missing.
        /// </summary>
        [TestMethod]
        public void TestGetCredentialsUsernameNoPrefix()
        {
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("TestUser:TestPassword"));
            APISettingsModel settings = new() { Credentials = encoded };

            string actual = CredentialsFunction.GetCredentialsUsername(settings);

            Assert.AreEqual(
                string.Empty,
                actual);
        }

        #endregion
    }
}
