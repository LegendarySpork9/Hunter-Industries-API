// Copyright © - 29/06/2026 - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPI.Services.Media;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.API.Services.Media
{
    [TestClass]
    public class MediaServiceTest
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

        #region GetApplicationMedia

        /// <summary>
        /// Checks whether the GetApplicationMedia method returns the correct records and total count.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationMedia()
        {
            List<MediaRecord> records =
            [
                new MediaRecord
                {
                    Id = 1,
                    Name = "TestMedia",
                    Type = new MediaTypeRecord { Extension = ".png", MimeType = "image/png" },
                    Size = 1024,
                    Path = "/images",
                    Domain = "https://example.com",
                    URL = "https://example.com/images/TestMedia.png",
                    Application = "TestApp",
                    DateUploaded = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    DateUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    5,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (List<MediaRecord> actual, int totalRecords) = await service.GetApplicationMedia(
                "TestApp",
                10,
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                5,
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
            List<MediaRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            (List<MediaRecord> actual, int totalRecords) = await service.GetApplicationMedia(
                "TestApp",
                10,
                1);

            Assert.AreEqual(
                0,
                actual.Count);
            Assert.AreEqual(
                0,
                totalRecords);
        }

        #endregion

        #region GetApplicationEntityMedia

        /// <summary>
        /// Checks whether the GetApplicationEntityMedia method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMedia()
        {
            List<MediaRecord> records =
            [
                new MediaRecord
                {
                    Id = 1,
                    Name = "TestMedia",
                    Type = new MediaTypeRecord { Extension = ".png", MimeType = "image/png" },
                    Size = 1024,
                    Path = "/images",
                    Domain = "https://example.com",
                    URL = "https://example.com/images/TestMedia.png",
                    Application = "TestApp",
                    DateUploaded = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    DateUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<MediaRecord> actual = await service.GetApplicationEntityMedia(
                "TestApp",
                1);

            Assert.AreEqual(
                1,
                actual.Count);
            Assert.AreEqual(
                "TestMedia",
                actual[0].Name);
        }

        /// <summary>
        /// Checks whether the GetApplicationEntityMedia method returns an empty list when the database returns no results.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMediaEmpty()
        {
            List<MediaRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            List<MediaRecord> actual = await service.GetApplicationEntityMedia(
                "TestApp",
                1);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetMediaId

        /// <summary>
        /// Checks whether the GetMediaId method returns a single record when found.
        /// </summary>
        [TestMethod]
        public async Task TestGetMediaId()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new MediaRecord
                    {
                        Id = 1,
                        Name = "TestMedia",
                        Type = new MediaTypeRecord { Extension = ".png", MimeType = "image/png" },
                        Size = 1024,
                        Path = "/images",
                        Domain = "https://example.com",
                        URL = "https://example.com/images/TestMedia.png",
                        Application = "TestApp",
                        DateUploaded = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        DateUpdated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    (Exception)null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            MediaRecord actual = await service.GetMediaId(1);

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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (MediaRecord)null,
                    (Exception)null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            MediaRecord actual = await service.GetMediaId(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region MediaExists (string, string)

        /// <summary>
        /// Checks whether the MediaExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsByName()
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

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

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
            List<int> results = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    results,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaExists(
                "TestApp",
                "TestMedia");

            Assert.IsFalse(actual);
        }

        #endregion

        #region MediaExists (int)

        /// <summary>
        /// Checks whether the MediaExists method returns true when a matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsById()
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

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaExists(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaExists method returns false when no matching record is found.
        /// </summary>
        [TestMethod]
        public async Task TestMediaExistsByIdNotFound()
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

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaExists(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region MediaTypeCreated

        /// <summary>
        /// Checks whether the MediaTypeCreated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaTypeCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

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
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaTypeCreated(
                ".png",
                "image/png");

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaTypeCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestMediaTypeCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaTypeCreated(
                ".png",
                "image/png");

            Assert.IsFalse(actual);
        }

        #endregion

        #region MediaCreated

        /// <summary>
        /// Checks whether the MediaCreated method returns true and the media id when the record is created.
        /// </summary>
        [TestMethod]
        public async Task TestMediaCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            MediaModel media = new()
            {
                Name = "TestMedia",
                Extension = ".png",
                MimeType = "image/png",
                Size = 1024,
                Path = "/images",
                Domain = "https://example.com"
            };

            (bool created, int mediaId) = await service.MediaCreated("TestApp", media);

            Assert.IsTrue(created);
            Assert.AreEqual(
                1,
                mediaId);
        }

        /// <summary>
        /// Checks whether the MediaCreated method returns false when the database returns null.
        /// </summary>
        [TestMethod]
        public async Task TestMediaCreatedNullResult()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            MediaModel media = new()
            {
                Name = "TestMedia",
                Extension = ".png",
                MimeType = "image/png",
                Size = 1024,
                Path = "/images",
                Domain = "https://example.com"
            };

            (bool created, int mediaId) = await service.MediaCreated("TestApp", media);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                mediaId);
        }

        /// <summary>
        /// Checks whether the MediaCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestMediaCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    new Exception("Database error")));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            MediaModel media = new()
            {
                Name = "TestMedia",
                Extension = ".png",
                MimeType = "image/png",
                Size = 1024,
                Path = "/images",
                Domain = "https://example.com"
            };

            (bool created, int mediaId) = await service.MediaCreated("TestApp", media);

            Assert.IsFalse(created);
            Assert.AreEqual(
                0,
                mediaId);
        }

        #endregion

        #region ApplicationEntityLinkCreated

        /// <summary>
        /// Checks whether the ApplicationEntityLinkCreated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationEntityLinkCreated()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ApplicationEntityLinkCreated(
                "TestApp",
                1,
                1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the ApplicationEntityLinkCreated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationEntityLinkCreatedNoRowsAffected()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ApplicationEntityLinkCreated(
                "TestApp",
                1,
                1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the ApplicationEntityLinkCreated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestApplicationEntityLinkCreatedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.ApplicationEntityLinkCreated(
                "TestApp",
                1,
                1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region MediaUpdated

        /// <summary>
        /// Checks whether the MediaUpdated method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdated()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update Media\nset [Name] = @name\nwhere MediaId = @mediaId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaUpdated(
                1,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048 });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns true when ClearPath is set.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdatedClearPath()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update Media\nset [Name] = @name\nwhere MediaId = @mediaId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaUpdated(
                1,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048, ClearPath = true });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdatedNoRowsAffected()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update Media\nset [Name] = @name\nwhere MediaId = @mediaId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaUpdated(
                1,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048 });

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaUpdated method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestMediaUpdatedWithError()
        {
            _MockFileSystem.Setup(f => f.ReadAllText(It.IsAny<string>()))
                .Returns("update Media\nset [Name] = @name\nwhere MediaId = @mediaId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaUpdated(
                1,
                new MediaUpdateModel { Name = "UpdatedMedia", Size = 2048 });

            Assert.IsFalse(actual);
        }

        #endregion

        #region MediaDeleted

        /// <summary>
        /// Checks whether the MediaDeleted method returns true when one row is affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaDeleted()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaDeleted(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the MediaDeleted method returns false when zero rows are affected.
        /// </summary>
        [TestMethod]
        public async Task TestMediaDeletedNoRowsAffected()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    null));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaDeleted(1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the MediaDeleted method returns false when the database returns an error.
        /// </summary>
        [TestMethod]
        public async Task TestMediaDeletedWithError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    new Exception("Database error")));

            MediaService service = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                _MockOptions.Object,
                mockDatabase.Object);

            bool actual = await service.MediaDeleted(1);

            Assert.IsFalse(actual);
        }

        #endregion
    }
}
