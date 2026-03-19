// Copyright © - Unpublished - Toby Hunter
using System;

namespace HunterIndustriesAPI.Abstractions
{
    /// <summary>
    /// Interface for the DateTime object.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// </summary>
        DateTime UtcNow { get; }
        /// <summary>
        /// </summary>
        DateTime DefaultDate { get; }
    }
}
