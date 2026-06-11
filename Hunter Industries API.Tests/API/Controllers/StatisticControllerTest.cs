// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using HunterIndustriesAPI.Objects.Statistics.Error;
using HunterIndustriesAPI.Objects.Statistics.Server;
using HunterIndustriesAPI.Objects.Statistics.Shared;
using HunterIndustriesAPICommon.Abstractions;
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

namespace HunterIndustriesAPI.Tests.API.Controllers
{
    [TestClass]
    public class StatisticControllerTest
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

        #region GetDashboard

        /// <summary>
        /// Checks whether the GetDashboard method returns a 200 with dashboard statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboard()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.SetupSequence(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new TopBarStatRecord
                        {
                            Applications = 1,
                            Users = 2,
                            Calls = new MonthlyStatRecord
                            {
                                ThisMonth = 3,
                                LastMonth = 2
                            },
                            LoginAttempts = new MonthlyStatRecord
                            {
                                ThisMonth = 4,
                                LastMonth = 3
                            },
                            Changes = new MonthlyStatRecord
                            {
                                ThisMonth = 5,
                                LastMonth = 4
                            },
                            Errors = new MonthlyStatRecord
                            {
                                ThisMonth = 6,
                                LastMonth = 5
                            }
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new APITrafficRecord
                        {
                            Day = "Monday",
                            SuccessfulCalls = 10,
                            UnsuccessfulCalls = 2
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new IPAndSummaryErrorRecord
                        {
                            IPAddress = "127.0.0.1",
                            Summary = "Test Error",
                            Errors = 1
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 10
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new MethodCallRecord
                        {
                            Method = "GET",
                            Calls = 20
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new StatusCallRecord
                        {
                            Status = "OK",
                            Calls = 15
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new ChangeCallRecord
                        {
                            Field = "Username",
                            Calls = 5
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new LoginAttemptStatisticRecord
                        {
                            Username = "Admin",
                            Application = "TestApp",
                            SuccessfulAttempts = 10,
                            UnsuccessfulAttempts = 2,
                            TotalAttempts = 12
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new Objects.Statistics.Dashboard.ServerHealthOverviewRecord
                        {
                            ServerId = 1,
                            Name = "Test",
                            Uptime = 99.5m
                        }
                    ],
                    (Exception)null));

            StatisticController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/statistic/dashboard")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.GetDashboard();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetServer

        /// <summary>
        /// Checks whether the GetServer method returns a 200 with server statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetServer()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.SetupSequence(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new AlertComponentRecord
                        {
                            Component = "CPU",
                            Alerts = 3
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new AlertStatusRecord
                        {
                            Status = "Open",
                            Alerts = 2
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new EventComponentRecord
                        {
                            Component = "CPU",
                            Status = "Normal",
                            DateOccured = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new Objects.Statistics.Server.ServerHealthOverviewRecord
                        {
                            Day = "2024-01-01",
                            Uptime = 99.5m
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new RecentAlertRecord
                        {
                            Id = 1,
                            Reporter = "System",
                            Component = "CPU",
                            ComponentStatus = "Critical",
                            AlertStatus = "Open",
                            AlertDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new EventComponentRecord
                        {
                            Component = "Memory",
                            Status = "Warning",
                            DateOccured = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc)
                        }
                    ],
                    (Exception)null));

            StatisticController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/statistic/server/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.GetServer(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetError

        /// <summary>
        /// Checks whether the GetError method returns a 200 with error statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetError()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.SetupSequence(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new ErrorOverTimeRecord
                        {
                            Month = "January",
                            Errors = 5
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new IPErrorRecord
                        {
                            IPAddress = "127.0.0.1",
                            Errors = 3
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new SummaryErrorRecord
                        {
                            Summary = "Test Error",
                            Errors = 2
                        }
                    ],
                    (Exception)null));

            StatisticController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/statistic/error")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.GetError();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetApplication

        /// <summary>
        /// Checks whether the GetApplication method returns a 200 with application statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetApplication()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.SetupSequence(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 10
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new MethodCallRecord
                        {
                            Method = "GET",
                            Calls = 20
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new StatusCallRecord
                        {
                            Status = "OK",
                            Calls = 15
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new ChangeCallRecord
                        {
                            Field = "Username",
                            Calls = 5
                        }
                    ],
                    (Exception)null));

            StatisticController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/statistic/application/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.GetApplication(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region GetUser

        /// <summary>
        /// Checks whether the GetUser method returns a 200 with user statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetUser()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.SetupSequence(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, object>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new EndpointCallRecord
                        {
                            Endpoint = "/token",
                            Calls = 10
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new MethodCallRecord
                        {
                            Method = "GET",
                            Calls = 20
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new StatusCallRecord
                        {
                            Status = "OK",
                            Calls = 15
                        }
                    ],
                    (Exception)null))
                .Returns((
                    [
                        new ChangeCallRecord
                        {
                            Field = "Username",
                            Calls = 5
                        }
                    ],
                    (Exception)null));

            StatisticController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/statistic/user/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.GetUser(1);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion
    }
}
