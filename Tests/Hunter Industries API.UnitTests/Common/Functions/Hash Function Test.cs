// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.UnitTests.Common.Functions
{
    [TestClass]
    public class HashFunctionTest
    {
        /// <summary>
        /// Tests whether the HashString method returns null when given null.
        /// </summary>
        [TestMethod]
        public void TestHashStringNull()
        {
            string actual = HashFunction.HashString(null);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the HashString method returns null when given an empty string.
        /// </summary>
        [TestMethod]
        public void TestHashStringEmpty()
        {
            string actual = HashFunction.HashString("");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the HashString method returns null when given whitespace.
        /// </summary>
        [TestMethod]
        public void TestHashStringWhitespace()
        {
            string actual = HashFunction.HashString("   ");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the HashString method returns a 128 character string when given a valid value.
        /// </summary>
        [TestMethod]
        public void TestHashStringLength()
        {
            int expected = 128;
            string actual = HashFunction.HashString("password");

            Assert.AreEqual(
                expected,
                actual.Length);
        }

        /// <summary>
        /// Tests whether the HashString method returns the same hash when given the same value.
        /// </summary>
        [TestMethod]
        public void TestHashStringConsistent()
        {
            string first = HashFunction.HashString("password");
            string second = HashFunction.HashString("password");

            Assert.AreEqual(
                first,
                second);
        }

        /// <summary>
        /// Tests whether the HashString method returns different hashes when given different values.
        /// </summary>
        [TestMethod]
        public void TestHashStringDifferent()
        {
            string first = HashFunction.HashString("password");
            string second = HashFunction.HashString("different");

            Assert.AreNotEqual(
                first,
                second);
        }

        /// <summary>
        /// Tests whether the HashString method returns a different hash when given a salt.
        /// </summary>
        [TestMethod]
        public void TestHashStringWithSalt()
        {
            string unsalted = HashFunction.HashString("password");
            string salted = HashFunction.HashString("password", "somesalt");

            Assert.AreNotEqual(
                unsalted,
                salted);
        }

        /// <summary>
        /// Tests whether the HashString method returns the same hash when given the same value and salt.
        /// </summary>
        [TestMethod]
        public void TestHashStringWithSaltConsistent()
        {
            string first = HashFunction.HashString("password", "somesalt");
            string second = HashFunction.HashString("password", "somesalt");

            Assert.AreEqual(
                first,
                second);
        }

        /// <summary>
        /// Tests whether the HashString method returns different hashes when given different salts.
        /// </summary>
        [TestMethod]
        public void TestHashStringWithDifferentSalts()
        {
            string first = HashFunction.HashString("password", "salt1");
            string second = HashFunction.HashString("password", "salt2");

            Assert.AreNotEqual(
                first,
                second);
        }

        /// <summary>
        /// Tests whether the HashString method returns the same hash when given a null salt as when given no salt.
        /// </summary>
        [TestMethod]
        public void TestHashStringNullSaltMatchesNoSalt()
        {
            string withoutSalt = HashFunction.HashString("password");
            string withNullSalt = HashFunction.HashString("password", null);

            Assert.AreEqual(
                withoutSalt,
                withNullSalt);
        }

        /// <summary>
        /// Tests whether the GenerateSalt method returns a non-null non-empty string.
        /// </summary>
        [TestMethod]
        public void TestGenerateSalt()
        {
            string salt = HashFunction.GenerateSalt();

            Assert.IsFalse(string.IsNullOrWhiteSpace(salt));
        }

        /// <summary>
        /// Tests whether the GenerateSalt method returns a different value each time.
        /// </summary>
        [TestMethod]
        public void TestGenerateSaltUnique()
        {
            string first = HashFunction.GenerateSalt();
            string second = HashFunction.GenerateSalt();

            Assert.AreNotEqual(
                first,
                second);
        }
    }
}
