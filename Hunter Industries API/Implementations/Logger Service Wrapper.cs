// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Services;
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
        public void LogMessage(string level, string message, string summary = null)
        {
            LoggerService _logger = new LoggerService(IPAddressFunction.FetchIpAddress(new HttpRequestWrapper(HttpContext.Current.Request)));
            _logger.LogMessage(level, message, summary);
        }
    }
}