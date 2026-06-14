// Copyright © - 14/06/2026 - Toby Hunter
using HunterIndustriesAPIControlPanel.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Converters
{
    [TestClass]
    public class MediaConverterTest
    {

        /// <summary>
        /// Tests whether the FormatFileSize method returns "0 B" for zero bytes.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeZero()
        {
            string expected = "0 B";
            string actual = MediaConverter.FormatFileSize(0);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FormatFileSize method returns bytes correctly.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeBytes()
        {
            string expected = "500 B";
            string actual = MediaConverter.FormatFileSize(500);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FormatFileSize method returns kilobytes correctly.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeKilobytes()
        {
            string expected = "1 KB";
            string actual = MediaConverter.FormatFileSize(1024);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FormatFileSize method returns fractional kilobytes correctly.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeFractionalKilobytes()
        {
            string expected = "1.5 KB";
            string actual = MediaConverter.FormatFileSize(1536);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FormatFileSize method returns megabytes correctly.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeMegabytes()
        {
            string expected = "1 MB";
            string actual = MediaConverter.FormatFileSize(1048576);

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the FormatFileSize method returns gigabytes correctly.
        /// </summary>
        [TestMethod]
        public void TestFormatFileSizeGigabytes()
        {
            string expected = "1 GB";
            string actual = MediaConverter.FormatFileSize(1073741824);

            Assert.AreEqual(
                expected,
                actual);
        }

    }
}
