// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.Portfolio
{
    [TestClass]
    public class GitHubServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();

        [TestInitialize]
        public void Setup()
        {
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQL");
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("SELECT 1");
        }

        #region GetCIStatus

        /// <summary>
        /// Checks whether the GetCIStatus method returns the correct dictionary entries.
        /// </summary>
        [TestMethod]
        public async Task TestGetCIStatus()
        {
            List<KeyValuePair<string, string>> results =
            [
                new KeyValuePair<string, string>("Build", "Passing"),
                new KeyValuePair<string, string>("Tests", "Passing")
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, KeyValuePair<string, string>>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            Dictionary<string, string> actual = await service.GetCIStatus("TestRepo");

            Assert.AreEqual(
                2,
                actual.Count);
            Assert.AreEqual(
                "Passing",
                actual["Build"]);
        }

        /// <summary>
        /// Checks whether the GetCIStatus method returns an empty dictionary when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetCIStatusEmpty()
        {
            List<KeyValuePair<string, string>> results = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, KeyValuePair<string, string>>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            Dictionary<string, string> actual = await service.GetCIStatus("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetIssueBreakdown

        /// <summary>
        /// Checks whether the GetIssueBreakdown method returns the correct record.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueBreakdown()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingleGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new GitHubIssueBreakdownRecord
                    {
                        TotalIssues = 10,
                        Bugs = 3,
                        NewFeatures = 7
                    },
                    (Exception)null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            GitHubIssueBreakdownRecord actual = await service.GetIssueBreakdown("TestRepo");

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                10,
                actual.TotalIssues);
            Assert.AreEqual(
                3,
                actual.Bugs);
            Assert.AreEqual(
                7,
                actual.NewFeatures);
        }

        /// <summary>
        /// Checks whether the GetIssueBreakdown method returns null when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueBreakdownNull()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingleGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (GitHubIssueBreakdownRecord)null,
                    (Exception)null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            GitHubIssueBreakdownRecord actual = await service.GetIssueBreakdown("TestRepo");

            Assert.IsNull(actual);
        }

        #endregion

        #region GetIssueAssigneeBreakdown

        /// <summary>
        /// Checks whether the GetIssueAssigneeBreakdown method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueAssigneeBreakdown()
        {
            List<GitHubIssueAssigneeBreakdownRecord> records =
            [
                new GitHubIssueAssigneeBreakdownRecord
                {
                    Name = "Developer1",
                    Issues = 5
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueAssigneeBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<GitHubIssueAssigneeBreakdownRecord> actual = await service.GetIssueAssigneeBreakdown("TestRepo");

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "Developer1",
                actual[0].Name);
            Assert.AreEqual(
                5,
                actual[0].Issues);
        }

        /// <summary>
        /// Checks whether the GetIssueAssigneeBreakdown method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueAssigneeBreakdownEmpty()
        {
            List<GitHubIssueAssigneeBreakdownRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueAssigneeBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<GitHubIssueAssigneeBreakdownRecord> actual = await service.GetIssueAssigneeBreakdown("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetIssueInProgressBreakdown

        /// <summary>
        /// Checks whether the GetIssueInProgressBreakdown method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueInProgressBreakdown()
        {
            List<GitHubIssueInProgressBreakdownRecord> records =
            [
                new GitHubIssueInProgressBreakdownRecord
                {
                    Id = 42,
                    Assignee = "Developer1",
                    Title = "Fix login bug",
                    Type = "Bug"
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueInProgressBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<GitHubIssueInProgressBreakdownRecord> actual = await service.GetIssueInProgressBreakdown("TestRepo");

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                42,
                actual[0].Id);
            Assert.AreEqual(
                "Developer1",
                actual[0].Assignee);
            Assert.AreEqual(
                "Fix login bug",
                actual[0].Title);
            Assert.AreEqual(
                "Bug",
                actual[0].Type);
        }

        /// <summary>
        /// Checks whether the GetIssueInProgressBreakdown method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetIssueInProgressBreakdownEmpty()
        {
            List<GitHubIssueInProgressBreakdownRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QueryGitHub(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, GitHubIssueInProgressBreakdownRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            GitHubService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<GitHubIssueInProgressBreakdownRecord> actual = await service.GetIssueInProgressBreakdown("TestRepo");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion
    }
}
