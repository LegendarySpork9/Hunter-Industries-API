// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPICommon.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.API.Converters
{
    [TestClass]
    public class StatisticsConverterTest
    {
        #region GetSQLDashboard

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboard()
        {
            string expected = "Unknown.sql";
            string actual = StatisticsConverter.GetSQLDashboard("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetTopBarStats.sql" when given "topBarStats".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardTopBarStats()
        {
            string expected = "GetTopBarStats.sql";
            string actual = StatisticsConverter.GetSQLDashboard("topBarStats");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetAPITraffic.sql" when given "apiTraffic".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardApiTraffic()
        {
            string expected = "GetAPITraffic.sql";
            string actual = StatisticsConverter.GetSQLDashboard("apiTraffic");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetErrorsByIPAndSummary.sql" when given "errors".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardErrors()
        {
            string expected = "GetErrorsByIPAndSummary.sql";
            string actual = StatisticsConverter.GetSQLDashboard("errors");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetLoginAttemptsByUsernameAndApplication.sql" when given "loginAttempts".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardLoginAttempts()
        {
            string expected = "GetLoginAttemptsByUsernameAndApplication.sql";
            string actual = StatisticsConverter.GetSQLDashboard("loginAttempts");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetServerHealthOverview.sql" when given "serverHealthOverview".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardServerHealthOverview()
        {
            string expected = "GetServerHealthOverview.sql";
            string actual = StatisticsConverter.GetSQLDashboard("serverHealthOverview");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLDashboard method returns "GetServerHealth30Days.sql" when given "serverHealthUptime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLDashboardServerHealthUptime()
        {
            string expected = "GetServerHealth30Days.sql";
            string actual = StatisticsConverter.GetSQLDashboard("serverHealthUptime");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLShared

        /// <summary>
        /// Tests whether the GetSQLShared method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLShared()
        {
            string expected = "Unknown.sql";
            string actual = StatisticsConverter.GetSQLShared("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLShared method returns "GetCallsByEndpoint.sql" when given "endpointCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedEndpointCalls()
        {
            string expected = "GetCallsByEndpoint.sql";
            string actual = StatisticsConverter.GetSQLShared("endpointCalls");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLShared method returns "GetCallsByMethod.sql" when given "methodCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedMethodCalls()
        {
            string expected = "GetCallsByMethod.sql";
            string actual = StatisticsConverter.GetSQLShared("methodCalls");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLShared method returns "GetCallsByStatus.sql" when given "statusCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedStatusCalls()
        {
            string expected = "GetCallsByStatus.sql";
            string actual = StatisticsConverter.GetSQLShared("statusCalls");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLShared method returns "GetChangesByField.sql" when given "changeCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedChangeCalls()
        {
            string expected = "GetChangesByField.sql";
            string actual = StatisticsConverter.GetSQLShared("changeCalls");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLSharedSort

        /// <summary>
        /// Tests whether the GetSQLSharedSort method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedSort()
        {
            string expected = "Unknown.sql";
            string actual = StatisticsConverter.GetSQLSharedSort("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLSharedSort method returns the correct group and order clause when given "endpointCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedSortEndpointCalls()
        {
            string actual = StatisticsConverter.GetSQLSharedSort("endpointCalls");

            StringAssert.Contains(actual, "group by [Value]");
            StringAssert.Contains(actual, "order by EndpointCalls desc");
        }

        /// <summary>
        /// Tests whether the GetSQLSharedSort method returns the correct group and order clause when given "methodCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedSortMethodCalls()
        {
            string actual = StatisticsConverter.GetSQLSharedSort("methodCalls");

            StringAssert.Contains(actual, "group by [Value]");
            StringAssert.Contains(actual, "order by MethodCalls desc");
        }

        /// <summary>
        /// Tests whether the GetSQLSharedSort method returns the correct group and order clause when given "statusCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedSortStatusCalls()
        {
            string actual = StatisticsConverter.GetSQLSharedSort("statusCalls");

            StringAssert.Contains(actual, "group by [Value]");
            StringAssert.Contains(actual, "order by StatusCalls desc");
        }

        /// <summary>
        /// Tests whether the GetSQLSharedSort method returns the correct group and order clause when given "changeCalls".
        /// </summary>
        [TestMethod]
        public void TestGetSQLSharedSortChangeCalls()
        {
            string actual = StatisticsConverter.GetSQLSharedSort("changeCalls");

            StringAssert.Contains(actual, "group by Field");
            StringAssert.Contains(actual, "order by ChangeCount desc");
        }

        #endregion

        #region GetSQLServer

        /// <summary>
        /// Tests whether the GetSQLServer method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLServer()
        {
            string expected = "Unknown.sql";
            string actual = StatisticsConverter.GetSQLServer("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLServer method returns "GetAlertsByComponent.sql" when given "componentAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetSQLServerComponentAlerts()
        {
            string expected = "GetAlertsByComponent.sql";
            string actual = StatisticsConverter.GetSQLServer("componentAlerts");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLServer method returns "GetAlertsByStatus.sql" when given "statusAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetSQLServerStatusAlerts()
        {
            string expected = "GetAlertsByStatus.sql";
            string actual = StatisticsConverter.GetSQLServer("statusAlerts");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLServer method returns "GetLastEventPerComponent.sql" when given "lastComponentEvents".
        /// </summary>
        [TestMethod]
        public void TestGetSQLServerLastComponentEvents()
        {
            string expected = "GetLastEventPerComponent.sql";
            string actual = StatisticsConverter.GetSQLServer("lastComponentEvents");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLServer method returns "GetRecentAlerts.sql" when given "recentAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetSQLServerRecentAlerts()
        {
            string expected = "GetRecentAlerts.sql";
            string actual = StatisticsConverter.GetSQLServer("recentAlerts");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLServer method returns "GetRecentEvents.sql" when given "recentEvents".
        /// </summary>
        [TestMethod]
        public void TestGetSQLServerRecentEvents()
        {
            string expected = "GetRecentEvents.sql";
            string actual = StatisticsConverter.GetSQLServer("recentEvents");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetSQLError

        /// <summary>
        /// Tests whether the GetSQLError method returns "Unknown.sql" when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetSQLError()
        {
            string expected = "Unknown.sql";
            string actual = StatisticsConverter.GetSQLError("Trombone");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLError method returns "GetErrorsOverTime.sql" when given "errorsOverTime".
        /// </summary>
        [TestMethod]
        public void TestGetSQLErrorErrorsOverTime()
        {
            string expected = "GetErrorsOverTime.sql";
            string actual = StatisticsConverter.GetSQLError("errorsOverTime");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLError method returns "GetErrorsByIPAddress.sql" when given "ipErrors".
        /// </summary>
        [TestMethod]
        public void TestGetSQLErrorIpErrors()
        {
            string expected = "GetErrorsByIPAddress.sql";
            string actual = StatisticsConverter.GetSQLError("ipErrors");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetSQLError method returns "GetErrorsBySummary.sql" when given "summaryErrors".
        /// </summary>
        [TestMethod]
        public void TestGetSQLErrorSummaryErrors()
        {
            string expected = "GetErrorsBySummary.sql";
            string actual = StatisticsConverter.GetSQLError("summaryErrors");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetDataReaderMappingsDashboard

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns null when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboard()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("Trombone");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "topBarStats".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardTopBarStats()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("topBarStats");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "apiTraffic".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardApiTraffic()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("apiTraffic");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "errors".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardErrors()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("errors");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "loginAttempts".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardLoginAttempts()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("loginAttempts");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "serverHealthOverview".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardServerHealthOverview()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("serverHealthOverview");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsDashboard method returns a mapping when given "serverHealthUptime".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsDashboardServerHealthUptime()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsDashboard("serverHealthUptime");

            Assert.IsNotNull(actual);
        }

        #endregion

        #region GetDataReaderMappingsShared

        /// <summary>
        /// Tests whether the GetDataReaderMappingsShared method returns null when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsShared()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsShared("Trombone");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsShared method returns a mapping when given "endpointCalls".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsSharedEndpointCalls()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsShared("endpointCalls");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsShared method returns a mapping when given "methodCalls".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsSharedMethodCalls()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsShared("methodCalls");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsShared method returns a mapping when given "statusCalls".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsSharedStatusCalls()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsShared("statusCalls");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsShared method returns a mapping when given "changeCalls".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsSharedChangeCalls()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsShared("changeCalls");

            Assert.IsNotNull(actual);
        }

        #endregion

        #region GetDataReaderMappingsServer

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns null when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServer()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("Trombone");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns a mapping when given "componentAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServerComponentAlerts()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("componentAlerts");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns a mapping when given "statusAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServerStatusAlerts()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("statusAlerts");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns a mapping when given "lastComponentEvents".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServerLastComponentEvents()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("lastComponentEvents");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns a mapping when given "recentAlerts".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServerRecentAlerts()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("recentAlerts");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsServer method returns a mapping when given "recentEvents".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsServerRecentEvents()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsServer("recentEvents");

            Assert.IsNotNull(actual);
        }

        #endregion

        #region GetDataReaderMappingsError

        /// <summary>
        /// Tests whether the GetDataReaderMappingsError method returns null when given any value.
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsError()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsError("Trombone");

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsError method returns a mapping when given "errorsOverTime".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsErrorErrorsOverTime()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsError("errorsOverTime");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsError method returns a mapping when given "ipErrors".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsErrorIpErrors()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsError("ipErrors");

            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Tests whether the GetDataReaderMappingsError method returns a mapping when given "summaryErrors".
        /// </summary>
        [TestMethod]
        public void TestGetDataReaderMappingsErrorSummaryErrors()
        {
            var actual = StatisticsConverter.GetDataReaderMappingsError("summaryErrors");

            Assert.IsNotNull(actual);
        }

        #endregion
    }
}
