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
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.Portfolio
{
    [TestClass]
    public class PortfolioServiceTest
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

        #region GetItems

        /// <summary>
        /// Checks whether the GetItems method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetItems()
        {
            List<ItemRecord> records =
            [
                new ItemRecord
                {
                    Id = 1,
                    Name = "TestItem",
                    Type = "Web Application",
                    IconURL = "https://example.com/icon.png",
                    Summary = "Test summary",
                    Description = "Test description",
                    DemoURL = null,
                    ReleaseNotes = "Initial release",
                    UnitTestCoverage = 85.5m,
                    LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                    LLMUsageNotes = "Used for code generation",
                    DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    DateUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ItemRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<ItemRecord> actual = await service.GetItems();

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "TestItem",
                actual[0].Name);
        }

        /// <summary>
        /// Checks whether the GetItems method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetItemsEmpty()
        {
            List<ItemRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ItemRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<ItemRecord> actual = await service.GetItems();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetLinkedItemData

        /// <summary>
        /// Checks whether the GetLinkedItemData method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetLinkedItemData()
        {
            List<object> records = [("ASP.NET")];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> actual = await service.GetLinkedItemData("frameworks");

            Assert.AreEqual(
                1,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetLinkedItemData method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetLinkedItemDataEmpty()
        {
            List<object> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<object> actual = await service.GetLinkedItemData("frameworks");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetItem

        /// <summary>
        /// Checks whether the GetItem method returns the correct record.
        /// </summary>
        [TestMethod]
        public async Task TestGetItem()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ItemRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new ItemRecord
                    {
                        Id = 1,
                        Name = "TestItem",
                        Type = "Web Application",
                        IconURL = "https://example.com/icon.png",
                        Summary = "Test summary",
                        Description = "Test description",
                        DemoURL = null,
                        ReleaseNotes = "Initial release",
                        UnitTestCoverage = 85.5m,
                        LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                        LLMUsageNotes = "Used for code generation",
                        DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        DateUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    (Exception)null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ItemRecord actual = await service.GetItem(1);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                "TestItem",
                actual.Name);
        }

        /// <summary>
        /// Checks whether the GetItem method returns null when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetItemNull()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ItemRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (ItemRecord)null,
                    (Exception)null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ItemRecord actual = await service.GetItem(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region ItemExists (string)

        /// <summary>
        /// Checks whether the ItemExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByName()
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

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemExists("TestItem");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByNameNotFound()
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

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemExists("NonExistent");

            Assert.IsFalse(actual);
        }

        #endregion

        #region ItemExists (int)

        /// <summary>
        /// Checks whether the ItemExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsById()
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

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByIdNotFound()
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

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemExists(999);

            Assert.IsFalse(actual);
        }

        #endregion

        #region ItemCreated

        /// <summary>
        /// Checks whether the ItemCreated method returns true and the item id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestItemCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ItemModel item = new()
            {
                Name = "TestItem",
                Type = "Web Application",
                IconURL = "https://example.com/icon.png",
                Summary = "Test summary",
                Description = "Test description",
                DemoURL = null,
                ReleaseNotes = "Initial release",
                UnitTestCoverage = 85.5m,
                GitHubURL = "https://github.com/test",
                LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                LLMUsageNotes = "Used for code generation"
            };

            (bool created, int itemId) = await service.ItemCreated(item);

            Assert.IsTrue(created);
            Assert.AreEqual(
                1,
                itemId);
        }

        /// <summary>
        /// Checks whether the ItemCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestItemCreatedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ItemModel item = new()
            {
                Name = "TestItem",
                Type = "Web Application",
                IconURL = "https://example.com/icon.png",
                Summary = "Test summary",
                Description = "Test description",
                DemoURL = null,
                ReleaseNotes = "Initial release",
                UnitTestCoverage = 85.5m,
                GitHubURL = "https://github.com/test",
                LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                LLMUsageNotes = "Used for code generation"
            };

            (bool created, int itemId) = await service.ItemCreated(item);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                itemId);
        }

        /// <summary>
        /// Checks whether the ItemCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestItemCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            ItemModel item = new()
            {
                Name = "TestItem",
                Type = "Web Application",
                IconURL = "https://example.com/icon.png",
                Summary = "Test summary",
                Description = "Test description",
                DemoURL = null,
                ReleaseNotes = "Initial release",
                UnitTestCoverage = 85.5m,
                GitHubURL = "https://github.com/test",
                LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                LLMUsageNotes = "Used for code generation"
            };

            (bool created, int itemId) = await service.ItemCreated(item);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                itemId);
        }

        #endregion

        #region LinkedItemDataCreated

        /// <summary>
        /// Checks whether the LinkedItemDataCreated method returns true when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestLinkedItemDataCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            BuildHistoryRecord record = new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            bool actual = await service.LinkedItemDataCreated(
                "buildHistories",
                1,
                record);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkedItemDataCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLinkedItemDataCreatedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            BuildHistoryRecord record = new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            bool actual = await service.LinkedItemDataCreated(
                "buildHistories",
                1,
                record);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the LinkedItemDataCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestLinkedItemDataCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            BuildHistoryRecord record = new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            bool actual = await service.LinkedItemDataCreated(
                "buildHistories",
                1,
                record);

            Assert.IsFalse(actual);
        }

        #endregion

        #region LinkItemDataDeleted

        /// <summary>
        /// Checks whether the LinkItemDataDeleted method returns true when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataDeleted()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataDeleted(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataDeleted method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataDeletedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataDeleted(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataDeletedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataDeleted(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsFalse(actual);
        }

        #endregion

        #region LinkItemDataCreated

        /// <summary>
        /// Checks whether the LinkItemDataCreated method returns true when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataCreated(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataCreatedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataCreated(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.LinkItemDataCreated(
                "frameworks",
                1,
                "ASP.NET");

            Assert.IsFalse(actual);
        }

        #endregion

        #region ItemUpdated

        /// <summary>
        /// Checks whether the ItemUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemUpdated()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioItem set\n\t[Name] = @name\nwhere PortfolioItemId = @itemId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemUpdated(
                1,
                new ItemModel
                {
                    Name = "Updated",
                    LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" }
                });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemUpdatedNoRowsAffected()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioItem set\n\t[Name] = @name\nwhere PortfolioItemId = @itemId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemUpdated(
                1,
                new ItemModel
                {
                    Name = "Updated",
                    LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" }
                });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemUpdated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestItemUpdatedWithError()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioItem set\n\t[Name] = @name\nwhere PortfolioItemId = @itemId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemUpdated(
                1,
                new ItemModel
                {
                    Name = "Updated",
                    LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" }
                });

            Assert.IsFalse(actual);
        }

        #endregion

        #region ItemDeleted

        /// <summary>
        /// Checks whether the ItemDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemDeleted()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemDeleted(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemDeletedNoRowsAffected()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemDeleted(1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestItemDeletedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            PortfolioService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ItemDeleted(1);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
