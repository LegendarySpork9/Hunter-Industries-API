// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Statistics.Shared;
using System;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class SharedDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the endpoint call record.
        /// </summary>
        public static readonly Func<IDataReader, EndpointCallRecord> EndpointCallMapper = reader =>
        {
            EndpointCallRecord endpointCall = new EndpointCallRecord
            {
                Endpoint = reader.GetString(0),
                Calls = reader.GetInt32(1)
            };

            return endpointCall;
        };

        /// <summary>
        /// The SQL row to model mappings for the method call record.
        /// </summary>
        public static readonly Func<IDataReader, MethodCallRecord> MethodCallMapper = reader =>
        {
            MethodCallRecord methodCall = new MethodCallRecord
            {
                Method = reader.GetString(0),
                Calls = reader.GetInt32(1)
            };

            return methodCall;
        };

        /// <summary>
        /// The SQL row to model mappings for the status call record.
        /// </summary>
        public static readonly Func<IDataReader, StatusCallRecord> StatusCallMapper = reader =>
        {
            StatusCallRecord statusCall = new StatusCallRecord
            {
                Status = reader.GetString(0),
                Calls = reader.GetInt32(1)
            };

            return statusCall;
        };

        /// <summary>
        /// The SQL row to model mappings for the change call record.
        /// </summary>
        public static readonly Func<IDataReader, ChangeCallRecord> ChangeCallMapper = reader =>
        {
            ChangeCallRecord changeCall = new ChangeCallRecord
            {
                Field = reader.GetString(0),
                Calls = reader.GetInt32(1)
            };

            return changeCall;
        };
    }
}