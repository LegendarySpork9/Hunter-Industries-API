// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Controllers.Assistant;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.IntegrationTests.API.Helpers;
using HunterIndustriesAPI.Models.Requests.Bodies.Assistant;
using HunterIndustriesAPI.Models.Requests.Filters.Assistant;
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

namespace HunterIndustriesAPI.IntegrationTests.API.Controllers.Assistant
{
    [TestClass]
    [DoNotParallelize]
    public class DeletionControllerTest
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
                new HttpRequest(
                    null,
                    "http://localhost",
                    null),
                new HttpResponse(new System.IO.StringWriter()));
        }

        /// <summary>
        /// Creates a controller instance with real database dependencies for testing.
        /// </summary>
        private DeletionController CreateController()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            return new DeletionController(
                _MockLogger.Object,
                _FileSystem,
                database,
                mockOptions.Object,
                _MockClock.Object)
            {
                Request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri("https://localhost/v1.0/assistant/deletion")),
                Configuration = new HttpConfiguration()
            };
        }

        /// <summary>
        /// Inserts test assistant data into the database.
        /// </summary>
        private void InsertAssistantData(
            string assistantName,
            string idNumber,
            string userName,
            string hostName,
            string ipAddress,
            int deletionStatusId,
            string version)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Location] (HostName, IPAddress) VALUES (@hostName, @ipAddress); " +
                    "DECLARE @locationId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO [User] ([Name]) VALUES (@userName); " +
                    "DECLARE @userId INT = SCOPE_IDENTITY(); " +
                    "DECLARE @versionId INT; " +
                    "SELECT @versionId = VersionId FROM [Version] WHERE [Value] = @version; " +
                    "IF @versionId IS NULL BEGIN " +
                    "INSERT INTO [Version] ([Value]) VALUES (@version); " +
                    "SET @versionId = SCOPE_IDENTITY(); " +
                    "END; " +
                    "INSERT INTO AssistantInformation (LocationId, DeletionStatusId, VersionId, UserId, [Name], IDNumber) " +
                    "VALUES (@locationId, @deletionStatusId, @versionId, @userId, @assistantName, @idNumber);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@hostName",
                        hostName);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@userName",
                        userName);
                    cmd.Parameters.AddWithValue(
                        "@version",
                        version);
                    cmd.Parameters.AddWithValue(
                        "@deletionStatusId",
                        deletionStatusId);
                    cmd.Parameters.AddWithValue(
                        "@assistantName",
                        assistantName);
                    cmd.Parameters.AddWithValue(
                        "@idNumber",
                        idNumber);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Checks whether the Get method returns a 200 status code with deletion data.
        /// </summary>
        [TestMethod]
        public async Task TestGet()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost",
                "192.168.1.1",
                2,
                "1.0.0");

            DeletionController controller = CreateController();

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
            DeletionController controller = CreateController();

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

        /// <summary>
        /// Checks whether the Patch method returns a 200 status code when the deletion value is updated.
        /// </summary>
        [TestMethod]
        public async Task TestPatch()
        {
            InsertAssistantData(
                "TestAssistant",
                "A001",
                "TestUser",
                "TestHost",
                "192.168.1.1",
                2,
                "1.0.0");

            DeletionController controller = CreateController();

            AssistantFilterModel filters = new()
            {
                AssistantName = "TestAssistant",
                AssistantId = "A001"
            };

            DeletionModel request = new()
            {
                Deletion = true
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
            DeletionController controller = CreateController();

            AssistantFilterModel filters = new()
            {
                AssistantName = "NonExistent",
                AssistantId = "X999"
            };

            DeletionModel request = new()
            {
                Deletion = true
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

    }
}
