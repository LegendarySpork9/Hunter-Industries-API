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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Portfolio
{
    [TestClass]
    [DoNotParallelize]
    public class FilterServiceTest
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
        private FilterService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            FilterService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts a portfolio filter record and returns the generated ID.
        /// </summary>
        private int InsertFilter(
            string name,
            string type,
            string filterOperator = null,
            string path = null,
            string values = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioFilter ([Name], [Type], [Operator], [Path], [Values]) " +
                    "VALUES (@name, @type, @operator, @path, @values); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@type",
                        type);
                    cmd.Parameters.AddWithValue(
                        "@operator",
                        (object)filterOperator ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@path",
                        (object)path ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@values",
                        (object)values ?? System.DBNull.Value);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetFilters method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetFilters()
        {
            InsertFilter(
                "Language",
                "tag",
                values: "C#,JavaScript");

            FilterService service = CreateService();

            List<FilterRecord> actual = await service.GetFilters();

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "Language",
                actual[0].Name);
            Assert.AreEqual(
                "tag",
                actual[0].Type);
        }

        /// <summary>
        /// Checks whether the GetFilters method returns records with the new filter type fields.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltersWithNullCheckType()
        {
            InsertFilter(
                "Has LLM Usage",
                "null",
                filterOperator: "has value",
                path: "llmUsage");

            FilterService service = CreateService();

            List<FilterRecord> actual = await service.GetFilters();

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "null",
                actual[0].Type);
            Assert.AreEqual(
                "has value",
                actual[0].Operator);
            Assert.AreEqual(
                "llmUsage",
                actual[0].Path);
            Assert.IsNull(actual[0].Values);
        }

        /// <summary>
        /// Checks whether the GetFilters method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltersEmpty()
        {
            FilterService service = CreateService();

            List<FilterRecord> actual = await service.GetFilters();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByName()
        {
            InsertFilter(
                "Language",
                "tag",
                values: "C#,JavaScript");

            FilterService service = CreateService();

            bool actual = await service.FilterExists("Language");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByNameNotFound()
        {
            FilterService service = CreateService();

            bool actual = await service.FilterExists("NonExistent");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsById()
        {
            int filterId = InsertFilter(
                "Language",
                "tag",
                values: "C#,JavaScript");

            FilterService service = CreateService();

            bool actual = await service.FilterExists(filterId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestFilterExistsByIdNotFound()
        {
            FilterService service = CreateService();

            bool actual = await service.FilterExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the FilterCreated method returns true and the filter id when a tag filter is created.
        /// </summary>
        [TestMethod]
        public async Task TestFilterCreated()
        {
            FilterService service = CreateService();

            FilterModel filter = new()
            {
                Name = "Language",
                Type = "tag",
                Values = "C#,JavaScript"
            };

            (bool created, int filterId) = await service.FilterCreated(filter);

            Assert.IsTrue(created);
            Assert.IsTrue(filterId > 0);
        }

        /// <summary>
        /// Checks whether the FilterCreated method returns true when a null check filter is created with no values.
        /// </summary>
        [TestMethod]
        public async Task TestFilterCreatedWithNullValues()
        {
            FilterService service = CreateService();

            FilterModel filter = new()
            {
                Name = "Has LLM Usage",
                Type = "null",
                Operator = "has value",
                Path = "llmUsage"
            };

            (bool created, int filterId) = await service.FilterCreated(filter);

            Assert.IsTrue(created);
            Assert.IsTrue(filterId > 0);
        }

        /// <summary>
        /// Checks whether the FilterUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterUpdated()
        {
            int filterId = InsertFilter(
                "Language",
                "tag",
                values: "C#,JavaScript");

            FilterService service = CreateService();

            bool actual = await service.FilterUpdated(
                filterId,
                new FilterModel { Name = "Updated", Type = "tag", Values = "C#,Python" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterUpdatedNoRowsAffected()
        {
            FilterService service = CreateService();

            bool actual = await service.FilterUpdated(
                999,
                new FilterModel { Name = "Updated", Type = "tag", Values = "C#,Python" });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the FilterDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterDeleted()
        {
            int filterId = InsertFilter(
                "Language",
                "tag",
                values: "C#,JavaScript");

            FilterService service = CreateService();

            bool actual = await service.FilterDeleted(filterId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the FilterDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestFilterDeletedNoRowsAffected()
        {
            FilterService service = CreateService();

            bool actual = await service.FilterDeleted(999);

            Assert.IsFalse(actual);
        }

    }
}
