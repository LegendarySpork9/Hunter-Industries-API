using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class ChangeService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public ChangeService(ILoggerService _logger,
            IFileSystem _fileSystem,
            IDatabaseOptions _options,
            IDatabase _database)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Options = _options;
            _Database = _database;
        }

        /// <summary>
        /// Creates a record in the Change table.
        /// </summary>
        public async Task<bool> LogChange(int endpointId, int auditId, string field, string oldValue, string newValue)
        {
            ParameterFunction _parameterFunction = new ParameterFunction();

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange called with the parameters {_parameterFunction.FormatParameters(new string[] { endpointId.ToString(), auditId.ToString(), field, oldValue, newValue })}.");

            bool successful = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\LogChange.sql");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@EndpointID", SqlDbType.Int) { Value = endpointId },
                    new SqlParameter("@AuditID", SqlDbType.Int) { Value = auditId },
                    new SqlParameter("@Field", SqlDbType.VarChar) { Value = field },
                    new SqlParameter("@OldValue", SqlDbType.VarChar) { Value = oldValue },
                    new SqlParameter("@NewValue", SqlDbType.VarChar) { Value = newValue }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ChangeService.LogChange.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (rowsAffected == 1)
                {
                    successful = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ChangeService.LogChange.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ChangeService.LogChange returned {successful}.");
            return successful;
        }
    }
}
