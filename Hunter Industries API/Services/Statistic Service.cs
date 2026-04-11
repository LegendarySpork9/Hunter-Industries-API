// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class StatisticService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public StatisticService(ILoggerService _logger,
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
        /// Returns all statistic records that match the parameters.
        /// </summary>
        public async Task<List<object>> GetDashboardStatistic(string part)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetDashboardStatistic called with the parameters \"{part}\".");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Dashboard\{StatisticsConverter.GetSQLDashboard(part)}");
                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsDashboard(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run StatisticService.GetDashboardStatistic.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    records = results;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run StatisticService.GetDashboardStatistic.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetDashboardStatistic returned {records.Count} records.");
            return records;
        }

        /// <summary>
        /// Returns all statistic records that match the parameters.
        /// </summary>
        public async Task<List<object>> GetSharedStatistic(string part, string type = null, int id = 0)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetSharedStatistic called with the parameters {ParameterFunction.FormatParameters(new string[] { part, type, id.ToString() })}.");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Shared\{StatisticsConverter.GetSQLShared(part)}");
                SqlParameter[] parameters = Array.Empty< SqlParameter>();

                if (!string.IsNullOrWhiteSpace(type))
                {
                    if (type == "application")
                    {
                        sql += @"
and ApplicationId = @applicationId";
                        parameters = new SqlParameter[]
                        {
                            new SqlParameter("@applicationId", SqlDbType.VarChar) { Value = id }
                        };
                    }

                    else
                    {
                        sql += @"
and UserId = @userId";
                        parameters = new SqlParameter[]
                        {
                            new SqlParameter("@userId", SqlDbType.VarChar) { Value = id }
                        };
                    }
                }

                sql += StatisticsConverter.GetSQLSharedSort(part);

                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsShared(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings, parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run StatisticService.GetSharedStatistic.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    records = results;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run StatisticService.GetSharedStatistic.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetSharedStatistic returned {records.Count} records.");
            return records;
        }

        /// <summary>
        /// Returns all statistic records that match the parameters.
        /// </summary>
        public async Task<List<object>> GetServerStatistic(string part, int server)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetServerStatistic called with the parameters \"{part}\", \"{server}\".");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Server\{StatisticsConverter.GetSQLServer(part)}");
                SqlParameter[] parameters =
{
                    new SqlParameter("@serverId", SqlDbType.Int) { Value = server }
                };
                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsServer(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings, parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run StatisticService.GetServerStatistic.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    records = results;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run StatisticService.GetServerStatistic.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetServerStatistic returned {records.Count} records.");
            return records;
        }

        /// <summary>
        /// Returns all statistic records that match the parameters.
        /// </summary>
        public async Task<List<object>> GetErrorStatistic(string part)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetErrorStatistic called with the parameters \"{part}\".");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Error\{StatisticsConverter.GetSQLError(part)}");
                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsError(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run StatisticService.GetErrorStatistic.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    records = results;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run StatisticService.GetErrorStatistic.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetErrorStatistic returned {records.Count} records.");
            return records;
        }
    }
}
