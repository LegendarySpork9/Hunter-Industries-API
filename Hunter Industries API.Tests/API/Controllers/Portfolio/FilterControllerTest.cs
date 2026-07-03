// Copyright © - 02/07/2026 - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Portfolio;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
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

namespace HunterIndustriesAPI.Tests.API.Controllers.Portfolio
{
    [TestClass]
    public class FilterControllerTest
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

        #region Get

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with a list of filters.
        /// </summary>
        [TestMethod]
        public async Task TestGetFilters()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new FilterRecord
                        {
                            Id = 1,
                            Name = "Language",
                            Values = ["C#", "Python", "JavaScript"],
                            IsDeleted = false
                        }
                    ],
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no filters are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetFiltersEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    (Exception)null));
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)1,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a filter is created.
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
                    [],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Values = "C#,Python,JavaScript"
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

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(null);

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
                    [1],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Values = "C#,Python,JavaScript"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 500 status code when the creation fails.
        /// </summary>
        [TestMethod]
        public async Task TestPostCreationFailed()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)null,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Values = "C#,Python,JavaScript"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 500 status code when the new record could not be found.
        /// </summary>
        [TestMethod]
        public async Task TestPostIdZero()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    (object)0,
                    (Exception)null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Post,
                    new Uri("https://localhost/v2.2/portfolio/filter")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(
                new FilterModel
                {
                    Name = "Language",
                    Values = "C#,Python,JavaScript"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
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
                .Returns("update PortfolioFilter set\n\t[Name] = @name,\n\t[Values] = @values\nwhere PortfolioFilterId = @filterId");

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
                    [1],
                    (Exception)null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new FilterRecord
                    {
                        Id = 1,
                        Name = "Language",
                        Values = ["C#", "Python", "JavaScript"],
                        IsDeleted = false
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                mockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.2/portfolio/filter/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                new FilterModel
                {
                    Name = "Language Updated"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the body is invalid.
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

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.2/portfolio/filter/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
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
                    [],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.2/portfolio/filter/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new FilterModel
                {
                    Name = "Language Updated"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 500 status code when the update fails.
        /// </summary>
        [TestMethod]
        public async Task TestPatchUpdateFailed()
        {
            Mock<IFileSystem> mockFileSystem = new();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Returns("update PortfolioFilter set\n\t[Name] = @name,\n\t[Values] = @values\nwhere PortfolioFilterId = @filterId");

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
                    [1],
                    (Exception)null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, FilterRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new FilterRecord
                    {
                        Id = 1,
                        Name = "Language",
                        Values = ["C#", "Python", "JavaScript"],
                        IsDeleted = false
                    },
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                mockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    new Uri("https://localhost/v2.2/portfolio/filter/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                new FilterModel
                {
                    Name = "Language Updated"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
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
                    [1],
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    new Uri("https://localhost/v2.2/portfolio/filter/1")),
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
                    [],
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    new Uri("https://localhost/v2.2/portfolio/filter/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 500 status code when the deletion fails.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteFailed()
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
                    [1],
                    (Exception)null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    0,
                    (Exception)null));

            FilterController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Delete,
                    new Uri("https://localhost/v2.2/portfolio/filter/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.InternalServerError,
                contentResult.StatusCode);
        }

        #endregion
    }
}
