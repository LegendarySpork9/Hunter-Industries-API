using log4net;
using System.Reflection;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class LoggerService
    {
        private readonly string Identifier;
        private readonly ILog Logger = LogManager.GetLogger("APILog");

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public LoggerService(string id)
        {
            Identifier = id;
        }

        /// <summary>
        /// Converts the model into a log friendly format.
        /// </summary>
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

        /// <summary>
        /// Converts the parameters into a log friendly format.
        /// </summary>
        public string FormatParameters(string[] parameters = null)
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

        /// <summary>
        /// Adds the message to the log file and SQL table.
        /// </summary>
        public void LogMessage(string level, string message, string summary = null)
        {
            switch (level)
            {
                case "Info": Logger.Info($"{Identifier} - {message}"); break;
                case "Debug": Logger.Debug($"{Identifier} - {message}"); break;
                case "Warn": Logger.Warn($"{Identifier} - {message}"); break;
                case "Error": ThreadContext.Properties["IPAddress"] = Identifier; ThreadContext.Properties["Summary"] = summary; Logger.Error(message); break;
            }
        }
    }
}