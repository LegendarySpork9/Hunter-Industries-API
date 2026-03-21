// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HunterIndustriesAPI.Tests.Functions
{
    [TestClass]
    public class TokenFunctionTest
    {
        #region ExtractCredentialsFromBasicAuth

        /// <summary>
        /// Tests whether the ExtractCredentialsFromBasicAuth method returns empty strings when given an invalid value.
        /// </summary>
        [TestMethod]
        public void TestExtractCredentialsFromBasicAuth()
        {
            (string username, string password) = TokenFunction.ExtractCredentialsFromBasicAuth("InvalidValue");

            Assert.AreEqual(string.Empty, username);
            Assert.AreEqual(string.Empty, password);
        }

        /// <summary>
        /// Tests whether the ExtractCredentialsFromBasicAuth method returns the correct username when given a valid header.
        /// </summary>
        [TestMethod]
        public void TestExtractCredentialsFromBasicAuthUsername()
        {
            string expected = "testuser";
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("testuser:testpass"));
            (string username, string _) = TokenFunction.ExtractCredentialsFromBasicAuth(header);

            Assert.AreEqual(expected, username);
        }

        /// <summary>
        /// Tests whether the ExtractCredentialsFromBasicAuth method returns a hashed password when given a valid header.
        /// </summary>
        [TestMethod]
        public void TestExtractCredentialsFromBasicAuthPassword()
        {
            string expected = HashFunction.HashString("testpass");
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("testuser:testpass"));
            (string _, string password) = TokenFunction.ExtractCredentialsFromBasicAuth(header);

            Assert.AreEqual(expected, password);
        }

        /// <summary>
        /// Tests whether the ExtractCredentialsFromBasicAuth method returns empty strings when given a header without a colon.
        /// </summary>
        [TestMethod]
        public void TestExtractCredentialsFromBasicAuthNoColon()
        {
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("nocolonhere"));
            (string username, string password) = TokenFunction.ExtractCredentialsFromBasicAuth(header);

            Assert.AreEqual(string.Empty, username);
            Assert.AreEqual(string.Empty, password);
        }

        #endregion

        #region IsValidUser

        /// <summary>
        /// Tests whether the IsValidUser method returns false when given invalid credentials.
        /// </summary>
        [TestMethod]
        public void TestIsValidUser()
        {
            string[] usernames = new string[] { "admin" };
            string[] passwords = new string[] { "pass" };
            string[] phrases = new string[] { "phrase" };
            bool actual = TokenFunction.IsValidUser(usernames, passwords, phrases, "wrong", "wrong", "wrong");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the IsValidUser method returns false when given a valid username but invalid password.
        /// </summary>
        [TestMethod]
        public void TestIsValidUserWrongPassword()
        {
            string[] usernames = new string[] { "admin" };
            string[] passwords = new string[] { "pass" };
            string[] phrases = new string[] { "phrase" };
            bool actual = TokenFunction.IsValidUser(usernames, passwords, phrases, "admin", "wrong", "phrase");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the IsValidUser method returns false when given valid credentials but an invalid phrase.
        /// </summary>
        [TestMethod]
        public void TestIsValidUserWrongPhrase()
        {
            string[] usernames = new string[] { "admin" };
            string[] passwords = new string[] { "pass" };
            string[] phrases = new string[] { "phrase" };
            bool actual = TokenFunction.IsValidUser(usernames, passwords, phrases, "admin", "pass", "wrong");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the IsValidUser method returns true when given valid credentials.
        /// </summary>
        [TestMethod]
        public void TestIsValidUserValid()
        {
            string[] usernames = new string[] { "admin" };
            string[] passwords = new string[] { "pass" };
            string[] phrases = new string[] { "phrase" };
            bool actual = TokenFunction.IsValidUser(usernames, passwords, phrases, "admin", "pass", "phrase");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the IsValidUser method returns true when given valid credentials from multiple entries.
        /// </summary>
        [TestMethod]
        public void TestIsValidUserMultipleEntries()
        {
            string[] usernames = new string[] { "admin", "user" };
            string[] passwords = new string[] { "pass1", "pass2" };
            string[] phrases = new string[] { "phrase1", "phrase2" };
            bool actual = TokenFunction.IsValidUser(usernames, passwords, phrases, "user", "pass2", "phrase2");

            Assert.IsTrue(actual);
        }

        #endregion
    }
}
