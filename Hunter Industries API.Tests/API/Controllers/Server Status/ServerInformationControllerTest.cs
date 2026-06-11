// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.ServerStatus;
using HunterIndustriesAPI.Models.Requests.Bodies.ServerStatus;
using HunterIndustriesAPI.Objects.ServerStatus;
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

namespace HunterIndustriesAPI.Tests.API.Controllers.ServerStatus
{
    [TestClass]
    public class ServerInformationControllerTest
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
        /// Checks whether the Get method returns a 200 status code with a list of servers.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [
                        new ServerInformationRecord
                        {
                            Id = 1,
                            Name = "Test",
                            HostName = "TestServer",
                            Game = "Minecraft",
                            GameVersion = "1.7.10",
                            Connection = new ConnectionRecord { IPAddress = "127.0.0.1", Port = 25565 },
                            Downtime = new DowntimeRecord { Time = "02:00:00", Duration = 60 },
                            IsActive = true
                        }
                    ],
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverinformation")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(true);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with information when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetEmpty()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    "1",
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverinformation")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Get(true);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        #endregion

        #region Post

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a server is added.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverinformation")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(new ServerInformationModel
            {
                Name = "Test",
                HostName = "TestServer",
                Game = "Minecraft",
                GameVersion = "1.7.10",
                IPAddress = "127.0.0.1",
                Port = 25565,
                Time = "02:00:00",
                Duration = 60
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
                    "1",
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.1/serverstatus/serverinformation")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Post(null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        #endregion

        #region Patch

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the server is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
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
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    (Exception)null))
                .Returns((
                    [1],
                    (Exception)null));
            mockDatabase.Setup(d => d.QuerySingle(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, ServerInformationRecord>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    new ServerInformationRecord
                    {
                        Id = 1,
                        Name = "Test",
                        HostName = "TestServer",
                        Game = "Minecraft",
                        GameVersion = "1.7.10",
                        Connection = new ConnectionRecord { IPAddress = "127.0.0.1", Port = 25565 },
                        Downtime = new DowntimeRecord { Time = "02:00:00", Duration = 60 },
                        IsActive = false
                    },
                    null));
            mockDatabase.Setup(d => d.Execute(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/serverstatus/serverinformation/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the model is invalid.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/serverstatus/serverinformation/1")),
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
        /// Checks whether the Patch method returns a 404 status code when no server exists with the given id.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNotFound()
        {
            Mock<IDatabase> mockDatabase = new();
            mockDatabase.Setup(d => d.ExecuteScalar(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    1,
                    null));
            mockDatabase.Setup(d => d.Query(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [],
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/serverstatus/serverinformation/999")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                999,
                new ServerUpdateModel
                {
                    IsActive = true
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when a server with the name already exists.
        /// </summary>
        [TestMethod]
        public async Task TestPatchNameExists()
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
                    It.IsAny<Func<SqlDataReader, int>>(),
                    It.IsAny<SqlParameter[]>()).Result)
                .Returns((
                    [1],
                    null));

            ServerInformationController controller = new(
                _MockLogger.Object,
                _MockFileSystem.Object,
                mockDatabase.Object,
                _MockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/serverstatus/serverinformation/1")),
                Configuration = new HttpConfiguration()
            };

            IHttpActionResult actionResult = await controller.Patch(
                1,
                new ServerUpdateModel
                {
                    Name = "ExistingServer"
                });

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        #endregion
    }
}
