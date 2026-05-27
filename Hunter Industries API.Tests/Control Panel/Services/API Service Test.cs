// Copyright (c) - Unpublished - Toby Hunter
#nullable enable
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPIControlPanel.Abstractions;
using HunterIndustriesAPIControlPanel.Models.Requests.Patch;
using HunterIndustriesAPIControlPanel.Models.Requests.Post;
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Models.Responses.Related;
using HunterIndustriesAPIControlPanel.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Tests.ControlPanel.Services
{
    [TestClass]
    public class APIServiceTest
    {
        private readonly Mock<IConfigurableLoggerService> _MockLogger = new();
        private readonly Mock<IAPIClient> _MockAPIClient = new();
        private readonly Mock<IClock> _MockClock = new();

        private APIService CreateService()
        {
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            APIService service = new(
                _MockLogger.Object,
                _MockAPIClient.Object,
                _MockClock.Object)
            {
                ExpiryTime = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            return service;
        }

        private APIService CreateServiceWithExpiredToken()
        {
            _MockClock.Setup(c => c.UtcNow)
                .Returns(new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc));

            _MockAPIClient.Setup(c => c.Authorise())
                .ReturnsAsync(new AuthenticationModel
                {
                    Type = "Bearer",
                    Token = "NewToken",
                    ExpiresIn = 3600,
                    Info = new AuthenticationInfoModel
                    {
                        ApplicationName = "TestApp",
                        Scopes = ["read", "write"],
                        Issued = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        Expires = new DateTime(2025, 1, 4, 0, 0, 0, DateTimeKind.Utc)
                    }
                });

            APIService service = new(
                _MockLogger.Object,
                _MockAPIClient.Object,
                _MockClock.Object)
            {
                ExpiryTime = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            return service;
        }

        #region Authorise

        /// <summary>
        /// Tests whether the Authorise method sets the bearer token and expiry time.
        /// </summary>
        [TestMethod]
        public async Task TestAuthorise()
        {
            DateTime expectedExpiry = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc);

            _MockAPIClient.Setup(c => c.Authorise())
                .ReturnsAsync(new AuthenticationModel
                {
                    Type = "Bearer",
                    Token = "TestToken123",
                    ExpiresIn = 3600,
                    Info = new AuthenticationInfoModel
                    {
                        ApplicationName = "TestApp",
                        Scopes = ["read"],
                        Issued = new DateTime(2025, 6, 1, 11, 0, 0, DateTimeKind.Utc),
                        Expires = expectedExpiry
                    }
                });

            APIService service = CreateService();
            await service.Authorise();

            _MockAPIClient.Verify(
                c => c.SetBearerToken("TestToken123"),
                Times.Once);

            Assert.AreEqual(
                expectedExpiry,
                service.ExpiryTime);
        }

        /// <summary>
        /// Tests whether the Authorise method handles a null response.
        /// </summary>
        [TestMethod]
        public async Task TestAuthoriseNull()
        {
            _MockAPIClient.Setup(c => c.Authorise())
                .ReturnsAsync((AuthenticationModel?)null);

            APIService service = CreateService();
            DateTime originalExpiry = service.ExpiryTime;

            await service.Authorise();

            _MockAPIClient.Verify(
                c => c.SetBearerToken(It.IsAny<string>()),
                Times.Never);

            Assert.AreEqual(
                originalExpiry,
                service.ExpiryTime);
        }

        /// <summary>
        /// Tests whether the Authorise method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestAuthoriseException()
        {
            _MockAPIClient.Setup(c => c.Authorise())
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            DateTime originalExpiry = service.ExpiryTime;

            await service.Authorise();

            Assert.AreEqual(
                originalExpiry,
                service.ExpiryTime);
        }

        #endregion

        #region GetUsers

        /// <summary>
        /// Tests whether the GetUsers method returns users.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsers()
        {
            List<UserModel> expected =
            [
                new UserModel { Id = 1, Username = "TestUser", Password = "pass", Scopes = ["read"], IsDeleted = false }
            ];

            _MockAPIClient.Setup(c => c.GetUsers(false))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            List<UserModel> actual = await service.GetUsers(false);

            Assert.AreEqual(
                1,
                actual.Count);

            Assert.AreEqual(
                "TestUser",
                actual[0].Username);
        }

        /// <summary>
        /// Tests whether the GetUsers method returns an empty list when no users are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersEmpty()
        {
            _MockAPIClient.Setup(c => c.GetUsers(false))
                .ReturnsAsync([]);

            APIService service = CreateService();
            List<UserModel> actual = await service.GetUsers(false);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Tests whether the GetUsers method returns an empty list when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersException()
        {
            _MockAPIClient.Setup(c => c.GetUsers(false))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            List<UserModel> actual = await service.GetUsers(false);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Tests whether the GetUsers method re-authorises when the token is expired.
        /// </summary>
        [TestMethod]
        public async Task TestGetUsersReauthorises()
        {
            _MockAPIClient.Setup(c => c.GetUsers(false))
                .ReturnsAsync([]);

            APIService service = CreateServiceWithExpiredToken();
            await service.GetUsers(false);

            _MockAPIClient.Verify(
                c => c.Authorise(),
                Times.Once);
        }

        #endregion

        #region GetUser

        /// <summary>
        /// Tests whether the GetUser method returns a user.
        /// </summary>
        [TestMethod]
        public async Task TestGetUser()
        {
            UserModel expected = new() { Id = 1, Username = "TestUser", Password = "pass", Scopes = ["read"], IsDeleted = false };

            _MockAPIClient.Setup(c => c.GetUser(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            UserModel? actual = await service.GetUser(1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "TestUser",
                actual.Username);
        }

        /// <summary>
        /// Tests whether the GetUser method returns null when no user is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserNull()
        {
            _MockAPIClient.Setup(c => c.GetUser(1))
                .ReturnsAsync((UserModel?)null);

            APIService service = CreateService();
            UserModel? actual = await service.GetUser(1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetUser method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserException()
        {
            _MockAPIClient.Setup(c => c.GetUser(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            UserModel? actual = await service.GetUser(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region CreateUser

        /// <summary>
        /// Tests whether the CreateUser method returns a user.
        /// </summary>
        [TestMethod]
        public async Task TestCreateUser()
        {
            UserRequestModel request = new() { Username = "NewUser", Password = "pass", Scopes = ["read"] };

            UserModel expectedUser = new() { Id = 1, Username = "NewUser", Password = "pass", Scopes = ["read"], IsDeleted = false };

            _MockAPIClient.Setup(c => c.CreateUser(request))
                .ReturnsAsync((expectedUser, (ResponseModel?)null));

            APIService service = CreateService();
            (UserModel? actual, ResponseModel? _) = await service.CreateUser(request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "NewUser",
                actual.Username);
        }

        /// <summary>
        /// Tests whether the CreateUser method returns null and the response model on failure.
        /// </summary>
        [TestMethod]
        public async Task TestCreateUserFailed()
        {
            UserRequestModel request = new() { Username = "NewUser", Password = "pass", Scopes = ["read"] };

            ResponseModel response = new() { StatusCode = HttpStatusCode.BadRequest, Message = "Username taken" };

            _MockAPIClient.Setup(c => c.CreateUser(request))
                .ReturnsAsync(((UserModel?)null, response));

            APIService service = CreateService();
            (UserModel? actualUser, ResponseModel? actualResponse) = await service.CreateUser(request);

            Assert.IsNull(actualUser);
            Assert.IsNotNull(actualResponse);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                actualResponse.StatusCode);
        }

        /// <summary>
        /// Tests whether the CreateUser method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestCreateUserException()
        {
            UserRequestModel request = new() { Username = "NewUser", Password = "pass", Scopes = ["read"] };

            _MockAPIClient.Setup(c => c.CreateUser(request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (UserModel? actualUser, ResponseModel? actualResponse) = await service.CreateUser(request);

            Assert.IsNull(actualUser);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region UpdateUser

        /// <summary>
        /// Tests whether the UpdateUser method returns the updated user.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUser()
        {
            UserUpdateRequestModel request = new() { Username = "UpdatedUser" };

            UserModel expectedUser = new() { Id = 1, Username = "UpdatedUser", Password = "pass", Scopes = ["read"], IsDeleted = false };

            _MockAPIClient.Setup(c => c.UpdateUser(1, request))
                .ReturnsAsync((expectedUser, (ResponseModel?)null));

            APIService service = CreateService();
            (UserModel? actual, ResponseModel? _) = await service.UpdateUser(1, request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "UpdatedUser",
                actual.Username);
        }

        /// <summary>
        /// Tests whether the UpdateUser method returns null and the response model on failure.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUserFailed()
        {
            UserUpdateRequestModel request = new() { Username = "UpdatedUser" };

            ResponseModel response = new() { StatusCode = HttpStatusCode.NotFound, Message = "User not found" };

            _MockAPIClient.Setup(c => c.UpdateUser(1, request))
                .ReturnsAsync(((UserModel?)null, response));

            APIService service = CreateService();
            (UserModel? actualUser, ResponseModel? actualResponse) = await service.UpdateUser(1, request);

            Assert.IsNull(actualUser);
            Assert.IsNotNull(actualResponse);

            Assert.AreEqual(
                HttpStatusCode.NotFound,
                actualResponse.StatusCode);
        }

        /// <summary>
        /// Tests whether the UpdateUser method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUserException()
        {
            UserUpdateRequestModel request = new() { Username = "UpdatedUser" };

            _MockAPIClient.Setup(c => c.UpdateUser(1, request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (UserModel? actualUser, ResponseModel? actualResponse) = await service.UpdateUser(1, request);

            Assert.IsNull(actualUser);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region DeleteUser

        /// <summary>
        /// Tests whether the DeleteUser method returns true on success.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteUser()
        {
            _MockAPIClient.Setup(c => c.DeleteUser(1))
                .ReturnsAsync(true);

            APIService service = CreateService();
            bool actual = await service.DeleteUser(1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the DeleteUser method returns false on failure.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteUserFailed()
        {
            _MockAPIClient.Setup(c => c.DeleteUser(1))
                .ReturnsAsync(false);

            APIService service = CreateService();
            bool actual = await service.DeleteUser(1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the DeleteUser method returns false when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteUserException()
        {
            _MockAPIClient.Setup(c => c.DeleteUser(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            bool actual = await service.DeleteUser(1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region GetDashboardStatistics

        /// <summary>
        /// Tests whether the GetDashboardStatistics method returns statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatistics()
        {
            DashboardStatisticsModel expected = new()
            {
                Metrics = new MetricModel
                {
                    Applications = 5,
                    Users = 10,
                    Calls = new MonthlyStatModel { ThisMonth = 100, LastMonth = 80 },
                    LoginAttempts = new MonthlyStatModel { ThisMonth = 20, LastMonth = 15 },
                    Changes = new MonthlyStatModel { ThisMonth = 30, LastMonth = 25 },
                    Errors = new MonthlyStatModel { ThisMonth = 5, LastMonth = 3 }
                },
                ApiTraffic = [],
                Errors = [],
                EndpointCalls = [],
                MethodCalls = [],
                StatusCalls = [],
                Changes = [],
                LoginAttempts = [],
                ServerHealth = []
            };

            _MockAPIClient.Setup(c => c.GetDashboardStatistics())
                .ReturnsAsync(expected);

            APIService service = CreateService();
            DashboardStatisticsModel? actual = await service.GetDashboardStatistics();

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                10,
                actual.Metrics.Users);
        }

        /// <summary>
        /// Tests whether the GetDashboardStatistics method returns null when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticsNull()
        {
            _MockAPIClient.Setup(c => c.GetDashboardStatistics())
                .ReturnsAsync((DashboardStatisticsModel?)null);

            APIService service = CreateService();
            DashboardStatisticsModel? actual = await service.GetDashboardStatistics();

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDashboardStatistics method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetDashboardStatisticsException()
        {
            _MockAPIClient.Setup(c => c.GetDashboardStatistics())
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            DashboardStatisticsModel? actual = await service.GetDashboardStatistics();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetRecentAuditLogs

        /// <summary>
        /// Tests whether the GetRecentAuditLogs method returns audit logs with UTC dates.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecentAuditLogs()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            PagedAPIResponseModel<AuditHistoryModel> pagedResponse = new()
            {
                Entries =
                [
                    new AuditHistoryModel
                    {
                        Id = 1,
                        IpAddress = "127.0.0.1",
                        Endpoint = "/api/v1/user",
                        EndpointVersion = "1.0",
                        Method = "GET",
                        Status = "200",
                        OccuredAt = originalDate,
                        Paramaters = []
                    }
                ],
                EntryCount = 1,
                PageNumber = 1,
                PageSize = 10,
                TotalPageCount = 1,
                TotalCount = 1
            };

            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ReturnsAsync(pagedResponse);

            APIService service = CreateService();
            List<AuditHistoryModel> actual = await service.GetRecentAuditLogs();

            Assert.AreEqual(
                1,
                actual.Count);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual[0].OccuredAt.Kind);
        }

        /// <summary>
        /// Tests whether the GetRecentAuditLogs method returns an empty list when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecentAuditLogsNull()
        {
            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ReturnsAsync((PagedAPIResponseModel<AuditHistoryModel>?)null);

            APIService service = CreateService();
            List<AuditHistoryModel> actual = await service.GetRecentAuditLogs();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Tests whether the GetRecentAuditLogs method returns an empty list when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetRecentAuditLogsException()
        {
            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            List<AuditHistoryModel> actual = await service.GetRecentAuditLogs();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetAuditHistories

        /// <summary>
        /// Tests whether the GetAuditHistories method returns paged audit histories with UTC dates.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistories()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            PagedAPIResponseModel<AuditHistoryModel> pagedResponse = new()
            {
                Entries =
                [
                    new AuditHistoryModel
                    {
                        Id = 1,
                        IpAddress = "127.0.0.1",
                        Endpoint = "/api/v1/user",
                        EndpointVersion = "1.0",
                        Method = "GET",
                        Status = "200",
                        OccuredAt = originalDate,
                        Paramaters = []
                    }
                ],
                EntryCount = 1,
                PageNumber = 1,
                PageSize = 25,
                TotalPageCount = 1,
                TotalCount = 1
            };

            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ReturnsAsync(pagedResponse);

            APIService service = CreateService();
            PagedAPIResponseModel<AuditHistoryModel>? actual = await service.GetAuditHistories();

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.Entries[0].OccuredAt.Kind);
        }

        /// <summary>
        /// Tests whether the GetAuditHistories method passes query parameters to the API client.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoriesWithFilters()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<List<KeyValuePair<string, object>>?>(p => capturedParams = p)
                .ReturnsAsync((PagedAPIResponseModel<AuditHistoryModel>?)null);

            APIService service = CreateService();

            await service.GetAuditHistories(
                fromDate: "2025-01-01",
                toDate: "2025-06-01",
                ipAddress: "127.0.0.1",
                endpoint: "/api/v1/user",
                username: "TestUser",
                application: "TestApp",
                pageSize: 50,
                pageNumber: 2);

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                8,
                capturedParams.Count);
        }

        /// <summary>
        /// Tests whether the GetAuditHistories method excludes default query parameters.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoriesDefaultParams()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<List<KeyValuePair<string, object>>?>(p => capturedParams = p)
                .ReturnsAsync((PagedAPIResponseModel<AuditHistoryModel>?)null);

            APIService service = CreateService();
            await service.GetAuditHistories();

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                0,
                capturedParams.Count);
        }

        /// <summary>
        /// Tests whether the GetAuditHistories method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoriesException()
        {
            _MockAPIClient.Setup(c => c.GetPagedAuditHistory(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            PagedAPIResponseModel<AuditHistoryModel>? actual = await service.GetAuditHistories();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetAuditHistory

        /// <summary>
        /// Tests whether the GetAuditHistory method returns an audit history with a UTC date.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistory()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            AuditHistoryModel expected = new()
            {
                Id = 1,
                IpAddress = "127.0.0.1",
                Endpoint = "/api/v1/user",
                EndpointVersion = "1.0",
                Method = "GET",
                Status = "200",
                OccuredAt = originalDate,
                Paramaters = []
            };

            _MockAPIClient.Setup(c => c.GetAuditHistory(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            AuditHistoryModel? actual = await service.GetAuditHistory(1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.OccuredAt.Kind);
        }

        /// <summary>
        /// Tests whether the GetAuditHistory method returns null when no audit history is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryNull()
        {
            _MockAPIClient.Setup(c => c.GetAuditHistory(1))
                .ReturnsAsync((AuditHistoryModel?)null);

            APIService service = CreateService();
            AuditHistoryModel? actual = await service.GetAuditHistory(1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetAuditHistory method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetAuditHistoryException()
        {
            _MockAPIClient.Setup(c => c.GetAuditHistory(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            AuditHistoryModel? actual = await service.GetAuditHistory(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region GetUserSettings

        /// <summary>
        /// Tests whether the GetUserSettings method returns user settings.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettings()
        {
            List<UserSettingModel> expected =
            [
                new UserSettingModel
                {
                    Application = "TestApp",
                    Settings = [new SettingModel { Id = 1, Name = "Theme", Value = "Dark" }]
                }
            ];

            _MockAPIClient.Setup(c => c.GetUserSettings(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            List<UserSettingModel> actual = await service.GetUserSettings(1);

            Assert.AreEqual(
                1,
                actual.Count);

            Assert.AreEqual(
                "TestApp",
                actual[0].Application);
        }

        /// <summary>
        /// Tests whether the GetUserSettings method returns an empty list when no settings are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettingsEmpty()
        {
            _MockAPIClient.Setup(c => c.GetUserSettings(1))
                .ReturnsAsync([]);

            APIService service = CreateService();
            List<UserSettingModel> actual = await service.GetUserSettings(1);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Tests whether the GetUserSettings method returns an empty list when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetUserSettingsException()
        {
            _MockAPIClient.Setup(c => c.GetUserSettings(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            List<UserSettingModel> actual = await service.GetUserSettings(1);

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region CreateUserSetting

        /// <summary>
        /// Tests whether the CreateUserSetting method returns the created setting.
        /// </summary>
        [TestMethod]
        public async Task TestCreateUserSetting()
        {
            UserSettingRequestModel request = new()
            {
                UserId = 1,
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            };

            UserSettingModel expectedSetting = new()
            {
                Application = "TestApp",
                Settings = [new SettingModel { Id = 1, Name = "Theme", Value = "Dark" }]
            };

            _MockAPIClient.Setup(c => c.CreateUserSetting(request))
                .ReturnsAsync((expectedSetting, (ResponseModel?)null));

            APIService service = CreateService();
            (UserSettingModel? actual, ResponseModel? _) = await service.CreateUserSetting(request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "TestApp",
                actual.Application);
        }

        /// <summary>
        /// Tests whether the CreateUserSetting method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestCreateUserSettingException()
        {
            UserSettingRequestModel request = new()
            {
                UserId = 1,
                Application = "TestApp",
                SettingName = "Theme",
                SettingValue = "Dark"
            };

            _MockAPIClient.Setup(c => c.CreateUserSetting(request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (UserSettingModel? actual, ResponseModel? actualResponse) = await service.CreateUserSetting(request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region UpdateUserSetting

        /// <summary>
        /// Tests whether the UpdateUserSetting method returns the updated setting.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUserSetting()
        {
            UserSettingUpdateRequestModel request = new() { Value = "Light" };

            SettingModel expectedSetting = new() { Id = 1, Name = "Theme", Value = "Light" };

            _MockAPIClient.Setup(c => c.UpdateUserSetting(1, request))
                .ReturnsAsync((expectedSetting, (ResponseModel?)null));

            APIService service = CreateService();
            (SettingModel? actual, ResponseModel? _) = await service.UpdateUserSetting(1, request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "Light",
                actual.Value);
        }

        /// <summary>
        /// Tests whether the UpdateUserSetting method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateUserSettingException()
        {
            UserSettingUpdateRequestModel request = new() { Value = "Light" };

            _MockAPIClient.Setup(c => c.UpdateUserSetting(1, request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (SettingModel? actual, ResponseModel? actualResponse) = await service.UpdateUserSetting(1, request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region GetServers

        /// <summary>
        /// Tests whether the GetServers method returns servers.
        /// </summary>
        [TestMethod]
        public async Task TestGetServers()
        {
            List<ServerInformationModel> expected =
            [
                new ServerInformationModel
                {
                    Id = 1,
                    Name = "TestServer",
                    HostName = "test-host",
                    Game = "TestGame",
                    GameVersion = "1.0",
                    Connection = new ServerConnectionModel { IpAddress = "127.0.0.1", Port = 25565 },
                    EventInterval = 60,
                    IsActive = true
                }
            ];

            _MockAPIClient.Setup(c => c.GetServers())
                .ReturnsAsync(expected);

            APIService service = CreateService();
            List<ServerInformationModel> actual = await service.GetServers();

            Assert.AreEqual(
                1,
                actual.Count);

            Assert.AreEqual(
                "TestServer",
                actual[0].Name);
        }

        /// <summary>
        /// Tests whether the GetServers method returns an empty list when no servers are found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServersEmpty()
        {
            _MockAPIClient.Setup(c => c.GetServers())
                .ReturnsAsync([]);

            APIService service = CreateService();
            List<ServerInformationModel> actual = await service.GetServers();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        /// <summary>
        /// Tests whether the GetServers method returns an empty list when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetServersException()
        {
            _MockAPIClient.Setup(c => c.GetServers())
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            List<ServerInformationModel> actual = await service.GetServers();

            Assert.AreEqual(
                0,
                actual.Count);
        }

        #endregion

        #region GetServer

        /// <summary>
        /// Tests whether the GetServer method returns a server.
        /// </summary>
        [TestMethod]
        public async Task TestGetServer()
        {
            ServerInformationModel expected = new()
            {
                Id = 1,
                Name = "TestServer",
                HostName = "test-host",
                Game = "TestGame",
                GameVersion = "1.0",
                Connection = new ServerConnectionModel { IpAddress = "127.0.0.1", Port = 25565 },
                EventInterval = 60,
                IsActive = true
            };

            _MockAPIClient.Setup(c => c.GetServer(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ServerInformationModel? actual = await service.GetServer(1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "TestServer",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the GetServer method returns null when no server is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerNull()
        {
            _MockAPIClient.Setup(c => c.GetServer(1))
                .ReturnsAsync((ServerInformationModel?)null);

            APIService service = CreateService();
            ServerInformationModel? actual = await service.GetServer(1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetServer method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerException()
        {
            _MockAPIClient.Setup(c => c.GetServer(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ServerInformationModel? actual = await service.GetServer(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region CreateServer

        /// <summary>
        /// Tests whether the CreateServer method returns the created server.
        /// </summary>
        [TestMethod]
        public async Task TestCreateServer()
        {
            ServerRequestModel request = new()
            {
                Name = "NewServer",
                EventInterval = 60,
                HostName = "new-host",
                Game = "TestGame",
                GameVersion = "1.0",
                IPAddress = "127.0.0.1",
                Port = 25565
            };

            ServerInformationModel expectedServer = new()
            {
                Id = 1,
                Name = "NewServer",
                HostName = "new-host",
                Game = "TestGame",
                GameVersion = "1.0",
                Connection = new ServerConnectionModel { IpAddress = "127.0.0.1", Port = 25565 },
                EventInterval = 60,
                IsActive = true
            };

            _MockAPIClient.Setup(c => c.CreateServer(request))
                .ReturnsAsync((expectedServer, (ResponseModel?)null));

            APIService service = CreateService();
            (ServerInformationModel? actual, ResponseModel? _) = await service.CreateServer(request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "NewServer",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the CreateServer method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestCreateServerException()
        {
            ServerRequestModel request = new()
            {
                Name = "NewServer",
                EventInterval = 60,
                HostName = "new-host",
                Game = "TestGame",
                GameVersion = "1.0",
                IPAddress = "127.0.0.1",
                Port = 25565
            };

            _MockAPIClient.Setup(c => c.CreateServer(request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (ServerInformationModel? actual, ResponseModel? actualResponse) = await service.CreateServer(request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region UpdateServer

        /// <summary>
        /// Tests whether the UpdateServer method returns the updated server.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateServer()
        {
            ServerUpdateRequestModel request = new() { Name = "UpdatedServer" };

            ServerInformationModel expectedServer = new()
            {
                Id = 1,
                Name = "UpdatedServer",
                HostName = "test-host",
                Game = "TestGame",
                GameVersion = "1.0",
                Connection = new ServerConnectionModel { IpAddress = "127.0.0.1", Port = 25565 },
                EventInterval = 60,
                IsActive = true
            };

            _MockAPIClient.Setup(c => c.UpdateServer(1, request))
                .ReturnsAsync((expectedServer, (ResponseModel?)null));

            APIService service = CreateService();
            (ServerInformationModel? actual, ResponseModel? _) = await service.UpdateServer(1, request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "UpdatedServer",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the UpdateServer method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateServerException()
        {
            ServerUpdateRequestModel request = new() { Name = "UpdatedServer" };

            _MockAPIClient.Setup(c => c.UpdateServer(1, request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (ServerInformationModel? actual, ResponseModel? actualResponse) = await service.UpdateServer(1, request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region GetServerStatistics

        /// <summary>
        /// Tests whether the GetServerStatistics method returns statistics with UTC dates.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatistics()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            ServerStatisticsModel expected = new()
            {
                ComponentAlerts = [],
                StatusAlerts = [],
                LatestEvents =
                [
                    new ServerEventModel { Component = "CPU", Status = "Online", DateOccured = originalDate }
                ],
                ServerHealth = [],
                RecentAlerts =
                [
                    new RecentAlertModel
                    {
                        AlertId = 1,
                        Reporter = "System",
                        Component = "CPU",
                        ComponentStatus = "Online",
                        AlertStatus = "Active",
                        AlertDate = originalDate
                    }
                ],
                RecentEvents =
                [
                    new ServerEventModel { Component = "Memory", Status = "Online", DateOccured = originalDate }
                ]
            };

            _MockAPIClient.Setup(c => c.GetServerStatistics(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ServerStatisticsModel? actual = await service.GetServerStatistics(1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.LatestEvents[0].DateOccured.Kind);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.RecentAlerts[0].AlertDate.Kind);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.RecentEvents[0].DateOccured.Kind);
        }

        /// <summary>
        /// Tests whether the GetServerStatistics method returns null when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticsNull()
        {
            _MockAPIClient.Setup(c => c.GetServerStatistics(1))
                .ReturnsAsync((ServerStatisticsModel?)null);

            APIService service = CreateService();
            ServerStatisticsModel? actual = await service.GetServerStatistics(1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetServerStatistics method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetServerStatisticsException()
        {
            _MockAPIClient.Setup(c => c.GetServerStatistics(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ServerStatisticsModel? actual = await service.GetServerStatistics(1);

            Assert.IsNull(actual);
        }

        #endregion

        #region GetConfiguration

        /// <summary>
        /// Tests whether the GetConfiguration method returns configuration.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfiguration()
        {
            ConfigurationModel expected = new()
            {
                ConfigurationObjects = ["Application", "Machine", "Game"]
            };

            _MockAPIClient.Setup(c => c.GetConfiguration())
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ConfigurationModel? actual = await service.GetConfiguration();

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                3,
                actual.ConfigurationObjects.Count);
        }

        /// <summary>
        /// Tests whether the GetConfiguration method returns null when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfigurationNull()
        {
            _MockAPIClient.Setup(c => c.GetConfiguration())
                .ReturnsAsync((ConfigurationModel?)null);

            APIService service = CreateService();
            ConfigurationModel? actual = await service.GetConfiguration();

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetConfiguration method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfigurationException()
        {
            _MockAPIClient.Setup(c => c.GetConfiguration())
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ConfigurationModel? actual = await service.GetConfiguration();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetConfigurationEntity

        /// <summary>
        /// Tests whether the GetConfigurationEntity method returns an entity.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfigurationEntity()
        {
            ApplicationModel expected = new()
            {
                Id = 1,
                Name = "TestApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            _MockAPIClient.Setup(c => c.GetConfigurationEntity<ApplicationModel>("application", 1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ApplicationModel? actual = await service.GetConfigurationEntity<ApplicationModel>("application", 1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "TestApp",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the GetConfigurationEntity method returns null when no entity is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfigurationEntityNull()
        {
            _MockAPIClient.Setup(c => c.GetConfigurationEntity<ApplicationModel>("application", 1))
                .ReturnsAsync((ApplicationModel?)null);

            APIService service = CreateService();
            ApplicationModel? actual = await service.GetConfigurationEntity<ApplicationModel>("application", 1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetConfigurationEntity method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetConfigurationEntityException()
        {
            _MockAPIClient.Setup(c => c.GetConfigurationEntity<ApplicationModel>("application", 1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ApplicationModel? actual = await service.GetConfigurationEntity<ApplicationModel>("application", 1);

            Assert.IsNull(actual);
        }

        #endregion

        #region GetPagedConfiguration

        /// <summary>
        /// Tests whether the GetPagedConfiguration method returns paged configuration.
        /// </summary>
        [TestMethod]
        public async Task TestGetPagedConfiguration()
        {
            PagedAPIResponseModel<ApplicationModel> expected = new()
            {
                Entries =
                [
                    new ApplicationModel
                    {
                        Id = 1,
                        Name = "TestApp",
                        Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                        Settings = [],
                        IsDeleted = false
                    }
                ],
                EntryCount = 1,
                PageNumber = 1,
                PageSize = 25,
                TotalPageCount = 1,
                TotalCount = 1
            };

            _MockAPIClient.Setup(c => c.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>(
                    "application",
                    It.IsAny<List<KeyValuePair<string, object>>>(),
                    true))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            PagedAPIResponseModel<ApplicationModel>? actual = await service.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>("application");

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                1,
                actual.EntryCount);
        }

        /// <summary>
        /// Tests whether the GetPagedConfiguration method passes custom page parameters.
        /// </summary>
        [TestMethod]
        public async Task TestGetPagedConfigurationCustomParams()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            _MockAPIClient.Setup(c => c.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>(
                    "application",
                    It.IsAny<List<KeyValuePair<string, object>>>(),
                    false))
                .Callback<string, List<KeyValuePair<string, object>>?, bool>((_, p, _) => capturedParams = p)
                .ReturnsAsync((PagedAPIResponseModel<ApplicationModel>?)null);

            APIService service = CreateService();

            await service.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>(
                "application",
                pageSize: 50,
                pageNumber: 3,
                ignoreQuery: false);

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                2,
                capturedParams.Count);
        }

        /// <summary>
        /// Tests whether the GetPagedConfiguration method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetPagedConfigurationException()
        {
            _MockAPIClient.Setup(c => c.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>(
                    "application",
                    It.IsAny<List<KeyValuePair<string, object>>>(),
                    true))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            PagedAPIResponseModel<ApplicationModel>? actual = await service.GetPagedConfiguration<PagedAPIResponseModel<ApplicationModel>>("application");

            Assert.IsNull(actual);
        }

        #endregion

        #region CreateConfigurationEntity

        /// <summary>
        /// Tests whether the CreateConfigurationEntity method returns the created entity.
        /// </summary>
        [TestMethod]
        public async Task TestCreateConfigurationEntity()
        {
            ApplicationRequestModel request = new() { Name = "NewApp", Phrase = "NewPhrase" };

            ApplicationModel expectedApp = new()
            {
                Id = 2,
                Name = "NewApp",
                Authorisation = new AuthorisationModel { Id = 2, Phrase = "NewPhrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            _MockAPIClient.Setup(c => c.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                    "application",
                    request,
                    It.IsAny<List<KeyValuePair<string, object>>>()))
                .ReturnsAsync((expectedApp, (ResponseModel?)null));

            APIService service = CreateService();
            (ApplicationModel? actual, ResponseModel? _) = await service.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                "application",
                "NewApp",
                request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "NewApp",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the CreateConfigurationEntity method passes parent entity id as a query parameter.
        /// </summary>
        [TestMethod]
        public async Task TestCreateConfigurationEntityWithParentId()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            ApplicationRequestModel request = new() { Name = "NewApp", Phrase = "NewPhrase" };

            _MockAPIClient.Setup(c => c.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                    "application",
                    request,
                    It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<string, ApplicationRequestModel, List<KeyValuePair<string, object>>?>((_, _, p) => capturedParams = p)
                .ReturnsAsync(((ApplicationModel?)null, (ResponseModel?)null));

            APIService service = CreateService();

            await service.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                "application",
                "NewApp",
                request,
                parentEntityId: 5);

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                1,
                capturedParams.Count);

            Assert.AreEqual(
                "entityId",
                capturedParams[0].Key);

            Assert.AreEqual(
                5,
                capturedParams[0].Value);
        }

        /// <summary>
        /// Tests whether the CreateConfigurationEntity method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestCreateConfigurationEntityException()
        {
            ApplicationRequestModel request = new() { Name = "NewApp", Phrase = "NewPhrase" };

            _MockAPIClient.Setup(c => c.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                    "application",
                    request,
                    It.IsAny<List<KeyValuePair<string, object>>>()))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (ApplicationModel? actual, ResponseModel? actualResponse) = await service.CreateConfigurationEntity<ApplicationModel, ApplicationRequestModel>(
                "application",
                "NewApp",
                request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region UpdateConfigurationEntity

        /// <summary>
        /// Tests whether the UpdateConfigurationEntity method returns the updated entity.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateConfigurationEntity()
        {
            ApplicationUpdateRequestModel request = new() { Name = "UpdatedApp" };

            ApplicationModel expectedApp = new()
            {
                Id = 1,
                Name = "UpdatedApp",
                Authorisation = new AuthorisationModel { Id = 1, Phrase = "TestPhrase", IsDeleted = false },
                Settings = [],
                IsDeleted = false
            };

            _MockAPIClient.Setup(c => c.UpdateConfigurationEntity<ApplicationModel, ApplicationUpdateRequestModel>(
                    "application",
                    1,
                    request))
                .ReturnsAsync((expectedApp, (ResponseModel?)null));

            APIService service = CreateService();
            (ApplicationModel? actual, ResponseModel? _) = await service.UpdateConfigurationEntity<ApplicationModel, ApplicationUpdateRequestModel>(
                "application",
                1,
                request);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                "UpdatedApp",
                actual.Name);
        }

        /// <summary>
        /// Tests whether the UpdateConfigurationEntity method handles an exception.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateConfigurationEntityException()
        {
            ApplicationUpdateRequestModel request = new() { Name = "UpdatedApp" };

            _MockAPIClient.Setup(c => c.UpdateConfigurationEntity<ApplicationModel, ApplicationUpdateRequestModel>(
                    "application",
                    1,
                    request))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            (ApplicationModel? actual, ResponseModel? actualResponse) = await service.UpdateConfigurationEntity<ApplicationModel, ApplicationUpdateRequestModel>(
                "application",
                1,
                request);

            Assert.IsNull(actual);
            Assert.IsNull(actualResponse);
        }

        #endregion

        #region DeleteConfigurationEntity

        /// <summary>
        /// Tests whether the DeleteConfigurationEntity method returns true on success.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteConfigurationEntity()
        {
            _MockAPIClient.Setup(c => c.DeleteConfigurationEntity("application", 1))
                .ReturnsAsync(true);

            APIService service = CreateService();
            bool actual = await service.DeleteConfigurationEntity("application", 1);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Tests whether the DeleteConfigurationEntity method returns false on failure.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteConfigurationEntityFailed()
        {
            _MockAPIClient.Setup(c => c.DeleteConfigurationEntity("application", 1))
                .ReturnsAsync(false);

            APIService service = CreateService();
            bool actual = await service.DeleteConfigurationEntity("application", 1);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Tests whether the DeleteConfigurationEntity method returns false when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteConfigurationEntityException()
        {
            _MockAPIClient.Setup(c => c.DeleteConfigurationEntity("application", 1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            bool actual = await service.DeleteConfigurationEntity("application", 1);

            Assert.IsFalse(actual);
        }

        #endregion

        #region GetLogStatistics

        /// <summary>
        /// Tests whether the GetLogStatistics method returns statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetLogStatistics()
        {
            SharedStatisticsModel expected = new()
            {
                EndpointCalls = [],
                MethodCalls = [],
                StatusCalls = [],
                Changes = []
            };

            _MockAPIClient.Setup(c => c.GetLogStatistics("user", 1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            SharedStatisticsModel? actual = await service.GetLogStatistics("user", 1);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetLogStatistics method returns null when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetLogStatisticsNull()
        {
            _MockAPIClient.Setup(c => c.GetLogStatistics("user", 1))
                .ReturnsAsync((SharedStatisticsModel?)null);

            APIService service = CreateService();
            SharedStatisticsModel? actual = await service.GetLogStatistics("user", 1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetLogStatistics method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetLogStatisticsException()
        {
            _MockAPIClient.Setup(c => c.GetLogStatistics("user", 1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            SharedStatisticsModel? actual = await service.GetLogStatistics("user", 1);

            Assert.IsNull(actual);
        }

        #endregion

        #region GetErrorStatistics

        /// <summary>
        /// Tests whether the GetErrorStatistics method returns statistics.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatistics()
        {
            ErrorStatisticsModel expected = new()
            {
                Errors = [],
                IPErrors = [],
                SummaryErrors = []
            };

            _MockAPIClient.Setup(c => c.GetErrorStatistics())
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ErrorStatisticsModel? actual = await service.GetErrorStatistics();

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetErrorStatistics method returns null when no data is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticsNull()
        {
            _MockAPIClient.Setup(c => c.GetErrorStatistics())
                .ReturnsAsync((ErrorStatisticsModel?)null);

            APIService service = CreateService();
            ErrorStatisticsModel? actual = await service.GetErrorStatistics();

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetErrorStatistics method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorStatisticsException()
        {
            _MockAPIClient.Setup(c => c.GetErrorStatistics())
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ErrorStatisticsModel? actual = await service.GetErrorStatistics();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetErrors

        /// <summary>
        /// Tests whether the GetErrors method returns paged errors with UTC dates.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrors()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            PagedAPIResponseModel<ErrorModel> pagedResponse = new()
            {
                Entries =
                [
                    new ErrorModel
                    {
                        Id = 1,
                        DateOccured = originalDate,
                        IPAddress = "127.0.0.1",
                        Summary = "Error in UserService.GetUsers.",
                        Message = "NullReferenceException"
                    }
                ],
                EntryCount = 1,
                PageNumber = 1,
                PageSize = 25,
                TotalPageCount = 1,
                TotalCount = 1
            };

            _MockAPIClient.Setup(c => c.GetPagedErrorLog(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ReturnsAsync(pagedResponse);

            APIService service = CreateService();
            PagedAPIResponseModel<ErrorModel>? actual = await service.GetErrors();

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.Entries[0].DateOccured.Kind);
        }

        /// <summary>
        /// Tests whether the GetErrors method passes query parameters to the API client.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorsWithFilters()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            _MockAPIClient.Setup(c => c.GetPagedErrorLog(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<List<KeyValuePair<string, object>>?>(p => capturedParams = p)
                .ReturnsAsync((PagedAPIResponseModel<ErrorModel>?)null);

            APIService service = CreateService();

            await service.GetErrors(
                fromDate: "2025-01-01",
                toDate: "2025-06-01",
                ipAddress: "127.0.0.1",
                summary: "Error",
                pageSize: 50,
                pageNumber: 2);

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                6,
                capturedParams.Count);
        }

        /// <summary>
        /// Tests whether the GetErrors method excludes default query parameters.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorsDefaultParams()
        {
            List<KeyValuePair<string, object>>? capturedParams = null;

            _MockAPIClient.Setup(c => c.GetPagedErrorLog(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Callback<List<KeyValuePair<string, object>>?>(p => capturedParams = p)
                .ReturnsAsync((PagedAPIResponseModel<ErrorModel>?)null);

            APIService service = CreateService();
            await service.GetErrors();

            Assert.IsNotNull(capturedParams);

            Assert.AreEqual(
                0,
                capturedParams.Count);
        }

        /// <summary>
        /// Tests whether the GetErrors method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorsException()
        {
            _MockAPIClient.Setup(c => c.GetPagedErrorLog(It.IsAny<List<KeyValuePair<string, object>>>()))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            PagedAPIResponseModel<ErrorModel>? actual = await service.GetErrors();

            Assert.IsNull(actual);
        }

        #endregion

        #region GetError

        /// <summary>
        /// Tests whether the GetError method returns an error with a UTC date.
        /// </summary>
        [TestMethod]
        public async Task TestGetError()
        {
            DateTime originalDate = new(2025, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);

            ErrorModel expected = new()
            {
                Id = 1,
                DateOccured = originalDate,
                IPAddress = "127.0.0.1",
                Summary = "Error in UserService.GetUsers.",
                Message = "NullReferenceException"
            };

            _MockAPIClient.Setup(c => c.GetError(1))
                .ReturnsAsync(expected);

            APIService service = CreateService();
            ErrorModel? actual = await service.GetError(1);

            Assert.IsNotNull(actual);

            Assert.AreEqual(
                DateTimeKind.Utc,
                actual.DateOccured.Kind);
        }

        /// <summary>
        /// Tests whether the GetError method returns null when no error is found.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorNull()
        {
            _MockAPIClient.Setup(c => c.GetError(1))
                .ReturnsAsync((ErrorModel?)null);

            APIService service = CreateService();
            ErrorModel? actual = await service.GetError(1);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetError method returns null when an exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetErrorException()
        {
            _MockAPIClient.Setup(c => c.GetError(1))
                .ThrowsAsync(new Exception("Connection refused"));

            APIService service = CreateService();
            ErrorModel? actual = await service.GetError(1);

            Assert.IsNull(actual);
        }

        #endregion
    }
}
