// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters;
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Media;
using HunterIndustriesAPI.Objects.Media;
using HunterIndustriesAPICommon.Abstractions;
using HunterIndustriesAPICommon.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Services.Media
{
    /// <summary>
    /// </summary>
    public class MediaService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public MediaService(
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
        /// Returns all media records that match the parameters.
        /// </summary>
        public async Task<(List<MediaRecord>, int)> GetApplicationMedia(
            string application,
            int pageSize,
            int pageNumber,
            bool includeDeleted = false)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.GetApplicationMedia called with the parameters {ParameterFunction.FormatParameters(new string[] { application, includeDeleted.ToString(), pageSize.ToString(), pageNumber.ToString() })}.");

            List<MediaRecord> media = new List<MediaRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "GetApplicationMedia.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = application },
                    new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                    new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                };

                if (!includeDeleted)
                {
                    sql += "\nand Media.IsDeleted = 0";
                }

                (List<MediaRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => new MediaRecord
                    {
                        Id = reader.GetInt32(0),
                        Type = new MediaTypeRecord
                        {
                            Entension = reader.GetString(1),
                            MimeType = reader.GetString(2)
                        },
                        Name = reader.GetString(3),
                        Size = reader.GetInt64(4),
                        Path = reader.GetString(5),
                        Domain = reader.GetString(6),
                        URL = $"{reader.GetString(6)}{reader.GetString(5)}/{reader.GetString(3)}{reader.GetString(1)}",
                        Application = reader.GetString(7),
                        DateUploaded = DateTime.SpecifyKind(
                            reader.GetDateTime(8),
                            DateTimeKind.Utc),
                        DateUpdated = DateTime.SpecifyKind(
                            reader.GetDateTime(9),
                            DateTimeKind.Utc),
                        IsDeleted = reader.GetBoolean(10),
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.GetApplicationMedia.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                media = results;
                totalRecords = await GetTotalMedia(
                    application,
                    includeDeleted);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.GetApplicationMedia.";
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
                $"MediaService.GetApplicationMedia returned {media.Count} records | {totalRecords} total records.");
            return (
                media,
                totalRecords);
        }

        /// <summary>
        /// Returns the number of media records that match the parameters.
        /// </summary>
        private async Task<int> GetTotalMedia(
            string application,
            bool includeDeleted)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.GetTotalMedia called with the parameters {ParameterFunction.FormatParameters(new string[] { application, includeDeleted.ToString() })}.");

            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "GetTotalMedia.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = application }
                };

                if (!includeDeleted)
                {
                    sql += "\nand Media.IsDeleted = 0";
                }

                (int result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.GetTotalMedia.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                totalRecords = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.GetTotalMedia.";
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
                $"MediaService.GetTotalMedia returned {totalRecords}.");
            return totalRecords;
        }

        /// <summary>
        /// Returns all media records that match the parameters.
        /// </summary>
        public async Task<List<MediaRecord>> GetApplicationEntityMedia(
            string application,
            int applicationEntityId,
            bool includeDeleted = false)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.GetApplicationEntityMedia called with the parameters {ParameterFunction.FormatParameters(new string[] { application, includeDeleted.ToString(), applicationEntityId.ToString() })}.");

            List<MediaRecord> media = new List<MediaRecord>();
            int totalRecords = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "GetApplicationEntityMedia.sql"));
                sql += MediaConverter.GetSQLGetApplicationEntity(application);
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@entityId", SqlDbType.Int) { Value = applicationEntityId }
                };

                if (!includeDeleted)
                {
                    sql += "\nand Media.IsDeleted = 0";
                }

                (List<MediaRecord> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => new MediaRecord
                    {
                        Id = reader.GetInt32(0),
                        Type = new MediaTypeRecord
                        {
                            Entension = reader.GetString(1),
                            MimeType = reader.GetString(2)
                        },
                        Name = reader.GetString(3),
                        Size = reader.GetInt64(4),
                        Path = reader.GetString(5),
                        Domain = reader.GetString(6),
                        URL = $"{reader.GetString(6)}{reader.GetString(5)}/{reader.GetString(3)}{reader.GetString(1)}",
                        Application = reader.GetString(7),
                        DateUploaded = DateTime.SpecifyKind(
                            reader.GetDateTime(8),
                            DateTimeKind.Utc),
                        DateUpdated = DateTime.SpecifyKind(
                            reader.GetDateTime(9),
                            DateTimeKind.Utc),
                        IsDeleted = reader.GetBoolean(10),
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.GetApplicationEntityMedia.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                media = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.GetApplicationEntityMedia.";
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
                $"MediaService.GetApplicationEntityMedia returned {media.Count} records .");
            return media;
        }

        /// <summary>
        /// Returns the media record that matches the id.
        /// </summary>
        public async Task<MediaRecord> GetMediaId(int mediaId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.GetMediaId called with the parameter \"{mediaId}\".");

            MediaRecord media = new MediaRecord();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "GetMediaId.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@mediaId", SqlDbType.Int) { Value = mediaId }
                };

                (MediaRecord result, Exception ex) = await _Database.QuerySingle(
                    sql,
                    reader => new MediaRecord
                    {
                        Id = reader.GetInt32(0),
                        Type = new MediaTypeRecord
                        {
                            Entension = reader.GetString(1),
                            MimeType = reader.GetString(2)
                        },
                        Name = reader.GetString(3),
                        Size = reader.GetInt64(4),
                        Path = reader.GetString(5),
                        Domain = reader.GetString(6),
                        URL = $"{reader.GetString(6)}{reader.GetString(5)}/{reader.GetString(3)}{reader.GetString(1)}",
                        Application = reader.GetString(7),
                        DateUploaded = DateTime.SpecifyKind(
                            reader.GetDateTime(8),
                            DateTimeKind.Utc),
                        DateUpdated = DateTime.SpecifyKind(
                            reader.GetDateTime(9),
                            DateTimeKind.Utc),
                        IsDeleted = reader.GetBoolean(10),
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.GetMediaId.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                media = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.GetMediaId.";
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Warning,
                    message);
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Error,
                    ex.ToString(),
                    message);
            }

            if (media != null)
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "MediaService.GetMediaId returned 1 record");
            }

            else
            {
                _Logger.LogMessage(
                    StandardValues.LoggerValues.Debug,
                    "MediaService.GetMediaId returned 0 records");
            }

            return media;
        }

        /// <summary>
        /// Returns whether a media record already exists for the application with the given name.
        /// </summary>
        public async Task<bool> MediaExists(
            string application,
            string name)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaExists called with the parameters \"{application}\", \"{name}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "MediaExists.sql"));
                sql += "\nwhere [Application] = @application\nand Media.[Name] = @name";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = application },
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = name }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaExists.";
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
                string message = "An error occured when trying to run MediaService.MediaExists.";
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
                $"MediaService.MediaExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Returns whether a media record exists with the given id.
        /// </summary>
        public async Task<bool> MediaExists(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaExists called with the parameter \"{id}\".");

            bool exists = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "MediaExists.sql"));
                sql += "\nwhere MediaId = @mediaId";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@mediaId", SqlDbType.Int) { Value = id }
                };

                (List<int> results, Exception ex) = await _Database.Query(
                    sql,
                    reader => reader.GetInt32(0),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaExists.";
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
                string message = "An error occured when trying to run MediaService.MediaExists.";
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
                $"MediaService.MediaExists returned {exists}.");
            return exists;
        }

        /// <summary>
        /// Checks and creates the MediaType record.
        /// </summary>
        public async Task<bool> MediaTypeCreated(
            string extension,
            string mimeType)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaTypeCreated called with the parameters \"{extension}\", \"{mimeType}\".");

            bool created = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "CreateMediaType.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@extension", SqlDbType.VarChar) { Value = extension },
                    new SqlParameter("@mimeType", SqlDbType.VarChar) { Value = mimeType }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaTypeCreated.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);

                    created = false;
                }

                if (rowsAffected == 1)
                {
                    created = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.MediaTypeCreated.";
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
                $"MediaService.MediaTypeCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Creates the media.
        /// </summary>
        public async Task<(bool, int)> MediaCreated(MediaModel media)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaCreated called with the parameters {ParameterFunction.FormatParameters(media)}.");

            bool created = true;
            int mediaId = 0;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "CreateMedia.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = media.Name },
                    new SqlParameter("@size", SqlDbType.BigInt) { Value = media.Size },
                    new SqlParameter("@path", SqlDbType.VarChar) { Value = media.Path },
                    new SqlParameter("@extension", SqlDbType.VarChar) { Value = media.Entension },
                    new SqlParameter("@mimeType", SqlDbType.VarChar) { Value = media.MimeType },
                    new SqlParameter("@domain", SqlDbType.VarChar) { Value = media.Domain },
                    new SqlParameter("@application", SqlDbType.VarChar) { Value = media.Application },
                };

                (object result, Exception ex) = await _Database.ExecuteScalar(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaCreated.";
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
                    mediaId = int.Parse(result.ToString());
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.MediaCreated.";
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
                $"MediaService.MediaCreated returned {created}.");
            return (
                created,
                mediaId);
        }

        /// <summary>
        /// Creates the application entity link record.
        /// </summary>
        public async Task<bool> ApplicationEntityLinkCreated(
            string application,
            int entityId,
            int mediaId)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.ApplicationEntityLinkCreated called with the parameters \"{application}\", \"{entityId}\", \"{mediaId}\".");

            bool created = false;

            try
            {
                string sql = MediaConverter.GetSQLCreateApplicationEntityLink(application);
                sql += @"
values (
    @entityId,
    @mediaId
)";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@entityId", SqlDbType.Int) { Value = entityId },
                    new SqlParameter("@mediaId", SqlDbType.Int) { Value = mediaId }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.ApplicationEntityLinkCreated.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);

                    created = false;
                }

                if (rowsAffected == 1)
                {
                    created = true;
                }
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run MediaService.ApplicationEntityLinkCreated.";
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
                $"MediaService.ApplicationEntityLinkCreated returned {created}.");
            return created;
        }

        /// <summary>
        /// Updates the details of the given media.
        /// </summary>
        public async Task<bool> MediaUpdated(
            int id,
            MediaUpdateModel media)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaUpdated called with the parameters {ParameterFunction.FormatParameters(new string[] { id.ToString(), ParameterFunction.FormatParameters(media) })}.");

            bool updated = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "MediaUpdate.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = media.Name },
                    new SqlParameter("@size", SqlDbType.BigInt) { Value = media.Size },
                    new SqlParameter("@path", SqlDbType.VarChar) { Value = media.Path },
                    new SqlParameter("@mediaId", SqlDbType.Int) { Value = id }
                };

                sql = SQLFunction.CleanSQL(
                    media,
                    sql);
                parameters = SQLFunction.CleanParameterArray(
                    media,
                    parameters);

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaUpdated.";
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
                string message = "An error occured when trying to run MediaService.MediaUpdated.";
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
                $"MediaService.MediaUpdated returned {updated}.");
            return updated;
        }

        /// <summary>
        /// Sets the media to deleted.
        /// </summary>
        public async Task<bool> MediaDeleted(int id)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"MediaService.MediaDeleted called with the parameter \"{id}\".");

            bool deleted = false;

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Media",
                    "DeleteMedia.sql"));
                SqlParameter[] parameters =
                {
                    new SqlParameter("@mediaId", SqlDbType.Int) { Value = id }
                };

                (int rowsAffected, Exception ex) = await _Database.Execute(
                    sql,
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run MediaService.MediaDeleted.";
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
                string message = "An error occured when trying to run MediaService.MediaDeleted.";
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
                $"MediaService.MediaDeleted returned {deleted}.");
            return deleted;
        }
    }
}