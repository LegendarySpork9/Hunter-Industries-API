// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Services;

namespace HunterIndustriesAPIControlPanel.Implementations
{
    /// <summary>
    /// </summary>
    public class LoggerServiceWrapper : ILoggerService
    {
        private readonly string IPAddress;

        public LoggerServiceWrapper(string ipAddress)
        {
            IPAddress = ipAddress;
        }

        /// <summary>
        /// Logs the given message to the log file.
        /// </summary>
        public void LogMessage(string level, string message, string summary = null)
        {
            LoggerService _logger = new(IPAddress);
            _logger.LogMessage(level, message, summary);
        }
    }
}