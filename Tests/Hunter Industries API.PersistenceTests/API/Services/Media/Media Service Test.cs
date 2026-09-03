// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services.Media;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services.Media
{
    [TestClass]
    [DoNotParallelize]
    public class MediaServiceTest
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
        private MediaService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            MediaService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts prerequisite records for media testing.
        /// </summary>
        private void InsertMediaPrerequisites(
            string applicationName,
            string extension,
            string mimeType,
            string domainHost)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('testphrase', 0); " +
                    "DECLARE @phraseId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, @appName, 0); " +
                    "IF NOT EXISTS (SELECT 1 FROM MediaType WHERE Extension = @ext AND MimeType = @mime) " +
                    "INSERT INTO MediaType (Extension, MimeType) VALUES (@ext, @mime); " +
                    "IF NOT EXISTS (SELECT 1 FROM Domain WHERE Host = @host) " +
                    "INSERT INTO Domain (Host, IsDeleted) VALUES (@host, 0);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);
                    cmd.Parameters.AddWithValue(
                        "@ext",
                        extension);
                    cmd.Parameters.AddWithValue(
                        "@mime",
                        mimeType);
                    cmd.Parameters.AddWithValue(
                        "@host",
                        domainHost);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a media record and returns the generated ID.
        /// </summary>
        private int InsertMedia(
            string applicationName,
            string name,
            string extension,
            string mimeType,
            string domainHost,
            long size,
            string path = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Media (MediaTypeId, DomainId, ApplicationId, [Name], Size, [Path], DateUploaded, DateUpdated, IsDeleted) " +
                    "SELECT MT.MediaTypeId, D.DomainId, A.ApplicationId, @name, @size, @path, GETUTCDATE(), GETUTCDATE(), 0 " +
                    "FROM MediaType MT, Domain D, [Application] A " +
                    "WHERE MT.Extension = @ext AND MT.MimeType = @mime " +
                    "AND D.Host = @host " +
                    "AND A.[Name] = @appName; " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@size",
                        size);
                    cmd.Parameters.AddWithValue(
                        "@path",
                        (object)path ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@ext",
                        extension);
                    cmd.Parameters.AddWithValue(
                        "@mime",
                        mimeType);
                    cmd.Parameters.AddWithValue(
                        "@host",
                        domainHost);
                    cmd.Parameters.AddWithValue(
                        "@appName",
                        applicationName);

                    return (int)(decimal)cmd.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// Checks whether the GetApplicationMedia method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationMedia()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            (List<MediaRecord> actual, int totalRecords) = await service.GetApplicationMedia(
                "TestApp",
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                1,
                totalRecords);
            Assert.AreEqual(
                "TestMedia",
                actual[0].Name);
        }

        /// <summary>
        /// Checks whether the GetApplicationMedia method returns an empty list and zero count when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationMediaEmpty()
        {
            MediaService service = CreateService();

            (List<MediaRecord> actual, int totalRecords) = await service.GetApplicationMedia(
                "NonExistent",
                10,
                1);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        /// <summary>
        /// Checks whether the GetApplicationEntityMedia method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMedia()
        {
            InsertMediaPrerequisites(
                "Portfolio",
                ".png",
                "image/png",
                "https://example.com");

            int mediaId = InsertMedia(
                "Portfolio",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                    "SELECT PIT.PortfolioItemTypeId, 'TestItem', 'Summary', 'Description', 'icon.png', 'Notes', 'https://github.com', GETUTCDATE(), GETUTCDATE(), 0 " +
                    "FROM PortfolioItemType PIT WHERE PIT.[Name] = 'Web Application'; " +
                    "DECLARE @itemId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO PortfolioItemImage (PortfolioItemId, MediaId) VALUES (@itemId, @mediaId);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@mediaId",
                        mediaId);
                    cmd.ExecuteNonQuery();
                }

                int portfolioItemId;
                using (SqlCommand cmd = new(
                    "SELECT TOP 1 PortfolioItemId FROM PortfolioItem",
                    conn))
                {
                    portfolioItemId = (int)cmd.ExecuteScalar();
                }

                MediaService service = CreateService();

                List<MediaRecord> actual = await service.GetApplicationEntityMedia(
                    "Portfolio",
                    portfolioItemId);

                Assert.AreEqual(
                    1,
                    actual.Count);
                Assert.AreEqual(
                    "TestMedia",
                    actual[0].Name);
            }
        }

        /// <summary>
        /// Checks whether the GetApplicationEntityMedia method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMediaEmpty()
        {
            MediaService service = CreateService();

            List<MediaRecord> actual = await service.GetApplicationEntityMedia(
                "Portfolio",
                999);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Checks whether the GetMediaId method returns a single record when found.
        /// </summary>
        [TestMethod]
        public async Task TestGetMediaId()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            MediaRecord actual = await service.GetMediaId(mediaId);

            Assert.IsNotNull(actual);
            Assert.AreEqual(
                "TestMedia",
                actual.Name);
        }

        /// <summary>
        /// Checks whether the GetMediaId method returns null when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetMediaIdEmpty()
        {
            MediaService service = CreateService();

            MediaRecord actual = await service.GetMediaId(999);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Checks whether the MediaExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsByName()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            bool actual = await service.MediaExists(
                "TestApp",
                "TestMedia");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsByNameNotFound()
        {
            MediaService service = CreateService();

            bool actual = await service.MediaExists(
                "TestApp",
                "TestMedia");

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsById()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            bool actual = await service.MediaExists(mediaId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsByIdNotFound()
        {
            MediaService service = CreateService();

            bool actual = await service.MediaExists(999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaTypeCreated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaTypeCreated()
        {
            MediaService service = CreateService();

            bool actual = await service.MediaTypeCreated(
                ".png",
                "image/png");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaTypeCreated method returns true when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaTypeCreatedNoRowsAffected()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO MediaType (Extension, MimeType) VALUES ('.png', 'image/png')",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            MediaService service = CreateService();

            bool actual = await service.MediaTypeCreated(
                ".png",
                "image/png");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaCreated method returns true and the media id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestMediaCreated()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");

            MediaService service = CreateService();

            MediaModel media = new()
            {
                Name = "TestMedia",
                Extension = ".png",
                MimeType = "image/png",
                Size = 1024,
                Path = "/images",
                Domain = "https://example.com"
            };

            (bool created, int mediaId) = await service.MediaCreated(
                "TestApp",
                media);

            Assert.IsTrue(created);
            Assert.IsTrue(mediaId > 0);
        }

        /// <summary>
        /// Checks whether the MediaCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestMediaCreatedNullResult()
        {
            MediaService service = CreateService();

            MediaModel media = new()
            {
                Name = "TestMedia",
                Extension = ".png",
                MimeType = "image/png",
                Size = 1024,
                Path = "/images",
                Domain = "https://nonexistent.com"
            };

            (bool created, int mediaId) = await service.MediaCreated(
                "NonExistentApp",
                media);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                mediaId);
        }

        /// <summary>
        /// Checks whether the ApplicationEntityLinkCreated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationEntityLinkCreated()
        {
            InsertMediaPrerequisites(
                "Portfolio",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "Portfolio",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Web Application'); " +
                    "INSERT INTO PortfolioItem (TypeId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                    "SELECT PIT.PortfolioItemTypeId, 'TestItem', 'Summary', 'Description', 'icon.png', 'Notes', 'https://github.com', GETUTCDATE(), GETUTCDATE(), 0 " +
                    "FROM PortfolioItemType PIT WHERE PIT.[Name] = 'Web Application';",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            int portfolioItemId;
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 PortfolioItemId FROM PortfolioItem",
                    conn))
                {
                    portfolioItemId = (int)cmd.ExecuteScalar();
                }
            }

            MediaService service = CreateService();

            bool actual = await service.ApplicationEntityLinkCreated(
                "Portfolio",
                portfolioItemId,
                mediaId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ApplicationEntityLinkCreated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationEntityLinkCreatedNoRowsAffected()
        {
            MediaService service = CreateService();

            bool actual = await service.ApplicationEntityLinkCreated(
                "Portfolio",
                999,
                999);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdated()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            bool actual = await service.MediaUpdated(
                mediaId,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048 });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns true when ClearPath is set.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdatedClearPath()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024,
                "/images");

            MediaService service = CreateService();

            bool actual = await service.MediaUpdated(
                mediaId,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048, ClearPath = true });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdatedNoRowsAffected()
        {
            MediaService service = CreateService();

            bool actual = await service.MediaUpdated(
                999,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048 });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaDeleted()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");
            int mediaId = InsertMedia(
                "TestApp",
                "TestMedia",
                ".png",
                "image/png",
                "https://example.com",
                1024);

            MediaService service = CreateService();

            bool actual = await service.MediaDeleted(mediaId);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaDeletedNoRowsAffected()
        {
            MediaService service = CreateService();

            bool actual = await service.MediaDeleted(999);

            Assert.IsFalse(actual);
        }

    }
}
