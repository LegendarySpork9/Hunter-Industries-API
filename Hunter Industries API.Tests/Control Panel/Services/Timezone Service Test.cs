// Copyright (c) - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HunterIndustriesAPI.Tests.ControlPanel.Services
{
    [TestClass]
    public class TimezoneServiceTest
    {
        #region GetOffsetLabel

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns UTC+0 by default.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelDefault()
        {
            TimezoneService service = new();

            Assert.AreEqual(
                "UTC+0",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns UTC+0 when conversion is disabled.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelDisabled()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = false,
                OffsetHours = 2
            };

            Assert.AreEqual(
                "UTC+0",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns a positive whole hour offset.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelPositiveWholeHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 1
            };

            Assert.AreEqual(
                "UTC+1",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns a negative whole hour offset.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelNegativeWholeHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = -5
            };

            Assert.AreEqual(
                "UTC-5",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns a positive half hour offset.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelHalfHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 5.5
            };

            Assert.AreEqual(
                "UTC+5:30",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns a negative half hour offset.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelNegativeHalfHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = -9.5
            };

            Assert.AreEqual(
                "UTC-9:30",
                service.GetOffsetLabel());
        }

        /// <summary>
        /// Tests whether the GetOffsetLabel method returns a quarter hour offset.
        /// </summary>
        [TestMethod]
        public void TestGetOffsetLabelQuarterHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 5.75
            };

            Assert.AreEqual(
                "UTC+5:45",
                service.GetOffsetLabel());
        }

        #endregion

        #region ConvertFromUtc

        /// <summary>
        /// Tests whether the ConvertFromUtc method returns the original datetime when conversion is disabled.
        /// </summary>
        [TestMethod]
        public void TestConvertFromUtcDisabled()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = false,
                OffsetHours = 5
            };

            DateTime utcTime = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual(
                utcTime,
                service.ConvertFromUtc(utcTime));
        }

        /// <summary>
        /// Tests whether the ConvertFromUtc method shifts the datetime by a positive offset.
        /// </summary>
        [TestMethod]
        public void TestConvertFromUtcEnabled()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 1
            };

            DateTime utcTime = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
            DateTime expected = new(2025, 6, 15, 13, 0, 0);

            Assert.AreEqual(
                expected,
                service.ConvertFromUtc(utcTime));
        }

        /// <summary>
        /// Tests whether the ConvertFromUtc method shifts the datetime by a negative offset.
        /// </summary>
        [TestMethod]
        public void TestConvertFromUtcEnabledNegative()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = -5
            };

            DateTime utcTime = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
            DateTime expected = new(2025, 6, 15, 7, 0, 0);

            Assert.AreEqual(
                expected,
                service.ConvertFromUtc(utcTime));
        }

        /// <summary>
        /// Tests whether the ConvertFromUtc method returns unchanged datetime when offset is zero.
        /// </summary>
        [TestMethod]
        public void TestConvertFromUtcEnabledZero()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 0
            };

            DateTime utcTime = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual(
                utcTime,
                service.ConvertFromUtc(utcTime));
        }

        /// <summary>
        /// Tests whether the ConvertFromUtc method shifts the datetime by a half hour offset.
        /// </summary>
        [TestMethod]
        public void TestConvertFromUtcEnabledHalfHour()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 5.5
            };

            DateTime utcTime = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
            DateTime expected = new(2025, 6, 15, 17, 30, 0);

            Assert.AreEqual(
                expected,
                service.ConvertFromUtc(utcTime));
        }

        #endregion

        #region GetDateLabel

        /// <summary>
        /// Tests whether the GetDateLabel method returns the default label.
        /// </summary>
        [TestMethod]
        public void TestGetDateLabelDefault()
        {
            TimezoneService service = new();

            Assert.AreEqual(
                "Date (UTC+0)",
                service.GetDateLabel());
        }

        /// <summary>
        /// Tests whether the GetDateLabel method returns a label with a custom prefix.
        /// </summary>
        [TestMethod]
        public void TestGetDateLabelCustomPrefix()
        {
            TimezoneService service = new();

            Assert.AreEqual(
                "Date Uploaded (UTC+0)",
                service.GetDateLabel("Date Uploaded"));
        }

        /// <summary>
        /// Tests whether the GetDateLabel method includes the offset in the label.
        /// </summary>
        [TestMethod]
        public void TestGetDateLabelWithOffset()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = 1
            };

            Assert.AreEqual(
                "Date (UTC+1)",
                service.GetDateLabel());
        }

        /// <summary>
        /// Tests whether the GetDateLabel method includes a custom prefix and negative offset.
        /// </summary>
        [TestMethod]
        public void TestGetDateLabelCustomPrefixWithOffset()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = true,
                OffsetHours = -5
            };

            Assert.AreEqual(
                "Date Updated (UTC-5)",
                service.GetDateLabel("Date Updated"));
        }

        /// <summary>
        /// Tests whether the GetDateLabel method returns UTC+0 when conversion is disabled.
        /// </summary>
        [TestMethod]
        public void TestGetDateLabelDisabled()
        {
            TimezoneService service = new()
            {
                ConversionEnabled = false,
                OffsetHours = 3
            };

            Assert.AreEqual(
                "Date (UTC+0)",
                service.GetDateLabel());
        }

        #endregion
    }
}
