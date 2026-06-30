// Copyright © - 29/06/2026 - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Models.Requests.Filters.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPICommon.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.Tests.API.Controllers.Media
{
    [TestClass]
    public class MediaControllerTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IFileSystem> _MockFileSystem = new();
        private readonly Mock<IDatabaseOptions> _MockOptions = new();
        private readonly Mock<IClock> _MockClock = new();

        [TestInitialize]
        public void Setup()
        {
            _MockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("select 1");
            _MockOptions.Setup(o => o.ConnectionString)
                .Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _MockOptions.Setup(o => o.SQLFiles)
                .Returns("C:\\SQLFiles");
            _MockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://localhost", null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        #region GetApplicationMedia

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of media records.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationMedia()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<MediaRecord>
                    {
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
                            DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                            DateUpdated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false
                        }
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    5,
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/TestApp")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                new MediaFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationMediaEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<MediaRecord>(),
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/TestApp")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                new MediaFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetApplicationEntityMedia

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with media records for an application entity.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMedia()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<MediaRecord>
                    {
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
                            DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                            DateUpdated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false
                        }
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/TestApp/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no entity media is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMediaEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<MediaRecord>(),
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/TestApp/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetMediaId

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the media record.
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
                        DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        DateUpdated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no media is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetMediaIdNotFound()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, MediaRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (MediaRecord)null,
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.1/media/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a media record is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int>(),
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));
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
                        DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        DateUpdated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.1/media/TestApp/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                "TestApp",
                1,
                new MediaModel
                {
                    Name = "TestMedia",
                    Extension = ".png",
                    MimeType = "image/png",
                    Size = 1024,
                    Path = "/images",
                    Domain = "https://example.com"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.Created,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.1/media/TestApp/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                "TestApp",
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 status code when a record already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPostAlreadyExists()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int> { 1 },
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.1/media/TestApp/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                "TestApp",
                1,
                new MediaModel
                {
                    Name = "TestMedia",
                    Extension = ".png",
                    MimeType = "image/png",
                    Size = 1024,
                    Path = "/images",
                    Domain = "https://example.com"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the record is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            Mock<IFileSystem> mockFileSystem = new();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("update Media set\n[Name] = @name\nwhere MediaId = @mediaId");

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int> { 1 },
                    (Exception)null));
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
                        DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        DateUpdated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                        IsDeleted = false
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                mockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.1/media/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                new MediaUpdateModel
                {
                    Name = "UpdatedMedia"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int>(),
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.1/media/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new MediaUpdateModel
                {
                    Name = "UpdatedMedia"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when the body is null and no record exists.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int>(),
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.1/media/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int> { 1 },
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    new Uri("https://localhost/v2.1/media/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteNotFound()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new List<int>(),
                    (Exception)null));

            MediaController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    new Uri("https://localhost/v2.1/media/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        #endregion
    }
}
