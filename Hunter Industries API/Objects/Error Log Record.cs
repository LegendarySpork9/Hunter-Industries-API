// Copyright © - Unpublished - Toby Hunter
using System;

namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class ErrorLogRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// When the error happened.
        /// </summary>
        public DateTime DateOccured { get; set; }
        /// <summary>
        /// IP Address the call was made from.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// A short message about the error.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// The full error including the trace.
        /// </summary>
        public string Message { get; set; }
    }
}