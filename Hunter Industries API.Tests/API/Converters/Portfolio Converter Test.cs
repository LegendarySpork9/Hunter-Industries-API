// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Objects.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class PortfolioConverterTest
    {
        #region GetSQLGet

        /// <summary>
        /// Tests whether the GetSQLGet method returns the correct path when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetFrameworks()
        {
            string expected = Path.Combine(
                "Framework",
                "GetFrameworks.sql");
            string actual = PortfolioConverter.GetSQLGet("frameworks");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns the correct path when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetLanguages()
        {
            string expected = Path.Combine(
                "Language",
                "GetLanguages.sql");
            string actual = PortfolioConverter.GetSQLGet("languages");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns the correct path when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetEnvironments()
        {
            string expected = Path.Combine(
                "Environment",
                "GetEnvironments.sql");
            string actual = PortfolioConverter.GetSQLGet("environments");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns the correct path when given "buildHistories".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetBuildHistories()
        {
            string expected = Path.Combine(
                "Build History",
                "GetBuildHistories.sql");
            string actual = PortfolioConverter.GetSQLGet("buildHistories");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "Unknown.sql" when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetDefault()
        {
            string expected = "Unknown.sql";
            string actual = PortfolioConverter.GetSQLGet("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetSQLCreate

        /// <summary>
        /// Tests whether the GetSQLCreate method returns the correct path when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateFrameworks()
        {
            string expected = Path.Combine(
                "Framework",
                "CreateFramework.sql");
            string actual = PortfolioConverter.GetSQLCreate("frameworks");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns the correct path when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLanguages()
        {
            string expected = Path.Combine(
                "Language",
                "CreateLanguage.sql");
            string actual = PortfolioConverter.GetSQLCreate("languages");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns the correct path when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateEnvironments()
        {
            string expected = Path.Combine(
                "Environment",
                "CreateEnvironment.sql");
            string actual = PortfolioConverter.GetSQLCreate("environments");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns the correct path when given "buildHistories".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateBuildHistories()
        {
            string expected = Path.Combine(
                "Build History",
                "CreateBuildHistory.sql");
            string actual = PortfolioConverter.GetSQLCreate("buildHistories");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateItemType.sql" when given "type".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateType()
        {
            string expected = "CreateItemType.sql";
            string actual = PortfolioConverter.GetSQLCreate("type");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateLLMCompany.sql" when given "llmCompany".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLLMCompany()
        {
            string expected = "CreateLLMCompany.sql";
            string actual = PortfolioConverter.GetSQLCreate("llmCompany");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateLLMModel.sql" when given "llmModel".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLLMModel()
        {
            string expected = "CreateLLMModel.sql";
            string actual = PortfolioConverter.GetSQLCreate("llmModel");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "Unknown.sql" when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateDefault()
        {
            string expected = "Unknown.sql";
            string actual = PortfolioConverter.GetSQLCreate("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetSQLCreateLink

        /// <summary>
        /// Tests whether the GetSQLCreateLink method returns the correct path when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLinkFrameworks()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "CreateItemFramework.sql");
            string actual = PortfolioConverter.GetSQLCreateLink("frameworks");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreateLink method returns the correct path when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLinkLanguages()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "CreateItemLanguage.sql");
            string actual = PortfolioConverter.GetSQLCreateLink("languages");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreateLink method returns the correct path when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLinkEnvironments()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "CreateItemEnvironment.sql");
            string actual = PortfolioConverter.GetSQLCreateLink("environments");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreateLink method returns "Unknown.sql" when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateLinkDefault()
        {
            string expected = "Unknown.sql";
            string actual = PortfolioConverter.GetSQLCreateLink("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetSQLDeleteLink

        /// <summary>
        /// Tests whether the GetSQLDeleteLink method returns the correct path when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDeleteLinkFrameworks()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "DeleteItemFramework.sql");
            string actual = PortfolioConverter.GetSQLDeleteLink("frameworks");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDeleteLink method returns the correct path when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDeleteLinkLanguages()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "DeleteItemLanguage.sql");
            string actual = PortfolioConverter.GetSQLDeleteLink("languages");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDeleteLink method returns the correct path when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDeleteLinkEnvironments()
        {
            string expected = Path.Combine(
                "Item",
                "Links",
                "DeleteItemEnvironment.sql");
            string actual = PortfolioConverter.GetSQLDeleteLink("environments");

            Assert.AreEqual(
                expected,
                actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDeleteLink method returns "Unknown.sql" when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLDeleteLinkDefault()
        {
            string expected = "Unknown.sql";
            string actual = PortfolioConverter.GetSQLDeleteLink("Trombone");

            Assert.AreEqual(
                expected,
                actual);
        }

        #endregion

        #region GetParametersCreate

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateFrameworks()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "frameworks",
                1,
                "ASP.NET");

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "@name",
                actual[0].ParameterName);
            Assert.AreEqual(
                "ASP.NET",
                actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLanguages()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "languages",
                1,
                "C#");

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "@name",
                actual[0].ParameterName);
            Assert.AreEqual(
                "C#",
                actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateEnvironments()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "environments",
                1,
                "Windows");

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "@name",
                actual[0].ParameterName);
            Assert.AreEqual(
                "Windows",
                actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "buildHistories".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateBuildHistories()
        {
            DateTime releaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            BuildHistoryRecord record = new BuildHistoryRecord
            {
                Version = "1.0.0",
                ReleaseDate = releaseDate
            };

            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "buildHistories",
                1,
                record);

            Assert.AreEqual(
                3,
                actual.Length);
            Assert.AreEqual(
                "@itemId",
                actual[0].ParameterName);
            Assert.AreEqual(
                1,
                actual[0].Value);
            Assert.AreEqual(
                "@version",
                actual[1].ParameterName);
            Assert.AreEqual(
                "1.0.0",
                actual[1].Value);
            Assert.AreEqual(
                "@releaseDate",
                actual[2].ParameterName);
            Assert.AreEqual(
                releaseDate,
                actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "type".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateType()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "type",
                1,
                "Web Application");

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "@name",
                actual[0].ParameterName);
            Assert.AreEqual(
                "Web Application",
                actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "llmCompany".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLLMCompany()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "llmCompany",
                1,
                "Anthropic");

            Assert.AreEqual(
                1,
                actual.Length);
            Assert.AreEqual(
                "@name",
                actual[0].ParameterName);
            Assert.AreEqual(
                "Anthropic",
                actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "llmModel".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLLMModel()
        {
            LLMRecord record = new LLMRecord
            {
                Company = "Anthropic",
                Model = "Claude"
            };

            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "llmModel",
                1,
                record);

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "@company",
                actual[0].ParameterName);
            Assert.AreEqual(
                "Anthropic",
                actual[0].Value);
            Assert.AreEqual(
                "@model",
                actual[1].ParameterName);
            Assert.AreEqual(
                "Claude",
                actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns an empty array when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateDefault()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreate(
                "Trombone",
                1,
                "Test");

            Assert.AreEqual(
                0,
                actual.Length);
        }

        #endregion

        #region GetParametersCreateLink

        /// <summary>
        /// Tests whether the GetParametersCreateLink method returns the correct parameters when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLinkFrameworks()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreateLink(
                "frameworks",
                1,
                "ASP.NET");

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "@itemId",
                actual[0].ParameterName);
            Assert.AreEqual(
                1,
                actual[0].Value);
            Assert.AreEqual(
                "@framework",
                actual[1].ParameterName);
            Assert.AreEqual(
                "ASP.NET",
                actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreateLink method returns the correct parameters when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLinkLanguages()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreateLink(
                "languages",
                1,
                "C#");

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "@itemId",
                actual[0].ParameterName);
            Assert.AreEqual(
                1,
                actual[0].Value);
            Assert.AreEqual(
                "@language",
                actual[1].ParameterName);
            Assert.AreEqual(
                "C#",
                actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreateLink method returns the correct parameters when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLinkEnvironments()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreateLink(
                "environments",
                1,
                "Windows");

            Assert.AreEqual(
                2,
                actual.Length);
            Assert.AreEqual(
                "@itemId",
                actual[0].ParameterName);
            Assert.AreEqual(
                1,
                actual[0].Value);
            Assert.AreEqual(
                "@environment",
                actual[1].ParameterName);
            Assert.AreEqual(
                "Windows",
                actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreateLink method returns an empty array when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateLinkDefault()
        {
            SqlParameter[] actual = PortfolioConverter.GetParametersCreateLink(
                "Trombone",
                1,
                "Test");

            Assert.AreEqual(
                0,
                actual.Length);
        }

        #endregion

        #region GetDataReaderMappings

        /// <summary>
        /// Tests whether the GetDataReaderMappings method returns the SingleLinkedItemMapper when given "frameworks".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsFrameworks()
        {
            Func<IDataReader, object> actual = PortfolioConverter.GetDataReaderMappings("frameworks");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappings method returns the SingleLinkedItemMapper when given "languages".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsLanguages()
        {
            Func<IDataReader, object> actual = PortfolioConverter.GetDataReaderMappings("languages");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappings method returns the SingleLinkedItemMapper when given "environments".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsEnvironments()
        {
            Func<IDataReader, object> actual = PortfolioConverter.GetDataReaderMappings("environments");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappings method returns the BuildHistoryMapper when given "buildHistories".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsBuildHistories()
        {
            Func<IDataReader, object> actual = PortfolioConverter.GetDataReaderMappings("buildHistories");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappings method returns null when given an unrecognised value.
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDefault()
        {
            Func<IDataReader, object> actual = PortfolioConverter.GetDataReaderMappings("Trombone");

            Assert.IsNull(actual);
        }

        #endregion
    }
}
