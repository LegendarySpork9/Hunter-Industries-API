// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Models.Requests.Filters;
using HunterIndustriesAPI.Objects;
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

namespace Hunter_Industries_API.Tests.Controllers
{
    [TestClass]
    public class AuditControllerTest
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
        /// Checks whether the Get method returns a 200 status code with audit history data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>
            {
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = new string[0],
                    LoginAttempt = null,
                    Change = new List<ChangeRecord>()
                }
            };

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            AuditController controller = new AuditController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            AuditHistoryFilterModel filters = new AuditHistoryFilterModel();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with an info message when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            AuditController controller = new AuditController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            AuditHistoryFilterModel filters = new AuditHistoryFilterModel();

            IHttpActionResult actionResult = await controller.Get(filters);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with a single audit record.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>
            {
                new AuditHistoryRecord
                {
                    Id = 1,
                    IPAddress = "127.0.0.1",
                    Endpoint = "token",
                    Method = "POST",
                    Status = "OK",
                    OccuredAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    Paramaters = new string[0],
                    LoginAttempt = null,
                    Change = new List<ChangeRecord>()
                }
            };

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((1, null));

            AuditController controller = new AuditController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(1);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with an info message when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            Mock<IDatabase> _mockDatabase = new Mock<IDatabase>();

            List<AuditHistoryRecord> records = new List<AuditHistoryRecord>();

            _mockDatabase.Setup(d => d.ExecuteScalar(It.IsAny<string>(), It.IsAny<SqlParameter[]>()).Result).Returns(("1", null));
            _mockDatabase.Setup(d => d.Query(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, AuditHistoryRecord>>(), It.IsAny<SqlParameter[]>()).Result).Returns((records, null));
            _mockDatabase.Setup(d => d.QuerySingle(It.IsAny<string>(), It.IsAny<Func<SqlDataReader, int>>(), It.IsAny<SqlParameter[]>()).Result).Returns((0, null));

            AuditController controller = new AuditController(_mockLogger.Object, _mockFileSystem.Object, _mockDatabase.Object, _mockOptions.Object, _mockClock.Object);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            IHttpActionResult actionResult = await controller.Get(999);
            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.OK, contentResult.StatusCode);
        }

        #endregion
    }
}
