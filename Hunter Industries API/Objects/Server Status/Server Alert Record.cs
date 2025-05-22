using System;

namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerAlertRecord
    {
        /// <summary>
        /// The id number of the alert.
        /// </summary>
        public int AlertId { get; set; }
        /// <summary>
        /// The name of the reporter.
        /// </summary>
        public string Reporter { get; set; }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// The status of the component.
        /// </summary>
        public string ComponentStatus { get; set; }
        /// <summary>
        /// The status of the alert.
        /// </summary>
        public string AlertStatus { get; set; }
        /// <summary>
        /// The date and time the alert occured.
        /// </summary>
        public DateTime AlertDate { get; set; }
        /// <summary>
        /// The server information the alert relates to.
        /// </summary>
        public RelatedServerRecord server { get; set; }
    }
}