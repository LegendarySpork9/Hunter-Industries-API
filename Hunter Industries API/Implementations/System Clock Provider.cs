// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using System;

namespace HunterIndustriesAPI.Implementations
{
    /// <summary>
    /// </summary>
    public class SystemClockProvider :IClock
    {
        /// <summary>
        /// Returns the current UTC Date and time.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Returns the default date and time.
        /// </summary>
        public DateTime DefaultDate => new DateTime(1900, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    }
}