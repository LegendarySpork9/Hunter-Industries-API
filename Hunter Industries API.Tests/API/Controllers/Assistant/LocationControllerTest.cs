// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Assistant;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
using HunterIndustriesAPI.Models.Responses.Assistant;
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

namespace HunterIndustriesAPI.Tests.API.Controllers.Assistant
{
    [TestClass]
    public class LocationControllerTest
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
                new HttpRequest(
                    null,
                    "http://localhost",
                    null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        #region Get

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with location data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            LocationResponseModel locationResponse = new()
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                HostName = "TestHost",
                IPAddress = "192.168.1.1"
            };

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LocationResponseModel>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    locationResponse,
                    null));

            LocationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/assistant/location")),
                Configuration = new HttpConfiguration()
            };

            AssistantFilterModel filters = new()
            {
                AssistantName = "TestAssistant",
                AssistantId = "A001"
            };

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with an info message when the assistant does not exist.
        /// </summary>
        [TestMethod]
        public async Task TestGetNotFound()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LocationResponseModel>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    null,
                    null));

            LocationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/assistant/location")),
                Configuration = new HttpConfiguration()
            };

            AssistantFilterModel filters = new()
            {
                AssistantName = "NonExistent",
                AssistantId = "X999"
            };

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the location is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            List<(string, string)> existsResults =
            [
                ("TestAssistant", "A001")
            ];

            LocationResponseModel locationResponse = new()
            {
                AssistantName = "TestAssistant",
                IdNumber = "A001",
                HostName = "OldHost",
                IPAddress = "192.168.1.1"
            };

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    existsResults,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, LocationResponseModel>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    locationResponse,
                    null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            LocationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/assistant/location")),
                Configuration = new HttpConfiguration()
            };

            AssistantFilterModel filters = new()
            {
                AssistantName = "TestAssistant",
                AssistantId = "A001"
            };

            LocationModel request = new()
            {
                HostName = "NewHost",
                IPAddress = "192.168.1.2"
            };

            IHttpActionResult actionResult = await controller.Patch(
                request,
                filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 404 status code when the assistant does not exist.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            List<(string, string)> existsResults = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, (string, string)>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    existsResults,
                    null));

            LocationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/assistant/location")),
                Configuration = new HttpConfiguration()
            };

            AssistantFilterModel filters = new()
            {
                AssistantName = "NonExistent",
                AssistantId = "X999"
            };

            LocationModel request = new()
            {
                HostName = "NewHost",
                IPAddress = "192.168.1.2"
            };

            IHttpActionResult actionResult = await controller.Patch(
                request,
                filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        #endregion
    }
}
