// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace HunterIndustriesAPI.Tests.API.Mappings
{
    [TestClass]
    public class PortfolioDataReaderMappingTest
    {
        #region SingleLinkedItemMapper

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

            object result = PortfolioDataReaderMapping.SingleLinkedItemMapper(mockReader.Object);

            (int id, string name) = ((int, string))result;

            Assert.AreEqual(
                1,
                id);
            Assert.AreEqual(
                "ASP.NET",
                name);
        }

        #endregion

        #region BuildHistoryMapper

        /// <summary>
        /// Tests whether the BuildHistoryMapper returns the correct tuple of (int, BuildHistoryRecord).
        /// </summary>
        [TestMethod]
        public void TestBuildHistoryMapper()
        {
            DateTime releaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Mock<IDataReader> mockReader = new();
            mockReader.Setup(r => r.GetInt32(0))
                .Returns(1);
            mockReader.Setup(r => r.GetString(1))
                .Returns("1.0.0");
            mockReader.Setup(r => r.GetDateTime(3))
                .Returns(releaseDate);

            object result = PortfolioDataReaderMapping.BuildHistoryMapper(mockReader.Object);

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

        #endregion
    }
}
