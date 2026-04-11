// Copyright © - Unpublished - Toby Hunter
using System;

namespace HunterIndustriesAPI.Objects.Statistics.Server
{
    /// <summary>
    /// </summary>
    public class EventComponentRecord
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// The name of the status.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// When the event occured.
        /// </summary>
        public DateTime DateOccured { get; set; }
    }
}