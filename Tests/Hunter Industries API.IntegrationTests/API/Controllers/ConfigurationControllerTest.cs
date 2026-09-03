// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers
{
    [TestClass]
    [DoNotParallelize]
    public class ConfigurationControllerTest
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
        private ConfigurationController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new ConfigurationController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v2.0/configuration")),
                Configuration = new HttpConfiguration()
            };
        }

        /// <summary>
        /// Inserts a machine record into the database.
        /// </summary>
        private void InsertMachine(
            string hostName,
            bool isDeleted = false)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Machine (HostName, IsDeleted) VALUES (@hostName, @isDeleted)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@isDeleted",
                        isDeleted);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks whether the Get method returns a 200 status code with the list of configuration objects.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get();

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get list method returns a 200 status code with a list of records.
        /// </summary>
        [TestMethod]
        public async Task TestGetList()
        {
            InsertMachine("TestMachine");

            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(
                "machine",
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get list method returns a 200 status code with information when no records are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetListEmpty()
        {
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(
                "machine",
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with the record.
        /// </summary>
        [TestMethod]
        public async Task TestGetById()
        {
            InsertMachine("TestMachine");

            int machineId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 MachineId FROM Machine WHERE HostName = 'TestMachine'",
                    conn))
                {
                    machineId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(
                "machine",
                machineId);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Get by id method returns a 200 status code with information when no record is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetByIdEmpty()
        {
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Get(
                "machine",
                999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.OK,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Post method returns a 201 status code when a record is created.
        /// </summary>
        [TestMethod]
        public async Task TestPost()
        {
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(
                "machine",
                JObject.FromObject(new MachineModel
            {
                HostName = "NewMachine"
            }));

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
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(
                "machine",
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
            InsertMachine("ExistingMachine");

            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Post(
                "machine",
                JObject.FromObject(new MachineModel
            {
                HostName = "ExistingMachine"
            }));

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
            InsertMachine("OriginalMachine");

            int machineId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 MachineId FROM Machine WHERE HostName = 'OriginalMachine'",
                    conn))
                {
                    machineId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(
                "machine",
                machineId, JObject.FromObject(new MachineModel
            {
                HostName = "UpdatedMachine"
            }));

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
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(
                "machine",
                999,
                JObject.FromObject(new MachineModel
            {
                HostName = "UpdatedMachine"
            }));

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Patch method returns a 400 status code when the body is null.
        /// </summary>
        [TestMethod]
        public async Task TestPatchInvalidModel()
        {
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Patch(
                "machine",
                1,
                null);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                contentResult.StatusCode);
        }

        /// <summary>
        /// Checks whether the Delete method returns a 200 status code when the record is deleted.
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            InsertMachine("DeleteMachine");

            int machineId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "SELECT TOP 1 MachineId FROM Machine WHERE HostName = 'DeleteMachine'",
                    conn))
                {
                    machineId = (int)cmd.ExecuteScalar();
                }
            }

            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Delete(
                "machine",
                machineId);

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
            ConfigurationController controller = CreateController();

            IHttpActionResult actionResult = await controller.Delete(
                "machine",
                999);

            NegotiatedContentResult<object> contentResult = actionResult as NegotiatedContentResult<object>;
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                contentResult.StatusCode);
        }
    }
}
