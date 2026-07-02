// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
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
    public class PortfolioService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public PortfolioService(
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
        /// Returns all item records that match the parameters.
        /// </summary>
        public async Task<List<ItemRecord>> GetItems(bool includeDeleted = false)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.GetItems called with the parameter \"{includeDeleted}\".");

            List<ItemRecord> items = new List<ItemRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "GetItems.sql"));

                if (!includeDeleted)
                {
                    sql += "\nwhere IsDeleted = 0";
                }

                (List<ItemRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader =>
                    {
                        string demoURL = null;

                        if (!reader.IsDBNull(6) && !string.IsNullOrWhiteSpace(reader.GetString(6)))
                        {
                            demoURL = reader.GetString(6);
                        }

                        return new ItemRecord()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2),
                            IconURL = reader.GetString(3),
                            Summary = reader.GetString(4),
                            Description = reader.GetString(5),
                            DemoURL = demoURL,
                            ReleaseNotes = reader.GetString(7),
                            UnitTestCoverage = reader.GetDecimal(8),
                            LLMUsage = new LLMRecord()
                            {
                                Company = reader.GetString(9),
                                Model = reader.GetString(10),
                            },
                            LLMUsageNotes = reader.GetString(11),
                            DateCreated = DateTime.SpecifyKind(
                                reader.GetDateTime(12),
                                DateTimeKind.Utc),
                            DateUpdated = DateTime.SpecifyKind(
                                reader.GetDateTime(13),
                                DateTimeKind.Utc),
                            IsDeleted = reader.GetBoolean(14)
                        };
                    });

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.GetItems.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                items = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.GetItems.";
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
                $"PortfolioService.GetItems returned {items.Count} records.");
            return items;
        }

        /// <summary>
        /// Returns all linked item records that match the parameters.
        /// </summary>
        public async Task<List<object>> GetLinkedItemData(
            string linkedItem,
            int? itemId = null,
            bool includeDeleted = false)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.GetAllLinkedItemData called with the parameters \"{linkedItem}\", \"{includeDeleted}\".");

            List<object> linkedItemData = new List<object>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    PortfolioConverter.GetSQLGet(linkedItem)));
                SqlParameter[] parameters = null;

                if (itemId.HasValue)
                {
                    sql += "\nwhere [PI].PortfolioItemId = @itemId";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId.Value }
                    };
                }

                if (!includeDeleted)
                {
                    sql += "\nwhere IsDeleted = 0";
                }

                Func<IDataReader, object> dataReaderMappings = PortfolioConverter.GetDataReaderMappings(linkedItem);

                if (dataReaderMappings != null)
                {
                    (List<object> results, Exception ex) = await _Database.Query(
                    sql,
                    dataReaderMappings,
                    parameters);

                    if (ex != null)
                    {
                        string message = "An error occured when trying to run PortfolioService.GetAllLinkedItemData.";
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Warning,
                            message);
                        _Logger.LogMessage(
                            StandardValues.LoggerValues.Error,
                            ex.ToString(),
                            message);
                    }

                    linkedItemData = results;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.GetAllLinkedItemData.";
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
                $"PortfolioService.GetAllLinkedItemData returned {linkedItemData.Count} records .");
            return linkedItemData;
        }

        /// <summary>
        /// Returns the item record that matches the id.
        /// </summary>
        public async Task<ItemRecord> GetItem(int itemId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.GetItem called with the parameter \"{itemId}\".");

            ItemRecord item = new ItemRecord();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "GetItem.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId }
                };

                (ItemRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader =>
                    {
                        string demoURL = null;

                        if (!reader.IsDBNull(6) && !string.IsNullOrWhiteSpace(reader.GetString(6)))
                        {
                            demoURL = reader.GetString(6);
                        }

                        return new ItemRecord()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Type = reader.GetString(2),
                            IconURL = reader.GetString(3),
                            Summary = reader.GetString(4),
                            Description = reader.GetString(5),
                            DemoURL = demoURL,
                            ReleaseNotes = reader.GetString(7),
                            UnitTestCoverage = reader.GetDecimal(8),
                            LLMUsage = new LLMRecord()
                            {
                                Company = reader.GetString(9),
                                Model = reader.GetString(10),
                            },
                            LLMUsageNotes = reader.GetString(11),
                            DateCreated = DateTime.SpecifyKind(
                                reader.GetDateTime(12),
                                DateTimeKind.Utc),
                            DateUpdated = DateTime.SpecifyKind(
                                reader.GetDateTime(13),
                                DateTimeKind.Utc),
                            IsDeleted = reader.GetBoolean(14)
                        };
                    });

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.GetItem.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                item = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.GetItem.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (item != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "PortfolioService.GetItem returned 1 record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "PortfolioService.GetItem returned 0 records");
            }

            return item;
        }

        /// <summary>
        /// Returns whether an item record already exists with the given name.
        /// </summary>
        public async Task<bool> ItemExists(string name)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.ItemExists called with the parameter \"{name}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "ItemExists.sql"));
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
                    string message = "An error occured when trying to run PortfolioService.ItemExists.";
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
                string message = "An error occured when trying to run PortfolioService.ItemExists.";
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
                $"PortfolioService.ItemExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether an item record exists with the given id.
        /// </summary>
        public async Task<bool> ItemExists(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.ItemExists called with the parameter \"{id}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "ItemExists.sql"));
                sql += "\nand PortfolioItemId = @itemId";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@itemId", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.ItemExists.";
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
                string message = "An error occured when trying to run PortfolioService.ItemExists.";
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
                $"PortfolioService.ItemExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Creates the item.
        /// </summary>
        public async Task<(bool, int)> ItemCreated(ItemModel item)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.ItemCreated called with the parameters {ParameterFunction.FormatParameters(item)}.");

            bool created = true;
            int itemId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "CreateItem.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = item.Name },
                    new SqlParameter("@summary", SqlDbType.VarChar) { Value = item.Summary },
                    new SqlParameter("@description", SqlDbType.VarChar) { Value = item.Description },
                    new SqlParameter("@icon", SqlDbType.VarChar) { Value = item.IconURL },
                    new SqlParameter("@releaseNotes", SqlDbType.VarChar) { Value = item.ReleaseNotes },
                    new SqlParameter("@gitHub", SqlDbType.VarChar) { Value = item.GitHubURL },
                    new SqlParameter("@demo", SqlDbType.VarChar) { Value = item.DemoURL },
                    new SqlParameter("@unitTestCoverage", SqlDbType.Decimal) { Value = item.UnitTestCoverage },
                    new SqlParameter("@llmUsageNotes", SqlDbType.VarChar) { Value = item.LLMUsageNotes },
                    new SqlParameter("@model", SqlDbType.VarChar) { Value = item.LLMUsage.Model },
                    new SqlParameter("@company", SqlDbType.VarChar) { Value = item.LLMUsage.Company },
                    new SqlParameter("@type", SqlDbType.VarChar) { Value = item.Type },
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.ItemCreated.";
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
                    itemId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.ItemCreated.";
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
                $"PortfolioService.ItemCreated returned {created}.");
            return (
                created,
                itemId);
        }

        /// <summary>
        /// Creates the linked item records
        /// </summary>
        public async Task<bool> LinkedItemDataCreated(
            string linkedItem,
            int itemId,
            object record)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.LinkedItemDataCreated called with the parameters {ParameterFunction.FormatParameters(new string[] { linkedItem, itemId.ToString(), ParameterFunction.FormatParameters(record) })}.");

            bool created = true;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    PortfolioConverter.GetSQLCreate(linkedItem)));
                SqlParameter[] parameters = PortfolioConverter.GetParametersCreate(
                    linkedItem,
                    itemId,
                    record);

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.LinkedItemDataCreated.";
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
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.LinkedItemDataCreated.";
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
                $"PortfolioService.LinkedItemDataCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Deletes the link between the item and linked record
        /// </summary>
        public async Task<bool> LinkItemDataDeleted(
            string linkedItem,
            int itemId,
            string data)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.LinkItemDataDeleted called with the parameters \"{linkedItem}\", \"{itemId}\", \"{data}\".");

            bool deleted = true;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    PortfolioConverter.GetSQLDeleteLink(linkedItem)));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId }
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.LinkItemDataDeleted.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);

                    deleted = false;
                }

                if (result == null)
                {
                    deleted = false;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.LinkItemDataDeleted.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);

                deleted = false;
            }

            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.LinkItemDataDeleted returned {deleted}.");
            return deleted;
        }

        /// <summary>
        /// Creates the link between the item and linked record
        /// </summary>
        public async Task<bool> LinkItemDataCreated(
            string linkedItem,
            int itemId,
            string data)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.LinkItemDataCreated called with the parameters \"{linkedItem}\", \"{itemId}\", \"{data}\".");

            bool created = true;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    PortfolioConverter.GetSQLCreateLink(linkedItem)));
                SqlParameter[] parameters = PortfolioConverter.GetParametersCreateLink(
                    linkedItem,
                    itemId,
                    data);

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.LinkItemDataCreated.";
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
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run PortfolioService.LinkItemDataCreated.";
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
                $"PortfolioService.LinkItemDataCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Updates the details of the given item.
        /// </summary>
        public async Task<bool> ItemUpdated(
            int id,
            ItemModel item)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.ItemUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), ParameterFunction.FormatParameters(item) })}.");

            bool updated = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "ItemUpdated.sql"));
                List<SqlParameter> parameterList = new List<SqlParameter>()
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = item.Name },
                    new SqlParameter("@type", SqlDbType.VarChar) { Value = item.Type },
                    new SqlParameter("@icon", SqlDbType.VarChar) { Value = item.IconURL },
                    new SqlParameter("@summary", SqlDbType.VarChar) { Value = item.Summary },
                    new SqlParameter("@description", SqlDbType.VarChar) { Value = item.Description },
                    new SqlParameter("@demo", SqlDbType.VarChar) { Value = (object)item.DemoURL ?? DBNull.Value },
                    new SqlParameter("@releaseNotes", SqlDbType.VarChar) { Value = item.ReleaseNotes },
                    new SqlParameter("@unitTestCoverage", SqlDbType.Decimal) { Value = item.UnitTestCoverage },
                    new SqlParameter("@gitHub", SqlDbType.VarChar) { Value = item.GitHubURL },
                    new SqlParameter("@llmUsageNotes", SqlDbType.VarChar) { Value = item.LLMUsageNotes },
                    new SqlParameter("@model", SqlDbType.VarChar) { Value = item.LLMUsage.Model },
                    new SqlParameter("@company", SqlDbType.VarChar) { Value = item.LLMUsage.Company },
                    new SqlParameter("@itemId", SqlDbType.Int) { Value = id }
                };

                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    sql = sql.Replace(@"
	[Name] = @name,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@name"));

                }

                if (string.IsNullOrWhiteSpace(item.Type))
                {
                    sql = sql.Replace(@"
	TypeId = PIT.PortfolioItemTypeId,", "")
                        .Replace(@"
join PortfolioItemType PIT with (nolock) on [PI].TypeId = @type", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@type"));

                }

                if (string.IsNullOrWhiteSpace(item.IconURL))
                {
                    sql = sql.Replace(@"
	IconURL = @icon,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@icon"));

                }

                if (string.IsNullOrWhiteSpace(item.Summary))
                {
                    sql = sql.Replace(@"
	Summary = @summary,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@summary"));

                }

                if (string.IsNullOrWhiteSpace(item.Description))
                {
                    sql = sql.Replace(@"
	[Description] = @description,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@description"));

                }

                if (string.IsNullOrWhiteSpace(item.DemoURL))
                {
                    sql = sql.Replace(@"
	DemoLink = @demo,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@demo"));

                }

                if (string.IsNullOrWhiteSpace(item.ReleaseNotes))
                {
                    sql = sql.Replace(@"
	ReleaseNotes = @releaseNotes,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@releaseNotes"));

                }

                if (item.UnitTestCoverage == default)
                {
                    sql = sql.Replace(@"
	UnitTestCoverage = @unitTestCoverage,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@unitTestCoverage"));

                }

                if (string.IsNullOrWhiteSpace(item.GitHubURL))
                {
                    sql = sql.Replace(@"
	GitHubLink = @gitHub,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@gitHub"));

                }

                if (string.IsNullOrWhiteSpace(item.LLMUsageNotes))
                {
                    sql = sql.Replace(@"
	LLMUsageNotes = @llmUsageNotes,", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@llmUsageNotes"));

                }

                if (string.IsNullOrWhiteSpace(item.LLMUsage.Company) || string.IsNullOrWhiteSpace(item.LLMUsage.Model))
                {
                    sql = sql.Replace(@"
	LLMModelId = LLMModel.LLMModelId,", "")
                        .Replace(@"
left join LLMModel with (nolock) on LLMModel.[Name] = @model
join LLMCompany with (nolock) on LLMModel.LLMCompanyId = LLMCompany.LLMCompanyId
	and LLMCompany.[Name] = @company", "");
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@company"));
                    parameterList.RemoveAt(parameterList.FindIndex(p => p.ParameterName == "@model"));
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
                    string message = "An error occured when trying to run PortfolioService.ItemUpdated.";
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
                string message = "An error occured when trying to run PortfolioService.ItemUpdated.";
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
                $"PortfolioService.ItemUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Sets the item to deleted.
        /// </summary>
        public async Task<bool> ItemDeleted(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"PortfolioService.ItemDeleted called with the parameter \"{id}\".");

            bool deleted = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "Item",
                    "DeleteItem.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@itemId", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run PortfolioService.ItemDeleted.";
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
                string message = "An error occured when trying to run PortfolioService.ItemDeleted.";
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
                $"PortfolioService.ItemDeleted returned {deleted}.");
            return deleted;
        }
    }
}