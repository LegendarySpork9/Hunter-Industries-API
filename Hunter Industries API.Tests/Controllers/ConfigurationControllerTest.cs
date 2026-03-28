// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using HunterIndustriesAPI.Objects.Configuration;
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
using Newtonsoft.Json.Linq;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.Tests.Controllers
{
    [TestClass]
    public class ConfigurationControllerTest
    {
        private readonly Mock<ILoggerService> _mockLogger = new Mock<ILoggerService>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();
        private readonly Mock<IDatabaseOptions> _mockOptions = new Mock<IDatabaseOptions>();
        private readonly Mock<IClock> _mockClock = new Mock<IClock>();

        [TestInitialize]
        public void Setup()
        {
            _mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("select 1");
            _mockOptions.Setup(o => o.ConnectionString).Returns("Server=.;Database=Test;Trusted_Connection=True;");
            _mockOptions.Setup(o => o.SQLFiles).Returns("C:\\SQLFiles");
            _mockClock.Setup(c => c.DefaultDate).Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            _mockClock.Setup(c => c.UtcNow).Returns(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://localhost", null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        #region Get

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the list of configuration objects.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region GetList

        /// <summary>
        /// Checks whether the Get list method returns a 200 status code with a list of records.
        /// </summary>
        [TestMethod]
        public async Task TestGetList()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object> { new ComponentRecord { Id = 1, Name = "TestComponent", IsDeleted = false } }, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((5, null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get("component");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get list method returns a 200 status code with information when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetListEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object>(), null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get("component");

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with the record.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object> { new ComponentRecord { Id = 1, Name = "TestComponent", IsDeleted = false } }, null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get("component", 1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with information when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object>(), null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get("component", 999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a record is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object> { new ComponentRecord { Id = 1, Name = "TestComponent", IsDeleted = false } }, null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost/v2.0/configuration/component")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post("component", JObject.FromObject(new ComponentModel
            {
                Name = "TestComponent"
            }));

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.Created, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPostInvalidModel()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost/v2.0/configuration/component")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post("component", null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.BadRequest, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 200 status code when a record already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPostAlreadyExists()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost/v2.0/configuration/component")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post("component", JObject.FromObject(new ComponentModel
            {
                Name = "TestComponent"
            }));

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the record is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            Mock<IFileSystem> mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>())).Returns("update Component set\n[Name] = @Name\nwhere ComponentId = @ComponentId");

            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, object>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<object> { new ComponentRecord { Id = 1, Name = "TestComponent", IsDeleted = false } }, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, (Exception)null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch("component", 1, JObject.FromObject(new ComponentModel
            {
                Name = "UpdatedComponent"
            }));

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch("component", 999, JObject.FromObject(new ComponentModel
            {
                Name = "UpdatedComponent"
            }));

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.NotFound, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://localhost/v2.0/configuration/component/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch("component", 1, null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.BadRequest, contentResult.StatusCode);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int> { 1 }, null));
            _mockDatabase.Setup(d => d.Execute(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, new Uri("https://localhost/v2.0/configuration/component/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete("component", 1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 404 status code when no record exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteNotFound()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();
            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((new List<int>(), null));

            ConfigurationController controller = new ConfigurationController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object)
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, new Uri("https://localhost/v2.0/configuration/component/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Delete("component", 999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(HttpStatusCode.NotFound, contentResult.StatusCode);
        }

        #endregion
    }
}
