// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPICommon.Abstractions
{
    /// <summary>
    /// Interface for the DateTime object.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime DefaultDate { get; }
    }
}
