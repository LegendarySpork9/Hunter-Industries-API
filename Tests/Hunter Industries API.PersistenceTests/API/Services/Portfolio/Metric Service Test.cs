// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Portfolio
{
    [TestClass]
    [DoNotParallelize]
    public class MetricServiceTest
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
        private MetricService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            MetricService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts a portfolio item record and returns the generated ID.
        /// </summary>
        private int InsertPortfolioItem()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Web Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                    "SELECT PIT.PortfolioItemTypeId, 'TestItem', 'Summary', 'Description', 'icon.png', 'Notes', 'https://github.com', GETUTCDATE(), GETUTCDATE(), 0 " +
                    "FROM PortfolioItemType PIT WHERE PIT.[Name] = 'Web Application'; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Checks whether the MetricUpdated method returns true when the update is successful.
        /// </summary>
        [TestMethod]
        public async Task TestMetricUpdated()
        {
            int itemId = InsertPortfolioItem();

            MetricService service = CreateService();

            bool actual = await service.MetricUpdated(
                new ItemMetricModel
                {
                    Id = itemId,
                    Metric = "summary"
                });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MetricUpdated method returns false when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestMetricUpdatedFailed()
        {
            MetricService service = CreateService();

            bool actual = await service.MetricUpdated(
                new ItemMetricModel
                {
                    Id = 999,
                    Metric = "summary"
                });

            Assert.IsFalse(actual);
        }
    }
}
