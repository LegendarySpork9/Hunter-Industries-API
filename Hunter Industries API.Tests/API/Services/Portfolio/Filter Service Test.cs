// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
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
    public class FilterServiceTest
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

        #region GetFilters

        /// <summary>
        /// Checks whether the GetFilters method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetFilters()
        {
            List<FilterRecord> records =
            [
                new FilterRecord
                {
                    Id = 1,
                    Name = "Language",
                    Values = ["C#", "JavaScript"],
                    IsDeleted = false
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<FilterRecord> actual = await service.GetFilters();

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "Language",
                actual[0].Name);
        }

        /// <summary>
        /// Checks whether the GetFilters method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltersEmpty()
        {
            List<FilterRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<FilterRecord> actual = await service.GetFilters();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region FilterExists (string)

        /// <summary>
        /// Checks whether the FilterExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByName()
        {
            List<int> results = [1];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterExists("Language");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByNameNotFound()
        {
            List<int> results = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterExists("NonExistent");

            Assert.IsFalse(actual);
        }

        #endregion

        #region FilterExists (int)

        /// <summary>
        /// Checks whether the FilterExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsById()
        {
            List<int> results = [1];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByIdNotFound()
        {
            List<int> results = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterExists(999);

            Assert.IsFalse(actual);
        }

        #endregion

        #region FilterCreated

        /// <summary>
        /// Checks whether the FilterCreated method returns true and the filter id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestFilterCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            FilterModel filter = new()
            {
                Name = "Language",
                Values = "C#,JavaScript"
            };

            (bool created, int filterId) = await service.FilterCreated(filter);

            Assert.IsTrue(created);
            Assert.AreEqual(
                1,
                filterId);
        }

        /// <summary>
        /// Checks whether the FilterCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestFilterCreatedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            FilterModel filter = new()
            {
                Name = "Language",
                Values = "C#,JavaScript"
            };

            (bool created, int filterId) = await service.FilterCreated(filter);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                filterId);
        }

        /// <summary>
        /// Checks whether the FilterCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestFilterCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            FilterModel filter = new()
            {
                Name = "Language",
                Values = "C#,JavaScript"
            };

            (bool created, int filterId) = await service.FilterCreated(filter);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                filterId);
        }

        #endregion

        #region FilterUpdated

        /// <summary>
        /// Checks whether the FilterUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterUpdated()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioFilter set\n\t[Name] = @name,\n\t[Values] = @values\nwhere PortfolioFilterId = @filterId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterUpdated(
                1,
                new FilterModel { Name = "Updated", Values = "C#,Python" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterUpdatedNoRowsAffected()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioFilter set\n\t[Name] = @name,\n\t[Values] = @values\nwhere PortfolioFilterId = @filterId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterUpdated(
                1,
                new FilterModel { Name = "Updated", Values = "C#,Python" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the FilterUpdated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestFilterUpdatedWithError()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioFilter set\n\t[Name] = @name,\n\t[Values] = @values\nwhere PortfolioFilterId = @filterId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterUpdated(
                1,
                new FilterModel { Name = "Updated", Values = "C#,Python" });

            Assert.IsFalse(actual);
        }

        #endregion

        #region FilterDeleted

        /// <summary>
        /// Checks whether the FilterDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterDeleted()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterDeleted(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterDeletedNoRowsAffected()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterDeleted(1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the FilterDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestFilterDeletedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            FilterService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.FilterDeleted(1);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
