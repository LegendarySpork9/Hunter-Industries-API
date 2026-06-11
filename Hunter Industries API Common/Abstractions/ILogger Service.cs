// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPICommon.Abstractions
{
    /// <summary>
    /// Interface for the logger service.
    /// </summary>
    public interface ILoggerService
    {
        void LogMessage(string level, string message, string summary = null);
    }
}
