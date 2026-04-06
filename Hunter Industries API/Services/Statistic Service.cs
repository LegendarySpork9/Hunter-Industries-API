// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using System;
using System.Collections.Generic;
using System.Data;
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
        public async Task<object> GetDashboardStatistic(string part)
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
        public async Task<object> GetSharedStatistic(string part)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetSharedStatistic called with the parameters \"{part}\".");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Shared\{StatisticsConverter.GetSQLShared(part)}");
                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsShared(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings);

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
        public async Task<object> GetServerStatistic(string part)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"StatisticService.GetServerStatistic called with the parameters \"{part}\".");

            List<object> records = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Statistics\Server\{StatisticsConverter.GetSQLServer(part)}");
                Func<IDataReader, object> dataReaderMappings = StatisticsConverter.GetDataReaderMappingsServer(part);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings);

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
        public async Task<object> GetErrorStatistic(string part)
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
