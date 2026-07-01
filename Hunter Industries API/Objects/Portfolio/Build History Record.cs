// Copyright © - Unpublished - Toby Hunter
using System;

namespace HunterIndustriesAPI.Objects.Portfolio
{
    /// <summary>
    /// </summary>
    public class BuildHistoryRecord
    {
        /// <summary>
        /// The version number of the build.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The date the version was released.
        /// </summary>
        public DateTime ReleaseDate { get; set; }
    }
}