// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Error;
using HunterIndustriesAPI.Objects.Statistics.Server;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class ServerDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the alert component record.
        /// </summary>
        public static readonly Func<IDataReader, AlertComponentRecord> AlertComponentMapper = reader =>
        {
            AlertComponentRecord alertComponent = new AlertComponentRecord
            {
                Component = reader.GetString(0),
                Alerts = reader.GetInt32(1)
            };

            return alertComponent;
        };

        /// <summary>
        /// The SQL row to model mappings for the alert status record.
        /// </summary>
        public static readonly Func<IDataReader, AlertStatusRecord> AlertStatusMapper = reader =>
        {
            AlertStatusRecord alertStatus = new AlertStatusRecord
            {
                Status = reader.GetString(0),
                Alerts = reader.GetInt32(1)
            };

            return alertStatus;
        };

        /// <summary>
        /// The SQL row to model mappings for the event component record.
        /// </summary>
        public static readonly Func<IDataReader, EventComponentRecord> EventComponentMapper = reader =>
        {
            EventComponentRecord eventComponent = new EventComponentRecord
            {
                Component = reader.GetString(0),
                Status = reader.GetString(1),
                DateOccured = DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc)
            };

            return eventComponent;
        };

        /// <summary>
        /// The SQL row to model mappings for the recent alert record.
        /// </summary>
        public static readonly Func<IDataReader, RecentAlertRecord> RecentAlertMapper = reader =>
        {
            RecentAlertRecord recentAlert = new RecentAlertRecord
            {
                AlertId = reader.GetInt32(0),
                Reporter = reader.GetString(1),
                Component = reader.GetString(2),
                ComponentStatus = reader.GetString(3),
                AlertStatus = reader.GetString(4),
                AlertDate = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc)
            };

            return recentAlert;
        };
    }
}