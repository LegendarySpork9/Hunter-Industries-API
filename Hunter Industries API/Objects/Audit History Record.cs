using System;

namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class AuditHistoryRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// IP Address the call was made from.
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// The endpoint that was called.
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// The method of the call.
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// The status the call returned.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// The date and time the call was made.
        /// </summary>
        public DateTime OccuredAt { get; set; }
        /// <summary>
        /// Any filters or body input attached to the call.
        /// </summary>
        public string[] Paramaters { get; set; } = null;
    }
}