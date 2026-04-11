// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.API.Functions
{
    [TestClass]
    public class UserFunctionTest
    {
        /// <summary>
        /// Tests whether the GetScopesUpdateList method returns an empty list when given null required scopes.
        /// </summary>
        [TestMethod]
        public void TestGetScopesUpdateListNull()
        {
            List<KeyValuePair<string, string>> actual = UserFunction.GetScopesUpdateList(new List<string> { "User" }, null);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Tests whether the GetScopesUpdateList method returns an empty list when given matching scopes.
        /// </summary>
        [TestMethod]
        public void TestGetScopesUpdateListMatching()
        {
            List<string> current = new List<string> { "User", "Assistant API" };
            List<string> required = new List<string> { "User", "Assistant API" };
            List<KeyValuePair<string, string>> actual = UserFunction.GetScopesUpdateList(current, required);

            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Tests whether the GetScopesUpdateList method returns an Add entry when given a new scope.
        /// </summary>
        [TestMethod]
        public void TestGetScopesUpdateListAdd()
        {
            List<string> current = new List<string> { "User" };
            List<string> required = new List<string> { "User", "Assistant API" };
            List<KeyValuePair<string, string>> actual = UserFunction.GetScopesUpdateList(current, required);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Add", actual[0].Key);
            Assert.AreEqual("Assistant API", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetScopesUpdateList method returns a Remove entry when given a removed scope.
        /// </summary>
        [TestMethod]
        public void TestGetScopesUpdateListRemove()
        {
            List<string> current = new List<string> { "User", "Assistant API" };
            List<string> required = new List<string> { "User" };
            List<KeyValuePair<string, string>> actual = UserFunction.GetScopesUpdateList(current, required);

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Remove", actual[0].Key);
            Assert.AreEqual("Assistant API", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetScopesUpdateList method returns both Add and Remove entries when given mixed changes.
        /// </summary>
        [TestMethod]
        public void TestGetScopesUpdateListMixed()
        {
            List<string> current = new List<string> { "User", "Assistant API" };
            List<string> required = new List<string> { "User", "Server Status API" };
            List<KeyValuePair<string, string>> actual = UserFunction.GetScopesUpdateList(current, required);

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Add", actual[0].Key);
            Assert.AreEqual("Server Status API", actual[0].Value);
            Assert.AreEqual("Remove", actual[1].Key);
            Assert.AreEqual("Assistant API", actual[1].Value);
        }
    }
}
