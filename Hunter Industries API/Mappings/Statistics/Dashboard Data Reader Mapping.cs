// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Dashboard;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class DashboardDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the API traffic record.
        /// </summary>
        public static readonly Func<IDataReader, APITrafficRecord> APITrafficMapper = reader =>
        {
            APITrafficRecord apiTraffic = new APITrafficRecord
            {
                Day = reader.GetString(0),
                SuccessfulCalls = reader.GetInt32(1),
                UnsuccessfulCalls = reader.GetInt32(2)
            };

            return apiTraffic;
        };

        /// <summary>
        /// The SQL row to model mappings for the IP and summary error record.
        /// </summary>
        public static readonly Func<IDataReader, IPAndSummaryErrorRecord> IPAndSummaryErrorMapper = reader =>
        {
            IPAndSummaryErrorRecord ipAndSummaryError = new IPAndSummaryErrorRecord()
            {
                IPAddress = reader.GetString(0),
                Summary = reader.GetString(1),
                Errors = reader.GetInt32(2)
            };

            return ipAndSummaryError;
        };

        /// <summary>
        /// The SQL row to model mappings for the login attempt record.
        /// </summary>
        public static readonly Func<IDataReader, LoginAttemptRecord> LoginAttemptMapper = reader =>
        {
            LoginAttemptRecord loginAttempt = new LoginAttemptRecord()
            {
                Username = reader.GetString(0),
                Application = reader.GetString(1),
                SuccessfulAttempts = reader.GetInt32(2),
                UnsuccessfulAttempts = reader.GetInt32(3),
                TotalAttempts = reader.GetInt32(4)
            };

            return loginAttempt;
        };

        /// <summary>
        /// The SQL row to model mappings for the server health overview record.
        /// </summary>
        public static readonly Func<IDataReader, ServerHealthOverviewRecord> ServerHealthOverviewMapper = reader =>
        {
            ServerHealthOverviewRecord serverHealthOverview = new ServerHealthOverviewRecord()
            {
                ServerId = reader.GetInt32(0),
                Events = reader.GetInt32(1),
                Alerts = reader.GetInt32(2)
            };

            return serverHealthOverview;
        };

        /// <summary>
        /// The SQL row to model mappings for the uptime stat in the server health overview record.
        /// </summary>
        public static readonly Func<IDataReader, ServerHealthOverviewRecord> ServerHealthUptimeMapper = reader =>
        {
            ServerHealthOverviewRecord serverHealthUptime = new ServerHealthOverviewRecord()
            {
                ServerId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Uptime = reader.GetFloat(2)
            };

            return serverHealthUptime;
        };

        /// <summary>
        /// The SQL row to model mappings for the top bar stats record.
        /// </summary>
        public static readonly Func<IDataReader, TopBarStatRecord> TopBarStatusMapper = reader =>
        {
            TopBarStatRecord topBarStats = new TopBarStatRecord()
            {
                Applications = reader.GetInt32(0),
                Users = reader.GetInt32(1),
                Calls = reader.GetInt32(2),
                LoginAttempts = reader.GetInt32(3),
                Changes = reader.GetInt32(4),
                Errors = reader.GetInt32(5)
            };

            return topBarStats;
        };
    }
}