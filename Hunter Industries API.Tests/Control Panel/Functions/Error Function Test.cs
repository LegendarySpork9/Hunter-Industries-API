// Copyright (c) 11/06/2026 Toby Hunter
using HunterIndustriesAPIControlPanel.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Functions
{
    [TestClass]
    public class ErrorFunctionTest
    {

        /// <summary>
        /// Tests whether the ExtractClassMethod method returns the class.method notation from a summary.
        /// </summary>
        [TestMethod]
        public void TestExtractClassMethod()
        {
            string expected = "UserService.GetUsers";
            string actual = ErrorFunction.ExtractClassMethod("An error occurred in UserService.GetUsers.");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the ExtractClassMethod method returns a truncated summary when no class.method is found.
        /// </summary>
        [TestMethod]
        public void TestExtractClassMethodFallbackLong()
        {
            string input = "An error occurred while processing the request for data";
            string actual = ErrorFunction.ExtractClassMethod(input);

            Assert.AreEqual(
                $"{input[..40]}...",
                actual);
        }

        /// <summary>
        /// Tests whether the ExtractClassMethod method returns the full summary when no class.method is found and it is short.
        /// </summary>
        [TestMethod]
        public void TestExtractClassMethodFallbackShort()
        {
            string input = "An error occurred";
            string actual = ErrorFunction.ExtractClassMethod(input);

            Assert.AreEqual(
                input,
                actual);
        }

    }
}
