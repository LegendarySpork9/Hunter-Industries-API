// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Objects.Configuration;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services
{
    /// <summary>
    /// </summary>
    public class ConfigurationService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public ConfigurationService(ILoggerService _logger,
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
        /// Returns all records that match the parameters.
        /// </summary>
        public async Task<(List<object>, int)> GetRecords(string entity, int id, int? parentEntityId = null, int pageSize = 0, int pageNumber = 0)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.GetRecords called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, id.ToString(), parentEntityId.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<object> records = new List<object>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLGet(entity)}");
                SqlParameter[] parameters;

                if (id != 0)
                {
                    sql += ConfigurationConverter.GetSQLFilterId(entity);
                    parameters = ConfigurationConverter.GetParametersGetSingle(entity, id);
                }

                else if (parentEntityId.HasValue && entity == "applicationSetting")
                {
                    sql += @"
where ApplicationId = @applicationId";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@applicationId", SqlDbType.VarChar) { Value = parentEntityId.Value }
                    };
                }

                else
                {
                    sql += ConfigurationConverter.GetSQLGetPagination(entity);
                    parameters = ConfigurationConverter.GetParametersGet(entity, pageSize, pageNumber);
                }

                Func<IDataReader, object> dataReaderMappings = ConfigurationConverter.GetDataReaderMappings(entity);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(sql, dataReaderMappings, parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run ConfigurationService.GetRecords.";
                        _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                        _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                    }

                    if (entity == "application")
                    {
                        ApplicationRecord current = null;
                        List<ApplicationSettingRecord> settings = new List<ApplicationSettingRecord>();

                        foreach (ApplicationRecord record in results.Cast<ApplicationRecord>())
                        {
                            if (current != null && record.Id == current.Id)
                            {
                                settings.AddRange(record.Settings.Where(s => s.IsDeleted == false));
                            }

                            else
                            {
                                if (current != null)
                                {
                                    current.Settings = settings;
                                    records.Add(current);
                                    settings = new List<ApplicationSettingRecord>();
                                }

                                current = record;
                                settings = new List<ApplicationSettingRecord>(record.Settings);
                            }
                        }

                        if (current != null)
                        {
                            current.Settings = settings;
                            records.Add(current);
                        }
                    }

                    else
                    {
                        records = results;
                    }

                    if (id == 0)
                    {
                        totalRecords = await GetTotalRecord(entity);
                    }
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.GetRecords.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.GetRecords returned {records.Count} records | {totalRecords} total records.");
            return (records, totalRecords);
        }

        /// <summary>
        /// Returns the number of records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalRecord(string entity)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.GetTotalRecord called with the parameters {ParameterFunction.FormatParameters(new string[] { entity })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLGetTotal(entity)}");

                (int result, Exception ex) = await _Database.QuerySingle(sql, reader => reader.GetInt32(0), Array.Empty<SqlParameter>());

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.GetTotalRecord.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.GetTotalRecord.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.GetTotalRecord returned {totalRecords}.");
            return totalRecords;
        }

        /// <summary>
        /// Returns whether a record already exists with the given parameters.
        /// </summary>
        public async Task<bool> RecordExists(string entity, object record, int? parentEntityId = null)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordExists called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, ParameterFunction.FormatParameters(record), parentEntityId.ToString() })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLExists(entity)}");
                sql += ConfigurationConverter.GetSQLFilter(entity);

                SqlParameter[] parameters = ConfigurationConverter.GetParameterExists(entity, record, parentEntityId);

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.RecordExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.RecordExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a record already exists with the given id.
        /// </summary>
        public async Task<bool> RecordExists(string entity, int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordExists called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, id.ToString() })}.");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLExists(entity)}");
                sql += ConfigurationConverter.GetSQLFilterId(entity);

                SqlParameter[] parameters = ConfigurationConverter.GetParameterExists(entity, id);

                (List<int> results, Exception ex) = await _Database.Query(sql, reader => reader.GetInt32(0), parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.RecordExists.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.RecordExists.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        public async Task<(bool, int)> RecordCreated(string entity, object record, int? parentEntityId = null)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordCreated called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, ParameterFunction.FormatParameters(record), parentEntityId.ToString() })}.");

            bool created = true;
            int recordId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLCreate(entity)}");
                SqlParameter[] parameters = ConfigurationConverter.GetParametersCreate(entity, record, parentEntityId);

                (object result, Exception ex) = await _Database.ExecuteScalar(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.RecordCreated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    created = false;
                }

                if (result == null)
                {
                    created = false;
                }

                else
                {
                    recordId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.RecordCreated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                created = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordCreated returned {created}.");
            return (created, recordId);
        }

        /// <summary>
        /// Updates the details of the given record.
        /// </summary>
        public async Task<bool> RecordUpdated(string entity, int id, object record)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, id.ToString(), ParameterFunction.FormatParameters(record) })}.");

            bool updated = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLUpdated(entity)}");
                SqlParameter[] parameters = ConfigurationConverter.GetParametersUpdated(entity, record, id);

                sql = ConfigurationFunction.CleanSQL(record, sql);
                parameters = ConfigurationFunction.CleanParameterArray(record, parameters);

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.RecordUpdated.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                    updated = false;
                }

                if (rowsAffected != 1)
                {
                    updated = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.RecordUpdated.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                updated = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Sets the record to deleted.
        /// </summary>
        public async Task<bool> RecordDeleted(string entity, int id)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordDeleted called with the parameters {ParameterFunction.FormatParameters(new string[] { entity, id.ToString() })}.");

            bool deleted = true;

            try
            {
                string sql = _FileSystem.ReadAllText($@"{_Options.SQLFiles}\Configuration\{ConfigurationConverter.GetSQLDelete(entity)}");
                SqlParameter[] parameters = ConfigurationConverter.GetParametersGetSingle(entity, id);

                (int rowsAffected, Exception ex) = await _Database.Execute(sql, parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run ConfigurationService.RecordDeleted.";
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                    _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);
                }

                if (rowsAffected != 1)
                {
                    deleted = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run ConfigurationService.RecordDeleted.";
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, message);
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString(), message);

                deleted = false;
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"ConfigurationService.RecordDeleted returned {deleted}.");
            return deleted;
        }
    }
}