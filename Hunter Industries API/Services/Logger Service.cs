// Copyright © - unpublished - Toby Hunter
using log4net;
using HunterIndustriesAPI.Values;

namespace HunterIndustriesAPI.Services
{
    public class LoggerService
    {
        private readonly string OperationId;
        private readonly ILog _logger = LogManager.GetLogger("APILog");

        public LoggerService(string id)
        {
            OperationId = id;
        }

        public void LogMessage(LoggerLevels.LevelValues level, string message)
        {
            switch (level)
            {
                case LoggerLevels.LevelValues.Info: _logger.Info($"{OperationId} - {message}"); break;
                case LoggerLevels.LevelValues.Debug: _logger.Debug($"{OperationId} - {message}"); break;
                case LoggerLevels.LevelValues.Warning: _logger.Warn($"{OperationId} - {message}"); break;
                case LoggerLevels.LevelValues.Error: _logger.Error($"{OperationId} - {message}"); break;
            }
        }
    }
}
