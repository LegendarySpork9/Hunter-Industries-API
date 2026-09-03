// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.Objects.Statistics.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using ItemMappings = HunterIndustriesAPI.Mappings.Portfolio.PortfolioDataReaderMapping;
using StatisticMappings = HunterIndustriesAPI.Mappings.PortfolioDataReaderMapping;

namespace HunterIndustriesAPI.UnitTests.API.Mappings
{
    [TestClass]
    public class PortfolioDataReaderMappingTest
    {
        /// <summary>
        /// Tests whether the SingleLinkedItemMapper returns the correct tuple of (int, string).
        /// </summary>
        [TestMethod]
        public void TestSingleLinkedItemMapper()
        {
            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(1);
            mockReader.Setup(r => r.GetString(1))
                .Returns("ASP.NET");

            object result = ItemMappings.SingleLinkedItemMapper(mockReader.Object);

            (int id, string name) = ((int, string))result;

            Assert.AreEqual(
                1,
                id);
            Assert.AreEqual(
                "ASP.NET",
                name);
        }

        /// <summary>
        /// Tests whether the BuildHistoryMapper returns the correct tuple of (int, BuildHistoryRecord).
        /// </summary>
        [TestMethod]
        public void TestBuildHistoryMapper()
        {
            DateTime releaseDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(1);
            mockReader.Setup(r => r.GetString(1))
                .Returns("1.0.0");
            mockReader.Setup(r => r.GetDateTime(2))
                .Returns(releaseDate);

            object result = ItemMappings.BuildHistoryMapper(mockReader.Object);

            (int id, BuildHistoryRecord buildHistory) = ((int, BuildHistoryRecord))result;

            Assert.AreEqual(
                1,
                id);
            Assert.AreEqual(
                "1.0.0",
                buildHistory.Version);
            Assert.AreEqual(
                releaseDate,
                buildHistory.ReleaseDate);
        }

        /// <summary>
        /// Tests whether the TopBarStatusMapper returns the correct TopBarStatRecord.
        /// </summary>
        [TestMethod]
        public void TestTopBarStatusMapper()
        {
            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(10);
            mockReader.Setup(r => r.GetInt32(1))
                .Returns(3);
            mockReader.Setup(r => r.GetInt32(2))
                .Returns(5);

            TopBarStatRecord result = StatisticMappings.TopBarStatusMapper(mockReader.Object);

            Assert.AreEqual(
                10,
                result.Items);
            Assert.AreEqual(
                3,
                result.Filters);
            Assert.AreEqual(
                5,
                result.AIUsed);
        }

        /// <summary>
        /// Tests whether the TopFiveViewedItemMapper returns the correct TopFiveViewedItemsRecord.
        /// </summary>
        [TestMethod]
        public void TestTopFiveViewedItemMapper()
        {
            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetString(0))
                .Returns("Test Item");
            mockReader.Setup(r => r.GetInt32(1))
                .Returns(100);
            mockReader.Setup(r => r.GetInt32(2))
                .Returns(50);
            mockReader.Setup(r => r.GetInt32(3))
                .Returns(150);

            TopFiveViewedItemsRecord result = StatisticMappings.TopFiveViewedItemMapper(mockReader.Object);

            Assert.AreEqual(
                "Test Item",
                result.Name);
            Assert.AreEqual(
                100,
                result.SummaryViews);
            Assert.AreEqual(
                50,
                result.FullDetailViews);
            Assert.AreEqual(
                150,
                result.TotalViews);
        }

        /// <summary>
        /// Tests whether the TopFiveMapper returns the correct TopFiveRecord.
        /// </summary>
        [TestMethod]
        public void TestTopFiveMapper()
        {
            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetString(0))
                .Returns("ASP.NET");
            mockReader.Setup(r => r.GetInt32(1))
                .Returns(5);

            TopFiveRecord result = StatisticMappings.TopFiveMapper(mockReader.Object);

            Assert.AreEqual(
                "ASP.NET",
                result.Name);
            Assert.AreEqual(
                5,
                result.Uses);
        }

        /// <summary>
        /// Tests whether the LLMUsedMapper returns the correct LLMUsedRecord.
        /// </summary>
        [TestMethod]
        public void TestLLMUsedMapper()
        {
            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetString(0))
                .Returns("Anthropic");
            mockReader.Setup(r => r.GetString(1))
                .Returns("Claude");
            mockReader.Setup(r => r.GetInt32(2))
                .Returns(3);

            LLMUsedRecord result = StatisticMappings.LLMUsedMapper(mockReader.Object);

            Assert.AreEqual(
                "Anthropic",
                result.Company);
            Assert.AreEqual(
                "Claude",
                result.Model);
            Assert.AreEqual(
                3,
                result.Uses);
        }
    }
}
