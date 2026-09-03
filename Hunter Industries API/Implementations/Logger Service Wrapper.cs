// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Services;
using System.Web;

namespace HunterIndustriesAPI.Implementations
{
    /// <summary>
    /// </summary>
    public class LoggerServiceWrapper : ILoggerService
    {
        /// <summary>
        /// Logs the given message to the log file.
        /// </summary>
        public void LogMessage(
            string level,
            string message,
            string summary = null)
        {
            string ipAddress = HttpContext.Current?.Request?.Headers["CF-Connecting-IP"]
                ?? HttpContext.Current?.Request?.Headers["X-Forwarded-For"]
                ?? HttpContext.Current?.Request?.UserHostAddress
                ?? "Unknown";

            LoggerService _logger = new LoggerService(
                ipAddress,
                "APILog");
            _logger.LogMessage(
                level,
                message,
                summary);
        }
    }
}