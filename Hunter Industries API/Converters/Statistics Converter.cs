// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings;
using System;
using System.Data;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class StatisticsConverter
    {
        #region SQL

        /// <summary>
        /// Returns the Getx sql file to load for the dashboard.
        /// </summary>
        public static string GetSQLDashboard(string part)
        {
            switch (part)
            {
                case "topBarStats": return "GetTopBarStats.sql";
                case "apiTraffic": return "GetAPITraffic.sql";
                case "errors": return "GetErrorsByIPAndSummary.sql";
                case "loginAttempts": return "GetLoginAttemptsByUsernameAndApplication.sql";
                case "serverHealthOverview": return "GetServerHealthOverview.sql";
                case "serverHealthUptime": return "GetServerHealth30Days.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Getx sql file to load for the shared stats.
        /// </summary>
        public static string GetSQLShared(string part)
        {
            switch (part)
            {
                case "endpointCalls": return "GetCallsByEndpoint.sql";
                case "methodCalls": return "GetCallsByMethod.sql";
                case "statusCalls": return "GetCallsByStatus.sql";
                case "changeCalls": return "GetChangesByField.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Getx group query.
        /// </summary>
        public static string GetSQLSharedSort(string part)
        {
            switch (part)
            {
                case "endpointCalls": return @"
group by [Value]
order by EndpointCalls desc";
                case "methodCalls": return @"
group by [Value]
order by MethodCalls desc";
                case "statusCalls": return @"
group by [Value]
order by StatusCalls desc";
                case "changeCalls": return @"
group by Field
order by ChangeCount desc";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Getx sql file to load for the server page.
        /// </summary>
        public static string GetSQLServer(string part)
        {
            switch (part)
            {
                case "componentAlerts": return "GetAlertsByComponent.sql";
                case "statusAlerts": return "GetAlertsByStatus.sql";
                case "lastComponentEvents": return "GetLastEventPerComponent.sql";
                case "recentAlerts": return "GetRecentAlerts.sql";
                case "recentEvents": return "GetRecentEvents.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Getx sql file to load for the error page.
        /// </summary>
        public static string GetSQLError(string part)
        {
            switch (part)
            {
                case "errorsOverTime": return "GetErrorsOverTime.sql";
                case "ipErrors": return "GetErrorsByIPAddress.sql";
                case "summaryErrors": return "GetErrorsBySummary.sql";
                default: return "Unknown.sql";
            }
        }

        #endregion

        #region Mappings

        /// <summary>
        /// Returns the data reader mappings for the dashboard.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappingsDashboard(string part)
        {
            switch (part)
            {
                case "topBarStats": return DashboardDataReaderMapping.TopBarStatusMapper;
                case "apiTraffic": return DashboardDataReaderMapping.APITrafficMapper;
                case "errors": return DashboardDataReaderMapping.IPAndSummaryErrorMapper;
                case "loginAttempts": return DashboardDataReaderMapping.LoginAttemptMapper;
                case "serverHealthOverview": return DashboardDataReaderMapping.ServerHealthOverviewMapper;
                case "serverHealthUptime": return DashboardDataReaderMapping.ServerHealthUptimeMapper;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the data reader mappings for the shared stats.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappingsShared(string part)
        {
            switch (part)
            {
                case "endpointCalls": return SharedDataReaderMapping.EndpointCallMapper;
                case "methodCalls": return SharedDataReaderMapping.MethodCallMapper;
                case "statusCalls": return SharedDataReaderMapping.StatusCallMapper;
                case "changeCalls": return SharedDataReaderMapping.ChangeCallMapper;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the data reader mappings for the server page.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappingsServer(string part)
        {
            switch (part)
            {
                case "componentAlerts": return ServerDataReaderMapping.AlertComponentMapper;
                case "statusAlerts": return ServerDataReaderMapping.AlertStatusMapper;
                case "lastComponentEvents": return ServerDataReaderMapping.EventComponentMapper;
                case "recentAlerts": return ServerDataReaderMapping.RecentAlertMapper;
                case "recentEvents": return ServerDataReaderMapping.EventComponentMapper;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the data reader mappings for the error page.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappingsError(string part)
        {
            switch (part)
            {
                case "errorsOverTime": return ErrorDataReaderMapping.ErrorOverTimeMapper;
                case "ipErrors": return ErrorDataReaderMapping.IPErrorMapper;
                case "summaryErrors": return ErrorDataReaderMapping.SummaryErrorMapper;
                default: return null;
            }
        }

        #endregion
    }
}