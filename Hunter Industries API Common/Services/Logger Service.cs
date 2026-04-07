// Copyright © - Unpublished - Toby Hunter
using log4net;

namespace HunterIndustriesAPICommon.Services
{
    /// <summary>
    /// </summary>
    public class LoggerService
    {
        private readonly string Identifier;
        private readonly ILog Logger = LogManager.GetLogger("APILog");

        // Sets the class's global variables.
        public LoggerService(string id)
        {
            Identifier = id;
        }

        /// <summary>
        /// Adds the message to the log file and SQL table.
        /// </summary>
        public void LogMessage(string level, string message, string summary = null)
        {
            switch (level)
            {
                case "Info": Logger.Info($"{Identifier} - {message.Trim()}"); break;
                case "Debug": Logger.Debug($"{Identifier} - {message.Trim()}"); break;
                case "Warn": Logger.Warn($"{Identifier} - {message.Trim()}"); break;
                case "Error": ThreadContext.Properties["IPAddress"] = Identifier; ThreadContext.Properties["Summary"] = summary; Logger.Error(message); break;
            }
        }
    }
}