// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class GitHubIssueConverterTest
    {
        /// <summary>
        /// Tests whether the GetIssueType method returns "Bug" when given "bug".
        /// </summary>
        [TestMethod]
        public void TestGetIssueTypeBug()
        {
            string expected = "Bug";
            string actual = GitHubIssueConverter.GetIssueType("bug");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetIssueType method returns "New Feature" when given "new feature".
        /// </summary>
        [TestMethod]
        public void TestGetIssueTypeNewFeature()
        {
            string expected = "New Feature";
            string actual = GitHubIssueConverter.GetIssueType("new feature");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetIssueType method returns the input unchanged when given an unrecognised type.
        /// </summary>
        [TestMethod]
        public void TestGetIssueTypeDefault()
        {
            string expected = "enhancement";
            string actual = GitHubIssueConverter.GetIssueType("enhancement");

            Assert.AreEqual(
                expected,
                actual);
        }
    }
}
