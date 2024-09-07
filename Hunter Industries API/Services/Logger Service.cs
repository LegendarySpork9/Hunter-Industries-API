// Copyright © - unpublished - Toby Hunter
using log4net;
using System.Reflection;

namespace HunterIndustriesAPI.Services
{
    public class LoggerService
    {
        private readonly string Identifier;
        private readonly ILog _logger = LogManager.GetLogger("APILog");

        public LoggerService(string id)
        {
            Identifier = id;
        }

        public string FormatParameters(object model)
        {
            string formattedParameters = string.Empty;

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (property.GetValue(model) != null)
                {
                    formattedParameters += $"\"{property.GetValue(model)}\", ";
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(","));
            }

            return formattedParameters;
        }

        public string FormatParameters(string[]? parameters)
        {
            string formattedParameters = string.Empty;

            if (parameters != null)
            {
                foreach (string parameter in parameters)
                {
                    if (!string.IsNullOrWhiteSpace(parameter))
                    {
                        formattedParameters += $"\"{parameter}\", ";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(formattedParameters))
            {
                formattedParameters = formattedParameters.Trim().Remove(formattedParameters.LastIndexOf(",")).Replace("\"\"", "\"");
            }

            return formattedParameters;
        }

        public void LogMessage(string level, string message)
        {
            switch (level)
            {
                case "Info": _logger.Info($"{Identifier} - {message}"); break;
                case "Debug": _logger.Debug($"{Identifier} - {message}"); break;
                case "Warn": _logger.Warn($"{Identifier} - {message}"); break;
                case "Error": _logger.Error($"{Identifier} - {message}"); break;
            }
        }
    }
}
