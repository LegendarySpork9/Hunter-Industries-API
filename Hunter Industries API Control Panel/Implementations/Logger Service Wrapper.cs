// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Services;

namespace HunterIndustriesAPIControlPanel.Implementations
{
    /// <summary>
    /// </summary>
    public class LoggerServiceWrapper : IConfigurableLoggerService
    {
        private string IPAddress;

        public LoggerServiceWrapper(string ipAddress)
        {
            IPAddress = ipAddress;
        }

        /// <summary>
        /// Changes the identifier of the logger.
        /// </summary>
        public void ChangeIdentifier(string value) => IPAddress = value;

        /// <summary>
        /// Logs the given message to the log file.
        /// </summary>
        public void LogMessage(string level, string message, string summary = null)
        {
            LoggerService _logger = new(IPAddress, "Logs");
            _logger.LogMessage(level, message, summary);
        }
    }
}