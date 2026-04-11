// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPICommon.Abstractions
{
    /// <summary>
    /// Interface for the logger service.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// </summary>
        void LogMessage(string level, string message, string summary = null);
    }
}
