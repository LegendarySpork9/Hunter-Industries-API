// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Portfolio
{
    /// <summary>
    /// </summary>
    public class FilterService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public FilterService(
            ILoggerService _logger,
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
        /// Returns all filter records that match the parameters.
        /// </summary>
        public async Task<List<FilterRecord>> GetFilters(bool includeDeleted = false)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.GetFilters called with the parameter \"{includeDeleted}\".");

            List<FilterRecord> filters = new List<FilterRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "GetFilters.sql"));

                if (!includeDeleted)
                {
                    sql += "\nwhere IsDeleted = 0";
                }

                (List<FilterRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => new FilterRecord()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Type = reader.GetString(2),
                        Operator = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Path = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Values = reader.IsDBNull(5)
                            ? null
                            : reader.GetString(5)
                                .Split(',')
                                .ToList(),
                        IsDeleted = reader.GetBoolean(6)
                    });

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.GetFilters.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                filters = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.GetFilters.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.GetFilters returned {filters.Count} records.");
            return filters;
        }

        /// <summary>
        /// Returns the filter record that matches the id.
        /// </summary>
        public async Task<FilterRecord> GetFilter(int filterId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.GetFilter called with the parameter \"{filterId}\".");

            FilterRecord filter = new FilterRecord();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "GetFilters.sql"));
                sql += "\nwhere PortfolioFilterId = @filterId";
                
                SqlParameter[] parameters =
                {
                    new SqlParameter("@filterId", SqlDbType.Int) { Value = filterId }
                };

                (FilterRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader => new FilterRecord()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Type = reader.GetString(2),
                        Operator = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Path = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Values = reader.IsDBNull(5)
                            ? null
                            : reader.GetString(5)
                                .Split(',')
                                .ToList(),
                        IsDeleted = reader.GetBoolean(6)
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.GetFilter.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                filter = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.GetFilter.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (filter != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "FilterService.GetFilter returned 1 record.");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "FilterService.GetFilter returned 0 records.");
            }

            return filter;
        }

        /// <summary>
        /// Returns whether a filter record already exists with the given name.
        /// </summary>
        public async Task<bool> FilterExists(string name)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterExists called with the parameter \"{name}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "FilterExists.sql"));
                sql += "\nand [Name] = @name";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = name }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.FilterExists.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.FilterExists.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a filter record exists with the given id.
        /// </summary>
        public async Task<bool> FilterExists(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterExists called with the parameter \"{id}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "FilterExists.sql"));
                sql += "\nand PortfolioFilterId = @filterId";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@filterId", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.FilterExists.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (results.Count > 0)
                {
                    exists = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.FilterExists.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the filter.
        /// </summary>
        public async Task<(bool, int)> FilterCreated(FilterModel filter)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterCreated called with the parameters {ParameterFunction.FormatParameters(filter)}.");

            bool created = true;
            int filterId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "CreateFilter.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = filter.Name },
                    new SqlParameter("@type", SqlDbType.VarChar) { Value = filter.Type ?? "tag" },
                    new SqlParameter("@operator", SqlDbType.VarChar) { Value = (object)filter.Operator ?? DBNull.Value },
                    new SqlParameter("@path", SqlDbType.VarChar) { Value = (object)filter.Path ?? DBNull.Value },
                    new SqlParameter("@values", SqlDbType.VarChar) { Value = (object)filter.Values ?? DBNull.Value }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.FilterCreated.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);

                    created = false;
                }

                if (result == null)
                {
                    created = false;
                }

                else
                {
                    filterId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.FilterCreated.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);

                created = false;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterCreated returned {created}.");
            return (
                created,
                filterId);
        }

        /// <summary>
        /// Updates the details of the given filter.
        /// </summary>
        public async Task<bool> FilterUpdated(
            int id,
            FilterModel filter)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), ParameterFunction.FormatParameters(filter) })}.");

            bool updated = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "FilterUpdated.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>()
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = filter.Name },
                    new SqlParameter("@type", SqlDbType.VarChar) { Value = filter.Type },
                    new SqlParameter("@operator", SqlDbType.VarChar) { Value = (object)filter.Operator ?? DBNull.Value },
                    new SqlParameter("@path", SqlDbType.VarChar) { Value = (object)filter.Path ?? DBNull.Value },
                    new SqlParameter("@values", SqlDbType.VarChar) { Value = filter.Values },
                    new SqlParameter("@filterId", SqlDbType.Int) { Value = id }
                };

                if (string.IsNullOrWhiteSpace(filter.Name))
                {
                    sql = sql.Replace(@"
	[Name] = @name,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@name"));

                }

                if (string.IsNullOrWhiteSpace(filter.Type))
                {
                    sql = sql.Replace(@"
	[Type] = @type,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@type"));
                }

                if (string.IsNullOrWhiteSpace(filter.Operator))
                {
                    sql = sql.Replace(@"
	[Operator] = @operator,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@operator"));
                }

                if (string.IsNullOrWhiteSpace(filter.Path))
                {
                    sql = sql.Replace(@"
	[Path] = @path,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@path"));
                }

                if (string.IsNullOrWhiteSpace(filter.Values))
                {
                    sql = sql.Replace(@"
	[Values] = @values", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@values"));
                }

                List<string> sqlLines = sql.Split(new[]
                {
                    Environment.NewLine,
                    "\n",
                    "\r\n"
                },
                StringSplitOptions.None)
                    .ToList();
                string where = sqlLines.LastOrDefault(s => s.Contains("where"));

                if (where != null)
                {
                    int lastSetIndex = sqlLines.IndexOf(where) - 1;
                    string lastSet = sqlLines[lastSetIndex];

                    if (lastSet.Contains(','))
                    {
                        sqlLines[lastSetIndex] = sqlLines[lastSetIndex].Replace(",", "");
                    }
                }

                sql = string.Join(
                    Environment.NewLine,
                    sqlLines);

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameterList.ToArray());

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.FilterUpdated.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (rowsAffected == 1)
                {
                    updated = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.FilterUpdated.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Sets the filter to deleted.
        /// </summary>
        public async Task<bool> FilterDeleted(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterDeleted called with the parameter \"{id}\".");

            bool deleted = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Filter",
                    "DeleteFilter.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@filterId", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run FilterService.FilterDeleted.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                if (rowsAffected == 1)
                {
                    deleted = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run FilterService.FilterDeleted.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"FilterService.FilterDeleted returned {deleted}.");
            return deleted;
        }
    }
}