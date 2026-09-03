// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Implementations;
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using HunterIndustriesAPI.Objects.Statistics.Error;
using HunterIndustriesAPI.Objects.Statistics.Server;
using HunterIndustriesAPI.Objects.Statistics.Shared;
using DashboardTopBarStatRecord = HunterIndustriesAPI.Objects.Statistics.Dashboard.TopBarStatRecord;
using HunterIndustriesAPI.Objects.Statistics.Portfolio;
using PortfolioTopBarStatRecord = HunterIndustriesAPI.Objects.Statistics.Portfolio.TopBarStatRecord;
using HunterIndustriesAPI.PersistenceTests.API.Helpers;
using HunterIndustriesAPI.Services;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.PersistenceTests.API.Services
{
    [TestClass]
    [DoNotParallelize]
    public class StatisticServiceTest
    {
        private static string _ConnectionString;
        private static string _DatabaseName;
        private static string _SqlFilesPath;

        private readonly Mock<ILoggerService> _MockLogger = new();
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
        }

        /// <summary>
        /// Creates a service instance with real database dependencies for testing.
        /// </summary>
        private StatisticService CreateService()
        {
            Mock<IDatabaseOptions> mockOptions = new();
            mockOptions.Setup(o => o.ConnectionString)
                .Returns(_ConnectionString);
            mockOptions.Setup(o => o.SQLFiles)
                .Returns(_SqlFilesPath);

            DatabaseWrapper database = new(mockOptions.Object);

            StatisticService service = new(
                _MockLogger.Object,
                _FileSystem,
                mockOptions.Object,
                database);

            return service;
        }

        /// <summary>
        /// Inserts an audit history record into the database.
        /// </summary>
        private void InsertAuditHistory(
            int endpointId,
            int methodId,
            int statusId,
            DateTime dateOccured,
            int? applicationId = null,
            int? userId = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (UserId, ApplicationId, EndpointId, EndpointVersionId, MethodId, StatusId, IPAddress, DateOccured) " +
                    "VALUES (@userId, @applicationId, @endpointId, 1, @methodId, @statusId, '127.0.0.1', @dateOccured)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@applicationId",
                        (object)applicationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@endpointId",
                        endpointId);
                    cmd.Parameters.AddWithValue(
                        "@methodId",
                        methodId);
                    cmd.Parameters.AddWithValue(
                        "@statusId",
                        statusId);
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts an audit history record and returns the generated ID.
        /// </summary>
        private int InsertAuditHistoryAndGetId(
            int endpointId,
            int methodId,
            int statusId,
            DateTime dateOccured,
            int? applicationId = null,
            int? userId = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO AuditHistory (UserId, ApplicationId, EndpointId, EndpointVersionId, MethodId, StatusId, IPAddress, DateOccured) " +
                    "VALUES (@userId, @applicationId, @endpointId, 1, @methodId, @statusId, '127.0.0.1', @dateOccured); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@applicationId",
                        (object)applicationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@endpointId",
                        endpointId);
                    cmd.Parameters.AddWithValue(
                        "@methodId",
                        methodId);
                    cmd.Parameters.AddWithValue(
                        "@statusId",
                        statusId);
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Inserts an error log record into the database.
        /// </summary>
        private void InsertErrorLog(
            string ipAddress,
            string summary,
            DateTime dateOccured)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ErrorLog (DateOccured, IPAddress, Summary, [Message]) VALUES (@dateOccured, @ipAddress, @summary, 'Test error message')",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.Parameters.AddWithValue(
                        "@ipAddress",
                        ipAddress);
                    cmd.Parameters.AddWithValue(
                        "@summary",
                        summary);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a login attempt record into the database.
        /// </summary>
        private void InsertLoginAttempt(
            int auditId,
            DateTime dateOccured,
            bool isSuccessful,
            int? userId = null,
            int? phraseId = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO LoginAttempt (UserId, PhraseId, AuditId, DateOccured, IsSuccessful) " +
                    "VALUES (@userId, @phraseId, @auditId, @dateOccured, @isSuccessful)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@phraseId",
                        (object)phraseId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@auditId",
                        auditId);
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.Parameters.AddWithValue(
                        "@isSuccessful",
                        isSuccessful);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a change record into the database.
        /// </summary>
        private void InsertChange(
            int auditId,
            string field)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Change] (AuditId, Field, OldValue, NewValue) VALUES (@auditId, @field, 'old', 'new')",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@auditId",
                        auditId);
                    cmd.Parameters.AddWithValue(
                        "@field",
                        field);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts server information and returns the generated ID.
        /// </summary>
        private int InsertServerInformation(
            string name,
            bool isActive)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                int machineId;

                using (SqlCommand cmd = new(
                    "INSERT INTO Machine (HostName, IsDeleted) VALUES ('TestHost', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    machineId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int gameId;

                using (SqlCommand cmd = new(
                    "INSERT INTO Game ([Name], [Version], IsDeleted) VALUES ('TestGame', '1.0', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    gameId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int connectionId;

                using (SqlCommand cmd = new(
                    "INSERT INTO Connection (IPAddress, Port, IsDeleted) VALUES ('127.0.0.1', 25565, 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    connectionId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int serverId;

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerInformation (MachineId, GameId, ConnectionId, [Name], EventInterval, WebhookURL, RecipientId, IsActive) " +
                    "VALUES (@machineId, @gameId, @connectionId, @name, 300, 'http://test.com', 1, @isActive); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@machineId",
                        machineId);
                    cmd.Parameters.AddWithValue(
                        "@gameId",
                        gameId);
                    cmd.Parameters.AddWithValue(
                        "@connectionId",
                        connectionId);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    cmd.Parameters.AddWithValue(
                        "@isActive",
                        isActive);
                    serverId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return serverId;
            }
        }

        /// <summary>
        /// Inserts a component information record into the database.
        /// </summary>
        private void InsertComponentInformation(
            int serverId,
            int componentId,
            int componentStatusId,
            DateTime dateOccured)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ComponentInformation (ServerInformationId, ComponentId, ComponentStatusId, DateOccured) " +
                    "VALUES (@serverId, @componentId, @componentStatusId, @dateOccured)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@serverId",
                        serverId);
                    cmd.Parameters.AddWithValue(
                        "@componentId",
                        componentId);
                    cmd.Parameters.AddWithValue(
                        "@componentStatusId",
                        componentStatusId);
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts prerequisite records for alert testing and returns the user setting ID.
        /// </summary>
        private int InsertUserSettingForAlert()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                int apiUserId;

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('alertuser', 'pass', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    apiUserId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int phraseId;

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('alertphrase', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    phraseId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int applicationId;

                using (SqlCommand cmd = new(
                    "INSERT INTO Application (PhraseId, [Name], IsDeleted) VALUES (@phraseId, 'AlertApp', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@phraseId",
                        phraseId);
                    applicationId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int userSettingId;

                using (SqlCommand cmd = new(
                    "INSERT INTO UserSetting (UserId, ApplicationId, [Name], [Value]) VALUES (@userId, @applicationId, 'Reporter', 'TestReporter'); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@userId",
                        apiUserId);
                    cmd.Parameters.AddWithValue(
                        "@applicationId",
                        applicationId);
                    userSettingId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return userSettingId;
            }
        }

        /// <summary>
        /// Inserts a server alert record into the database.
        /// </summary>
        private void InsertServerAlert(
            int serverId,
            int userSettingId,
            int componentId,
            int componentStatusId,
            int alertStatusId,
            DateTime dateOccured)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO ServerAlert (ServerInformationId, UserSettingId, ComponentId, ComponentStatusId, AlertStatusId, DateOccured) " +
                    "VALUES (@serverId, @userSettingId, @componentId, @componentStatusId, @alertStatusId, @dateOccured)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@serverId",
                        serverId);
                    cmd.Parameters.AddWithValue(
                        "@userSettingId",
                        userSettingId);
                    cmd.Parameters.AddWithValue(
                        "@componentId",
                        componentId);
                    cmd.Parameters.AddWithValue(
                        "@componentStatusId",
                        componentStatusId);
                    cmd.Parameters.AddWithValue(
                        "@alertStatusId",
                        alertStatusId);
                    cmd.Parameters.AddWithValue(
                        "@dateOccured",
                        dateOccured);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a portfolio item record and returns the generated ID.
        /// </summary>
        private int InsertPortfolioItem(
            string name,
            int? llmModelId = null)
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                int typeId;

                using (SqlCommand cmd = new(
                    "IF NOT EXISTS (SELECT 1 FROM PortfolioItemType WHERE [Name] = 'Application') " +
                    "INSERT INTO PortfolioItemType ([Name]) VALUES ('Application'); " +
                    "SELECT PortfolioItemTypeId FROM PortfolioItemType WHERE [Name] = 'Application';",
                    conn))
                {
                    typeId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int itemId;

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioItem (TypeId, LLMModelId, [Name], Summary, [Description], IconURL, ReleaseNotes, GitHubLink, DateCreated, DateUpdated, IsDeleted) " +
                    "VALUES (@typeId, @llmModelId, @name, 'Summary', 'Description', 'http://icon.com', 'Notes', 'http://github.com', GETUTCDATE(), GETUTCDATE(), 0); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@typeId",
                        typeId);
                    cmd.Parameters.AddWithValue(
                        "@llmModelId",
                        (object)llmModelId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
                        "@name",
                        name);
                    itemId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return itemId;
            }
        }
        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatistic()
        {
            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('testphrase', 0); " +
                    "INSERT INTO [Application] (PhraseId, [Name], IsDeleted) VALUES (SCOPE_IDENTITY(), 'TestApp', 0); " +
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('testuser', 'pass', 0);",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("topBarStats");

            Assert.AreEqual(
                1,
                records.Count);

            DashboardTopBarStatRecord record = (DashboardTopBarStatRecord)records[0];

            Assert.AreEqual(
                1,
                record.Applications);
            Assert.AreEqual(
                1,
                record.Users);
            Assert.IsTrue(
                record.Calls.ThisMonth >= 1);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticUnknown()
        {
            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns API traffic records.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticApiTraffic()
        {
            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);
            InsertAuditHistory(
                2,
                1,
                4,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("apiTraffic");

            Assert.IsTrue(
                records.Count >= 1);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns error records grouped by IP and summary.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticErrors()
        {
            InsertErrorLog(
                "192.168.1.1",
                "NullReferenceException",
                DateTime.UtcNow);
            InsertErrorLog(
                "192.168.1.1",
                "NullReferenceException",
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("errors");

            Assert.AreEqual(
                1,
                records.Count);

            IPAndSummaryErrorRecord record = (IPAndSummaryErrorRecord)records[0];

            Assert.AreEqual(
                "192.168.1.1",
                record.IPAddress);
            Assert.AreEqual(
                2,
                record.Errors);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns login attempt records.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticLoginAttempts()
        {
            int auditId = InsertAuditHistoryAndGetId(
                1,
                2,
                1,
                DateTime.UtcNow);
            InsertLoginAttempt(
                auditId,
                DateTime.UtcNow,
                true);

            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("loginAttempts");

            Assert.AreEqual(
                1,
                records.Count);

            LoginAttemptStatisticRecord record = (LoginAttemptStatisticRecord)records[0];

            Assert.AreEqual(
                1,
                record.SuccessfulAttempts);
            Assert.AreEqual(
                1,
                record.TotalAttempts);
        }

        /// <summary>
        /// Checks whether the GetDashboardStatistic method returns server health records.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticServerHealth()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            InsertComponentInformation(
                serverId,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetDashboardStatistic("serverHealth");

            Assert.IsTrue(
                records.Count >= 1);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatistic()
        {
            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);
            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic("endpointCalls");

            Assert.AreEqual(
                1,
                records.Count);

            EndpointCallRecord record = (EndpointCallRecord)records[0];

            Assert.AreEqual(
                2,
                record.Calls);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records when filtered by application.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticApplication()
        {
            int applicationId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Authorisation (Phrase, IsDeleted) VALUES ('phrase1', 0); " +
                    "INSERT INTO [Application] (PhraseId, [Name], IsDeleted) VALUES (SCOPE_IDENTITY(), 'App1', 0); " +
                    "SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    applicationId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow,
                applicationId: applicationId);
            InsertAuditHistory(
                2,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic(
                "endpointCalls",
                "application",
                applicationId);

            Assert.AreEqual(
                1,
                records.Count);

            EndpointCallRecord record = (EndpointCallRecord)records[0];

            Assert.AreEqual(
                1,
                record.Calls);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns the correct records when filtered by user.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticUser()
        {
            int userId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO APIUser (Username, [Password], IsDeleted) VALUES ('filteruser', 'pass', 0); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow,
                userId: userId);
            InsertAuditHistory(
                2,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic(
                "endpointCalls",
                "user",
                userId);

            Assert.AreEqual(
                1,
                records.Count);

            EndpointCallRecord record = (EndpointCallRecord)records[0];

            Assert.AreEqual(
                1,
                record.Calls);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticUnknown()
        {
            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns method call records.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticMethodCalls()
        {
            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);
            InsertAuditHistory(
                2,
                2,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic("methodCalls");

            Assert.AreEqual(
                2,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns status call records.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticStatusCalls()
        {
            InsertAuditHistory(
                1,
                1,
                1,
                DateTime.UtcNow);
            InsertAuditHistory(
                2,
                1,
                4,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic("statusCalls");

            Assert.AreEqual(
                2,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetSharedStatistic method returns change call records.
        /// </summary>
        [TestMethod]
        public async Task TestGetSharedStatisticChangeCalls()
        {
            int auditId = InsertAuditHistoryAndGetId(
                1,
                3,
                1,
                DateTime.UtcNow);
            InsertChange(
                auditId,
                "Name");
            InsertChange(
                auditId,
                "Name");

            StatisticService service = CreateService();

            List<object> records = await service.GetSharedStatistic("changeCalls");

            Assert.AreEqual(
                1,
                records.Count);

            ChangeCallRecord record = (ChangeCallRecord)records[0];

            Assert.AreEqual(
                "Name",
                record.Field);
            Assert.AreEqual(
                2,
                record.Calls);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatistic()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            int userSettingId = InsertUserSettingForAlert();

            InsertServerAlert(
                serverId,
                userSettingId,
                1, 2, 1, DateTime.UtcNow);
            InsertServerAlert(
                serverId,
                userSettingId,
                1, 2, 1, DateTime.UtcNow);
            InsertServerAlert(
                serverId,
                userSettingId,
                2, 2, 1, DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "componentAlerts",
                serverId);

            Assert.AreEqual(
                2,
                records.Count);

            AlertComponentRecord record = (AlertComponentRecord)records[0];

            Assert.AreEqual(
                "PC",
                record.Component);
            Assert.AreEqual(
                2,
                record.Alerts);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticUnknown()
        {
            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "unknown",
                1);

            Assert.AreEqual(
                0,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns alert status records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticStatusAlerts()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            int userSettingId = InsertUserSettingForAlert();

            InsertServerAlert(
                serverId,
                userSettingId,
                1, 2, 1, DateTime.UtcNow);
            InsertServerAlert(
                serverId,
                userSettingId,
                1, 2, 2, DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "statusAlerts",
                serverId);

            Assert.AreEqual(
                2,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns recent alert records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticRecentAlerts()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            int userSettingId = InsertUserSettingForAlert();

            InsertServerAlert(
                serverId,
                userSettingId,
                1, 2, 1, DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "recentAlerts",
                serverId);

            Assert.AreEqual(
                1,
                records.Count);

            RecentAlertRecord record = (RecentAlertRecord)records[0];

            Assert.AreEqual(
                "TestReporter",
                record.Reporter);
            Assert.AreEqual(
                "PC",
                record.Component);
            Assert.AreEqual(
                "Offline",
                record.ComponentStatus);
            Assert.AreEqual(
                "Reported",
                record.AlertStatus);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns recent event records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticRecentEvents()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            InsertComponentInformation(
                serverId,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "recentEvents",
                serverId);

            Assert.AreEqual(
                1,
                records.Count);

            EventComponentRecord record = (EventComponentRecord)records[0];

            Assert.AreEqual(
                "PC",
                record.Component);
            Assert.AreEqual(
                "Online",
                record.Status);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns last event per component records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticLastComponentEvents()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            InsertComponentInformation(
                serverId,
                1,
                1,
                DateTime.UtcNow.AddMinutes(-5));
            InsertComponentInformation(
                serverId,
                1,
                2,
                DateTime.UtcNow);
            InsertComponentInformation(
                serverId,
                2,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "lastComponentEvents",
                serverId);

            Assert.AreEqual(
                2,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetServerStatistic method returns server health records.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticServerHealth()
        {
            int serverId = InsertServerInformation(
                "TestServer",
                true);
            InsertComponentInformation(
                serverId,
                1,
                1,
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetServerStatistic(
                "serverHealth",
                serverId);

            Assert.IsTrue(
                records.Count >= 1);
        }

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatistic()
        {
            InsertErrorLog(
                "192.168.1.1",
                "TestError",
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetErrorStatistic("errorsOverTime");

            Assert.IsTrue(
                records.Count >= 1);
        }

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticUnknown()
        {
            StatisticService service = CreateService();

            List<object> records = await service.GetErrorStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns errors grouped by IP address.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticIpErrors()
        {
            InsertErrorLog(
                "192.168.1.1",
                "Error1",
                DateTime.UtcNow);
            InsertErrorLog(
                "192.168.1.1",
                "Error2",
                DateTime.UtcNow);
            InsertErrorLog(
                "10.0.0.1",
                "Error1",
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetErrorStatistic("ipErrors");

            Assert.AreEqual(
                2,
                records.Count);

            IPErrorRecord record = (IPErrorRecord)records[0];

            Assert.AreEqual(
                "192.168.1.1",
                record.IPAddress);
            Assert.AreEqual(
                2,
                record.Errors);
        }

        /// <summary>
        /// Checks whether the GetErrorStatistic method returns errors grouped by summary.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticSummaryErrors()
        {
            InsertErrorLog(
                "192.168.1.1",
                "NullReferenceException",
                DateTime.UtcNow);
            InsertErrorLog(
                "10.0.0.1",
                "NullReferenceException",
                DateTime.UtcNow);
            InsertErrorLog(
                "10.0.0.1",
                "TimeoutException",
                DateTime.UtcNow);

            StatisticService service = CreateService();

            List<object> records = await service.GetErrorStatistic("summaryErrors");

            Assert.AreEqual(
                2,
                records.Count);

            SummaryErrorRecord record = (SummaryErrorRecord)records[0];

            Assert.AreEqual(
                "NullReferenceException",
                record.Summary);
            Assert.AreEqual(
                2,
                record.Errors);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns the correct records.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatistic()
        {
            InsertPortfolioItem("TestItem");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioFilter ([Name], [Type], IsDeleted) VALUES ('TestFilter', 'tag', 0)",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("topBarStats");

            Assert.AreEqual(
                1,
                records.Count);

            PortfolioTopBarStatRecord record = (PortfolioTopBarStatRecord)records[0];

            Assert.AreEqual(
                1,
                record.Items);
            Assert.AreEqual(
                1,
                record.Filters);
            Assert.AreEqual(
                0,
                record.AIUsed);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns an empty list when the part is unknown.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticUnknown()
        {
            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("unknown");

            Assert.AreEqual(
                0,
                records.Count);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns top five viewed items.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticTopFiveViewed()
        {
            int itemId = InsertPortfolioItem("ViewedItem");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO PortfolioItemMetric (PortfolioItemId, SummaryViews, FullDetailViews) VALUES (@itemId, 10, 5)",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("topFiveViewed");

            Assert.AreEqual(
                1,
                records.Count);

            TopFiveViewedItemsRecord record = (TopFiveViewedItemsRecord)records[0];

            Assert.AreEqual(
                "ViewedItem",
                record.Name);
            Assert.AreEqual(
                10,
                record.SummaryViews);
            Assert.AreEqual(
                5,
                record.FullDetailViews);
            Assert.AreEqual(
                15,
                record.TotalViews);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns top five frameworks.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticTopFiveFrameworks()
        {
            int itemId = InsertPortfolioItem("FrameworkItem");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO Framework ([Name]) VALUES ('ASP.NET'); " +
                    "DECLARE @frameworkId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO PortfolioItemFramework (PortfolioItemId, FrameworkId) VALUES (@itemId, @frameworkId);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("topFiveFrameworks");

            Assert.AreEqual(
                1,
                records.Count);

            TopFiveRecord record = (TopFiveRecord)records[0];

            Assert.AreEqual(
                "ASP.NET",
                record.Name);
            Assert.AreEqual(
                1,
                record.Uses);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns top five languages.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticTopFiveLanguages()
        {
            int itemId = InsertPortfolioItem("LanguageItem");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Language] ([Name]) VALUES ('C#'); " +
                    "DECLARE @languageId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO PortfolioItemLanguage (PortfolioItemId, LanguageId) VALUES (@itemId, @languageId);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("topFiveLanguages");

            Assert.AreEqual(
                1,
                records.Count);

            TopFiveRecord record = (TopFiveRecord)records[0];

            Assert.AreEqual(
                "C#",
                record.Name);
            Assert.AreEqual(
                1,
                record.Uses);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns top five environments.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticTopFiveEnvironments()
        {
            int itemId = InsertPortfolioItem("EnvironmentItem");

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    "INSERT INTO [Environment] ([Name]) VALUES ('Production'); " +
                    "DECLARE @environmentId INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO PortfolioItemEnvironment (PortfolioItemId, EnvironmentId) VALUES (@itemId, @environmentId);",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@itemId",
                        itemId);
                    cmd.ExecuteNonQuery();
                }
            }

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("topFiveEnvironments");

            Assert.AreEqual(
                1,
                records.Count);

            TopFiveRecord record = (TopFiveRecord)records[0];

            Assert.AreEqual(
                "Production",
                record.Name);
            Assert.AreEqual(
                1,
                record.Uses);
        }

        /// <summary>
        /// Checks whether the GetPortfolioStatistic method returns LLM used records.
        /// </summary>
        [TestMethod]
        public async Task TestGetPortfolioStatisticLlmUsed()
        {
            int llmModelId;

            using (SqlConnection conn = new(_ConnectionString))
            {
                conn.Open();

                int companyId;

                using (SqlCommand cmd = new(
                    "INSERT INTO LLMCompany ([Name]) VALUES ('Anthropic'); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    companyId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new(
                    "INSERT INTO LLMModel (LLMCompanyId, [Name]) VALUES (@companyId, 'Claude'); SELECT SCOPE_IDENTITY();",
                    conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@companyId",
                        companyId);
                    llmModelId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            InsertPortfolioItem(
                "LLMItem",
                llmModelId);

            StatisticService service = CreateService();

            List<object> records = await service.GetPortfolioStatistic("llmUsed");

            Assert.AreEqual(
                1,
                records.Count);

            LLMUsedRecord record = (LLMUsedRecord)records[0];

            Assert.AreEqual(
                "Anthropic",
                record.Company);
            Assert.AreEqual(
                "Claude",
                record.Model);
            Assert.AreEqual(
                1,
                record.Uses);
        }

    }
}
