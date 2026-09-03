// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Models.Requests.Filters.Media;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers.Media
{
    [TestClass]
    [DoNotParallelize]
    public class MediaControllerTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IClock> _MockClock = new();
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

            _MockClock.Setup(c => c.DefaultDate)
                .Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://localhost", null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        /// <summary>
        /// Creates a controller instance with real database dependencies for testing.
        /// </summary>
        private MediaController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new MediaController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object);
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
                    "INSERT INTO Domain (Host, IsDeleted) VALUES (@host, 0);", conn))
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
                        (object)path ?? DBNull.Value);
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
        /// Checks whether the Get method returns a 200 status code with a list of media records.
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
                1024,
                "/images");

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.1/media/TestApp"));
            controller.Configuration = new HttpConfiguration();

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.1/media/TestApp"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                new MediaFilterModel());

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with media records for an application entity.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplicationEntityMedia()
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
                1024,
                "/images");

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.1/media/TestApp/1"));
            controller.Configuration = new HttpConfiguration();

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.1/media/TestApp/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(
                "TestApp",
                999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the media record.
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
                1024,
                "/images");

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri($"https://localhost/v2.1/media/{mediaId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(mediaId);

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri("https://localhost/v2.1/media/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a media record is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            InsertMediaPrerequisites(
                "TestApp",
                ".png",
                "image/png",
                "https://example.com");

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.1/media/TestApp/1"));
            controller.Configuration = new HttpConfiguration();

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.1/media/TestApp/1"));
            controller.Configuration = new HttpConfiguration();

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
                1024,
                "/images");

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri("https://localhost/v2.1/media/TestApp/1"));
            controller.Configuration = new HttpConfiguration();

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

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the record is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
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

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri($"https://localhost/v2.1/media/{mediaId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                mediaId,
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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.1/media/999"));
            controller.Configuration = new HttpConfiguration();

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                new Uri("https://localhost/v2.1/media/1"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
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

            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri($"https://localhost/v2.1/media/{mediaId}"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(mediaId);

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
            MediaController controller = CreateController();
            controller.Request = new HttpRequestMessage(
                HttpMethod.Delete,
                new Uri("https://localhost/v2.1/media/999"));
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
