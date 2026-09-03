// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Portfolio
{
    [TestClass]
    [DoNotParallelize]
    public class PortfolioServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly IFileSystem _FileSystem = new FileSystemWrapper();

        /// <summary>
        /// Creates the test database and schema.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            (_ConnectionString, _DatabaseName) = LocalDbTestHelper.CreateDatabase();
            LocalDbTestHelper.CreateSchema(_ConnectionString);
            _SqlFilesPath = LocalDbTestHelper.GetSqlFilesPath();
        }

        /// <summary>
        /// Drops the test database.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            LocalDbTestHelper.DropDatabase(_DatabaseName);
        }

        /// <summary>
        /// Clears the test data between tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            LocalDbTestHelper.ClearData(_ConnectionString);
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private PortfolioService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            PortfolioService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts prerequisite records for portfolio testing.
        /// </summary>
        private void InsertPortfolioPrerequisites()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Web Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "IF NOT EXISTS (SELECT 1 FROM LLMCompany WHERE [Name] = 'Anthropic') " +
                    "INSERT INTO LLMCompany ([Name]) VALUES ('Anthropic'); " +
                    "IF NOT EXISTS (SELECT 1 FROM LLMModel WHERE [Name] = 'Claude') " +
                    "BEGIN " +
                    "DECLARE @companyId INT = (SELECT LLMCompanyId FROM LLMCompany WHERE [Name] = 'Anthropic'); " +
                    "INSERT INTO LLMModel (LLMCompanyId, [Name]) VALUES (@companyId, 'Claude'); " +
                    "END",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a portfolio item record and returns the generated ID.
        /// </summary>
        private int InsertPortfolioItem(
            string name = "TestItem",
            string type = "Web Application",
            bool withLLM = true)
        {
            InsertPortfolioPrerequisites();

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                string sql;

                if (withLLM)
                {
                    sql = "INSERT INTO PortfolioItem (TypeId, LLMModelId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, UnitTestCoverage, LLMUsageNotes, DateCreated, DateUpdated, IsDeleted) " +
                          "SELECT PIT.PortfolioItemTypeId, LM.LLMModelId, @name, 'Test summary', 'Test description', 'https://example.com/icon.png', 'Initial release', 'https://github.com/test', 85.5, 'Used for code generation', GETUTCDATE(), GETUTCDATE(), 0 " +
                          "FROM PortfolioItemType PIT, LLMModel LM " +
                          "WHERE PIT.[Name] = @type AND LM.[Name] = 'Claude'; " +
                          "SELECT SCOPE_IDENTITY();";
                }

                else
                {
                    sql = "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                          "SELECT PIT.PortfolioItemTypeId, @name, 'Test summary', 'Test description', 'https://example.com/icon.png', 'Initial release', 'https://github.com/test', GETUTCDATE(), GETUTCDATE(), 0 " +
                          "FROM PortfolioItemType PIT " +
                          "WHERE PIT.[Name] = @type; " +
                          "SELECT SCOPE_IDENTITY();";
                }

                using (SqlCommand cmd = new(
                    sql,
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetItems method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetItems()
        {
            InsertPortfolioItem();

            PortfolioService service = CreateService();

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
            PortfolioService service = CreateService();

            List<ItemRecord> actual = await service.GetItems();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetLinkedItemData method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetLinkedItemData()
        {
            int itemId = InsertPortfolioItem();

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Framework WHERE [Name] = 'ASP.NET') " +
                    "INSERT INTO Framework ([Name]) VALUES ('ASP.NET'); " +
                    "INSERT INTO PortfolioItemFramework (PortfolioItemId, FrameworkId) " +
                    "SELECT @itemId, FrameworkId FROM Framework WHERE [Name] = 'ASP.NET';",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            PortfolioService service = CreateService();

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
            PortfolioService service = CreateService();

            List<object> actual = await service.GetLinkedItemData("frameworks");

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetItem method returns the correct record.
        /// </summary>
        [TestMethod]
        public async Task TestGetItem()
        {
            int itemId = InsertPortfolioItem();

            PortfolioService service = CreateService();

            ItemRecord actual = await service.GetItem(itemId);

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
            PortfolioService service = CreateService();

            ItemRecord actual = await service.GetItem(999);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByName()
        {
            InsertPortfolioItem();

            PortfolioService service = CreateService();

            bool actual = await service.ItemExists("TestItem");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByNameNotFound()
        {
            PortfolioService service = CreateService();

            bool actual = await service.ItemExists("NonExistent");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsById()
        {
            int itemId = InsertPortfolioItem();

            PortfolioService service = CreateService();

            bool actual = await service.ItemExists(itemId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestItemExistsByIdNotFound()
        {
            PortfolioService service = CreateService();

            bool actual = await service.ItemExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemCreated method returns true and the item id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestItemCreated()
        {
            InsertPortfolioPrerequisites();

            PortfolioService service = CreateService();

            ItemModel item = new()
            {
                Name = "TestItem",
                Type = "Web Application",
                IconURL = "https://example.com/icon.png",
                Summary = "Test summary",
                Description = "Test description",
                DemoLink = null,
                ReleaseNotes = "Initial release",
                UnitTestCoverage = 85.5m,
                GitHubLink = "https://github.com/test",
                LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" },
                LLMUsageNotes = "Used for code generation"
            };

            (bool created, int itemId) = await service.ItemCreated(item);

            Assert.IsTrue(created);
            Assert.IsTrue(itemId > 0);
        }

        /// <summary>
        /// Checks whether the ItemCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestItemCreatedNullResult()
        {
            PortfolioService service = CreateService();

            ItemModel item = new()
            {
                Name = "TestItem",
                Type = "NonExistentType",
                IconURL = "https://example.com/icon.png",
                Summary = "Test summary",
                Description = "Test description",
                DemoLink = null,
                ReleaseNotes = "Initial release",
                UnitTestCoverage = 85.5m,
                GitHubLink = "https://github.com/test",
                LLMUsage = new LLMRecord { Company = "NonExistent", Model = "NonExistent" },
                LLMUsageNotes = "Used for code generation"
            };

            (bool created, int itemId) = await service.ItemCreated(item);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                itemId);
        }

        /// <summary>
        /// Checks whether the LinkedItemDataCreated method returns true when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestLinkedItemDataCreated()
        {
            int itemId = InsertPortfolioItem();

            PortfolioService service = CreateService();

            BuildHistoryRecord record = new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            bool actual = await service.LinkedItemDataCreated(
                "buildHistories",
                itemId,
                record);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkedItemDataCreated method returns false when the framework already exists.
        /// </summary>
        [TestMethod]
        public async Task TestLinkedItemDataCreatedNoRowsAffected()
        {
            PortfolioService service = CreateService();

            await service.LinkedItemDataCreated(
                "frameworks",
                999,
                "ExistingFramework");

            bool actual = await service.LinkedItemDataCreated(
                "frameworks",
                999,
                "ExistingFramework");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataDeleted method returns true when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataDeleted()
        {
            int itemId = InsertPortfolioItem();

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Framework WHERE [Name] = 'ASP.NET') " +
                    "INSERT INTO Framework ([Name]) VALUES ('ASP.NET'); " +
                    "INSERT INTO PortfolioItemFramework (PortfolioItemId, FrameworkId) " +
                    "SELECT @itemId, FrameworkId FROM Framework WHERE [Name] = 'ASP.NET';",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            PortfolioService service = CreateService();

            bool actual = await service.LinkItemDataDeleted(
                "frameworks",
                itemId,
                "ASP.NET");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataDeletedNoRowsAffected()
        {
            PortfolioService service = CreateService();

            bool actual = await service.LinkItemDataDeleted(
                "frameworks",
                999,
                "ASP.NET");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataCreated method returns true when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataCreated()
        {
            int itemId = InsertPortfolioItem();

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM Framework WHERE [Name] = 'ASP.NET') " +
                    "INSERT INTO Framework ([Name]) VALUES ('ASP.NET');",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            PortfolioService service = CreateService();

            bool actual = await service.LinkItemDataCreated(
                "frameworks",
                itemId,
                "ASP.NET");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the LinkItemDataCreated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestLinkItemDataCreatedNoRowsAffected()
        {
            PortfolioService service = CreateService();

            bool actual = await service.LinkItemDataCreated(
                "frameworks",
                999,
                "NonExistentFramework");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemUpdated()
        {
            int itemId = InsertPortfolioItem();

            PortfolioService service = CreateService();

            bool actual = await service.ItemUpdated(
                itemId,
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
            PortfolioService service = CreateService();

            bool actual = await service.ItemUpdated(
                999,
                new ItemModel
                {
                    Name = "Updated",
                    LLMUsage = new LLMRecord { Company = "Anthropic", Model = "Claude" }
                });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ItemDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemDeleted()
        {
            int itemId = InsertPortfolioItem();

            PortfolioService service = CreateService();

            bool actual = await service.ItemDeleted(itemId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ItemDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestItemDeletedNoRowsAffected()
        {
            PortfolioService service = CreateService();

            bool actual = await service.ItemDeleted(999);

            Assert.IsFalse(actual);
        }

    }
}
