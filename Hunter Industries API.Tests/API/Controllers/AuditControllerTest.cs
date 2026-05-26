// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Objects;
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

namespace HunterIndustriesAPI.Tests.API.Controllers
{
    [TestClass]
    public class AuditControllerTest
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
        /// Checks whether the Get method returns a 200 status code with audit history data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            List<AuditHistoryRecord> records =
            [
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = [],
                    LoginAttempt = null,
                    Change = []
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            AuditController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/audithistory")),
                Configuration = new HttpConfiguration()
            };

            AuditHistoryFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 204 status code with an info message when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            List<AuditHistoryRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
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

            AuditController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/audithistory")),
                Configuration = new HttpConfiguration()
            };

            AuditHistoryFilterModel filters = new();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.NoContent,
                contentResult.StatusCode);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with a single audit record.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            List<AuditHistoryRecord> records =
            [
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = [],
                    LoginAttempt = null,
                    Change = []
                }
            ];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    records,
                    null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            AuditController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/audithistory")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(1);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with an info message when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            List<AuditHistoryRecord> records = [];

            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(),
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

            AuditController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/audithistory")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(999);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion
    }
}
