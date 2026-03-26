// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Tests.Converters
{
    [TestClass]
    public class ConfigurationConverterTest
    {
        #region GetSQLGet

        /// <summary>
        /// Tests whether the GetSQLGet method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLGet()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLGet("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetApplication.sql" when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetApplication()
        {
            string expected = "GetApplication.sql";
            string actual = ConfigurationConverter.GetSQLGet("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetApplicationSetting.sql" when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetApplicationSetting()
        {
            string expected = "GetApplicationSetting.sql";
            string actual = ConfigurationConverter.GetSQLGet("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetAuthorisation.sql" when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetAuthorisation()
        {
            string expected = "GetAuthorisation.sql";
            string actual = ConfigurationConverter.GetSQLGet("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetComponent.sql" when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetComponent()
        {
            string expected = "GetComponent.sql";
            string actual = ConfigurationConverter.GetSQLGet("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetConnection.sql" when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetConnection()
        {
            string expected = "GetConnection.sql";
            string actual = ConfigurationConverter.GetSQLGet("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetDowntime.sql" when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetDowntime()
        {
            string expected = "GetDowntime.sql";
            string actual = ConfigurationConverter.GetSQLGet("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetGame.sql" when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetGame()
        {
            string expected = "GetGame.sql";
            string actual = ConfigurationConverter.GetSQLGet("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGet method returns "GetMachine.sql" when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetMachine()
        {
            string expected = "GetMachine.sql";
            string actual = ConfigurationConverter.GetSQLGet("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLGetPagination

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPagination()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLGetPagination("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the application pagination sql when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationApplication()
        {
            string expected = @"
order by ApplicationId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the authorisation pagination sql when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationAuthorisation()
        {
            string expected = @"
order by PhraseId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the component pagination sql when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationComponent()
        {
            string expected = @"
order by ComponentId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the connection pagination sql when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationConnection()
        {
            string expected = @"
order by ConnectionId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the downtime pagination sql when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationDowntime()
        {
            string expected = @"
order by DowntimeId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the game pagination sql when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationGame()
        {
            string expected = @"
order by GameId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetPagination method returns the machine pagination sql when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetPaginationMachine()
        {
            string expected = @"
order by MachineId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
            string actual = ConfigurationConverter.GetSQLGetPagination("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLGetTotal

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotal()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalApplication.sql" when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalApplication()
        {
            string expected = "GetTotalApplication.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalApplicationSetting.sql" when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalApplicationSetting()
        {
            string expected = "GetTotalApplicationSetting.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalAuthorisation.sql" when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalAuthorisation()
        {
            string expected = "GetTotalAuthorisation.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalComponent.sql" when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalComponent()
        {
            string expected = "GetTotalComponent.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalConnection.sql" when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalConnection()
        {
            string expected = "GetTotalConnection.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalDowntime.sql" when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalDowntime()
        {
            string expected = "GetTotalDowntime.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalGame.sql" when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalGame()
        {
            string expected = "GetTotalGame.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLGetTotal method returns "GetTotalMachine.sql" when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLGetTotalMachine()
        {
            string expected = "GetTotalMachine.sql";
            string actual = ConfigurationConverter.GetSQLGetTotal("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLExists

        /// <summary>
        /// Tests whether the GetSQLExists method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLExists()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLExists("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "ApplicationExists.sql" when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsApplication()
        {
            string expected = "ApplicationExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "ApplicationSettingExists.sql" when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsApplicationSetting()
        {
            string expected = "ApplicationSettingExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "AuthorisationExists.sql" when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsAuthorisation()
        {
            string expected = "AuthorisationExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "ComponentExists.sql" when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsComponent()
        {
            string expected = "ComponentExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "ConnectionExists.sql" when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsConnection()
        {
            string expected = "ConnectionExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "DowntimeExists.sql" when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsDowntime()
        {
            string expected = "DowntimeExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "GameExists.sql" when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsGame()
        {
            string expected = "GameExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLExists method returns "MachineExists.sql" when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLExistsMachine()
        {
            string expected = "MachineExists.sql";
            string actual = ConfigurationConverter.GetSQLExists("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLFilter

        /// <summary>
        /// Tests whether the GetSQLFilter method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilter()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLFilter("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the application filter sql when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterApplication()
        {
            string expected = @"
where [Name] = @Name";
            string actual = ConfigurationConverter.GetSQLFilter("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the applicationSetting filter sql when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterApplicationSetting()
        {
            string expected = @"
where ApplicationId = @ApplicationId
and [Name] = @Name";
            string actual = ConfigurationConverter.GetSQLFilter("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the authorisation filter sql when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterAuthorisation()
        {
            string expected = @"
where Phrase = @Phrase";
            string actual = ConfigurationConverter.GetSQLFilter("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the component filter sql when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterComponent()
        {
            string expected = @"
where [Name] = @Name";
            string actual = ConfigurationConverter.GetSQLFilter("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the connection filter sql when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterConnection()
        {
            string expected = @"
where IPAddress = @IPAddress
and [Port] = @Port";
            string actual = ConfigurationConverter.GetSQLFilter("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the downtime filter sql when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterDowntime()
        {
            string expected = @"
where Time = @Time";
            string actual = ConfigurationConverter.GetSQLFilter("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the game filter sql when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterGame()
        {
            string expected = @"
where [Name] = @Name
and [Version] = @Version";
            string actual = ConfigurationConverter.GetSQLFilter("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilter method returns the machine filter sql when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterMachine()
        {
            string expected = @"
where HostName = @HostName";
            string actual = ConfigurationConverter.GetSQLFilter("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLFilterId

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterId()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLFilterId("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the application filter sql when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdApplication()
        {
            string expected = @"
where ApplicationId = @ApplicationId";
            string actual = ConfigurationConverter.GetSQLFilterId("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the applicationSetting filter sql when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdApplicationSetting()
        {
            string expected = @"
where ApplicationSettingId = @ApplicationSettingId";
            string actual = ConfigurationConverter.GetSQLFilterId("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the authorisation filter sql when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdAuthorisation()
        {
            string expected = @"
where PhraseId = @PhraseId";
            string actual = ConfigurationConverter.GetSQLFilterId("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the component filter sql when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdComponent()
        {
            string expected = @"
where ComponentId = @ComponentId";
            string actual = ConfigurationConverter.GetSQLFilterId("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the connection filter sql when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdConnection()
        {
            string expected = @"
where ConnectionId = @ConnectionId";
            string actual = ConfigurationConverter.GetSQLFilterId("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the downtime filter sql when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdDowntime()
        {
            string expected = @"
where DowntimeId = @DowntimeId";
            string actual = ConfigurationConverter.GetSQLFilterId("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the game filter sql when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdGame()
        {
            string expected = @"
where GameId = @GameId";
            string actual = ConfigurationConverter.GetSQLFilterId("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLFilterId method returns the machine filter sql when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLFilterIdMachine()
        {
            string expected = @"
where MachineId = @MachineId";
            string actual = ConfigurationConverter.GetSQLFilterId("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLCreate

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreate()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLCreate("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateApplication.sql" when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateApplication()
        {
            string expected = "CreateApplication.sql";
            string actual = ConfigurationConverter.GetSQLCreate("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateApplicationSetting.sql" when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateApplicationSetting()
        {
            string expected = "CreateApplicationSetting.sql";
            string actual = ConfigurationConverter.GetSQLCreate("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateAuthorisation.sql" when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateAuthorisation()
        {
            string expected = "CreateAuthorisation.sql";
            string actual = ConfigurationConverter.GetSQLCreate("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateComponent.sql" when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateComponent()
        {
            string expected = "CreateComponent.sql";
            string actual = ConfigurationConverter.GetSQLCreate("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateConnection.sql" when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateConnection()
        {
            string expected = "CreateConnection.sql";
            string actual = ConfigurationConverter.GetSQLCreate("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateDowntime.sql" when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateDowntime()
        {
            string expected = "CreateDowntime.sql";
            string actual = ConfigurationConverter.GetSQLCreate("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateGame.sql" when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateGame()
        {
            string expected = "CreateGame.sql";
            string actual = ConfigurationConverter.GetSQLCreate("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLCreate method returns "CreateMachine.sql" when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLCreateMachine()
        {
            string expected = "CreateMachine.sql";
            string actual = ConfigurationConverter.GetSQLCreate("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLUpdated

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdated()
        {
            string expected = "Unknown.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "ApplicationUpdated.sql" when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedApplication()
        {
            string expected = "ApplicationUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("application");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "ApplicationSettingUpdated.sql" when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedApplicationSetting()
        {
            string expected = "ApplicationSettingUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("applicationSetting");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "AuthorisationUpdated.sql" when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedAuthorisation()
        {
            string expected = "AuthorisationUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("authorisation");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "ComponentUpdated.sql" when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedComponent()
        {
            string expected = "ComponentUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("component");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "ConnectionUpdated.sql" when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedConnection()
        {
            string expected = "ConnectionUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("connection");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "DowntimeUpdated.sql" when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedDowntime()
        {
            string expected = "DowntimeUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("downtime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "GameUpdated.sql" when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedGame()
        {
            string expected = "GameUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("game");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLUpdated method returns "MachineUpdated.sql" when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetSQLUpdatedMachine()
        {
            string expected = "MachineUpdated.sql";
            string actual = ConfigurationConverter.GetSQLUpdated("machine");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetParametersGet

        /// <summary>
        /// Tests whether the GetParametersGet method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersGet()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("Trombone", 10, 1);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetApplication()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("application", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetAuthorisation()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("authorisation", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetComponent()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("component", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetConnection()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("connection", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetDowntime()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("downtime", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetGame()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("game", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGet method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetMachine()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGet("machine", 10, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PageSize", actual[0].ParameterName);
            Assert.AreEqual(10, actual[0].Value);
            Assert.AreEqual("@PageNumber", actual[1].ParameterName);
            Assert.AreEqual(1, actual[1].Value);
        }

        #endregion

        #region GetParametersGetSingle

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingle()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("Trombone", 1);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleApplication()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("application", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleApplicationSetting()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("applicationSetting", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleAuthorisation()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("authorisation", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@PhraseId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleComponent()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("component", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ComponentId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleConnection()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("connection", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ConnectionId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleDowntime()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("downtime", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@DowntimeId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleGame()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("game", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@GameId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersGetSingle method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParametersGetSingleMachine()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersGetSingle("machine", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@MachineId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        #endregion

        #region GetParameterExists

        /// <summary>
        /// Tests whether the GetParameterExists method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParameterExists()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("Trombone", new object());

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsApplication()
        {
            ApplicationModel model = new ApplicationModel() { Name = "TestApp", Phrase = "TestPhrase" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("application", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestApp", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsApplicationSetting()
        {
            ApplicationSettingModel model = new ApplicationSettingModel() { Name = "TestSetting", Required = true };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("applicationSetting", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestSetting", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsAuthorisation()
        {
            AuthorisationModel model = new AuthorisationModel() { Phrase = "TestPhrase" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("authorisation", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Phrase", actual[0].ParameterName);
            Assert.AreEqual("TestPhrase", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsComponent()
        {
            ComponentModel model = new ComponentModel() { Name = "TestComponent" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("component", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestComponent", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsConnection()
        {
            ConnectionModel model = new ConnectionModel() { IPAddress = "192.168.1.1", Port = 8080 };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("connection", model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@IPAddress", actual[0].ParameterName);
            Assert.AreEqual("192.168.1.1", actual[0].Value);
            Assert.AreEqual("@Port", actual[1].ParameterName);
            Assert.AreEqual(8080, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsDowntime()
        {
            DowntimeModel model = new DowntimeModel() { Time = "03:00" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("downtime", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Time", actual[0].ParameterName);
            Assert.AreEqual("03:00", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsGame()
        {
            GameModel model = new GameModel() { Name = "TestGame", Version = "1.0" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("game", model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestGame", actual[0].Value);
            Assert.AreEqual("@Version", actual[1].ParameterName);
            Assert.AreEqual("1.0", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsMachine()
        {
            MachineModel model = new MachineModel() { HostName = "TestHost" };
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("machine", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@HostName", actual[0].ParameterName);
            Assert.AreEqual("TestHost", actual[0].Value);
        }

        #endregion

        #region GetParameterExistsById

        /// <summary>
        /// Tests whether the GetParameterExists method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsById()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("Trombone", 1);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdApplication()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("application", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdApplicationSetting()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("applicationSetting", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ApplicationSettingId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdAuthorisation()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("authorisation", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@PhraseId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdComponent()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("component", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ComponentId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdConnection()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("connection", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@ConnectionId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdDowntime()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("downtime", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@DowntimeId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdGame()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("game", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@GameId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParameterExists method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParameterExistsByIdMachine()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParameterExists("machine", 1);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@MachineId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
        }

        #endregion

        #region GetParametersCreate

        /// <summary>
        /// Tests whether the GetParametersCreate method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreate()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("Trombone", new object());

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateApplication()
        {
            ApplicationModel model = new ApplicationModel()
            {
                Name = "TestApp",
                Phrase = "TestPhrase"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("application", model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestApp", actual[0].Value);
            Assert.AreEqual("@Phrase", actual[1].ParameterName);
            Assert.AreEqual("TestPhrase", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateApplicationSetting()
        {
            ApplicationSettingModel model = new ApplicationSettingModel()
            {
                Name = "TestSetting",
                Required = true
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("applicationSetting", model, 1);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestSetting", actual[1].Value);
            Assert.AreEqual("@Required", actual[2].ParameterName);
            Assert.AreEqual(true, actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateAuthorisation()
        {
            AuthorisationModel model = new AuthorisationModel()
            {
                Phrase = "TestPhrase"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("authorisation", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Phrase", actual[0].ParameterName);
            Assert.AreEqual("TestPhrase", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateComponent()
        {
            ComponentModel model = new ComponentModel()
            {
                Name = "TestComponent"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("component", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestComponent", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateConnection()
        {
            ConnectionModel model = new ConnectionModel()
            {
               IPAddress = "192.168.1.1",
                Port = 8080
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("connection", model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@IPAddress", actual[0].ParameterName);
            Assert.AreEqual("192.168.1.1", actual[0].Value);
            Assert.AreEqual("@Port", actual[1].ParameterName);
            Assert.AreEqual(8080, actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateDowntime()
        {
            DowntimeModel model = new DowntimeModel()
            {
                Time = "03:00:00"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("downtime", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Time", actual[0].ParameterName);
            Assert.AreEqual("03:00:00", actual[0].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateGame()
        {
            GameModel model = new GameModel()
            {
                Name = "TestGame",
                Version = "1.0"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("game", model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
            Assert.AreEqual("TestGame", actual[0].Value);
            Assert.AreEqual("@Version", actual[1].ParameterName);
            Assert.AreEqual("1.0", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersCreate method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParametersCreateMachine()
        {
            MachineModel model = new MachineModel()
            {
                HostName = "TestMachine"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersCreate("machine", model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@HostName", actual[0].ParameterName);
            Assert.AreEqual("TestMachine", actual[0].Value);
        }

        #endregion

        #region GetParametersUpdated

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns an empty array when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdated()
        {
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("Trombone", new object(), 1);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "application".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedApplication()
        {
            ApplicationModel model = new ApplicationModel()
            {
                Name = "TestApp"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("application", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@ApplicationId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestApp", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "applicationSetting".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedApplicationSetting()
        {
            ApplicationSettingModel model = new ApplicationSettingModel()
            {
                Name = "TestSetting",
                Required = true
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("applicationSetting", model, 1);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("@ApplicationSettingId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestSetting", actual[1].Value);
            Assert.AreEqual("@Required", actual[2].ParameterName);
            Assert.AreEqual(true, actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "authorisation".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedAuthorisation()
        {
            AuthorisationModel model = new AuthorisationModel()
            {
                Phrase = "TestPhrase"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("authorisation", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@PhraseId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Phrase", actual[1].ParameterName);
            Assert.AreEqual("TestPhrase", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "component".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedComponent()
        {
            ComponentModel model = new ComponentModel()
            {
                Name = "TestComponent"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("component", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@ComponentId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestComponent", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "connection".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedConnection()
        {
            ConnectionModel model = new ConnectionModel()
            {
                IPAddress = "192.168.1.1",
                Port = 8080
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("connection", model, 1);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("@ConnectionId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@IPAddress", actual[1].ParameterName);
            Assert.AreEqual("192.168.1.1", actual[1].Value);
            Assert.AreEqual("@Port", actual[2].ParameterName);
            Assert.AreEqual(8080, actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "downtime".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedDowntime()
        {
            DowntimeModel model = new DowntimeModel()
            {
                Time = "03:00:00"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("downtime", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@DowntimeId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Time", actual[1].ParameterName);
            Assert.AreEqual("03:00:00", actual[1].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "game".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedGame()
        {
            GameModel model = new GameModel()
            {
                Name = "TestGame",
                Version = "1.0"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("game", model, 1);

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual("@GameId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@Name", actual[1].ParameterName);
            Assert.AreEqual("TestGame", actual[1].Value);
            Assert.AreEqual("@Version", actual[2].ParameterName);
            Assert.AreEqual("1.0", actual[2].Value);
        }

        /// <summary>
        /// Tests whether the GetParametersUpdated method returns the correct parameters when given "machine".
        /// </summary>
        [TestMethod]
        public void TestGetParametersUpdatedMachine()
        {
            MachineModel model = new MachineModel()
            {
                HostName = "TestMachine"
            };
            SqlParameter[] actual = ConfigurationConverter.GetParametersUpdated("machine", model, 1);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@MachineId", actual[0].ParameterName);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual("@HostName", actual[1].ParameterName);
            Assert.AreEqual("TestMachine", actual[1].Value);
        }

        #endregion
    }
}
