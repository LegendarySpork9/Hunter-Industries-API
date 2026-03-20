// Copyright © - Unpublished - Toby Hunter
using System;

namespace HunterIndustriesAPI.Objects.ServerStatus
{
    /// <summary>
    /// </summary>
    public class ServerEventRecord
    {
        /// <summary>
        /// The id number of the event.
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// The status of the component.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// When the event occured.
        /// </summary>
        public DateTime DateOccured { get; set; }
        /// <summary>
        /// The server information the event relates to.
        /// </summary>
        public RelatedServerRecord Server { get; set; }
    }
}