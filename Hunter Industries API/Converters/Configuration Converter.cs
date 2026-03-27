// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class ConfigurationConverter
    {
        #region SQL

        /// <summary>
        /// Returns the Getx sql file to load.
        /// </summary>
        public static string GetSQLGet(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\GetApplication.sql";
                case "applicationSetting": return @"Application Setting\GetApplicationSetting.sql";
                case "authorisation": return @"Authorisation\GetAuthorisation.sql";
                case "component": return @"Component\GetComponent.sql";
                case "connection": return @"Connection\GetConnection.sql";
                case "downtime": return @"Downtime\GetDowntime.sql";
                case "game": return @"Game\GetGame.sql";
                case "machine": return @"Machine\GetMachine.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Getx pagination sql.
        /// </summary>
        public static string GetSQLGetPagination(string entity)
        {
            switch (entity)
            {
                case "application": return @"
order by ApplicationId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "authorisation": return @"
order by PhraseId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "component": return @"
order by ComponentId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "connection": return @"
order by ConnectionId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "downtime": return @"
order by DowntimeId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "game": return @"
order by GameId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                case "machine": return @"
order by MachineId asc
offset (@PageSize * (@PageNumber - 1)) rows
fetch next @PageSize rows only";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the GetTotalx sql file to load.
        /// </summary>
        public static string GetSQLGetTotal(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\GetTotalApplication.sql";
                case "applicationSetting": return @"Application Setting\GetTotalApplicationSetting.sql";
                case "authorisation": return @"Authorisation\GetTotalAuthorisation.sql";
                case "component": return @"Component\GetTotalComponent.sql";
                case "connection": return @"Connection\GetTotalConnection.sql";
                case "downtime": return @"Downtime\GetTotalDowntime.sql";
                case "game": return @"Game\GetTotalGame.sql";
                case "machine": return @"Machine\GetTotalMachine.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the xExists sql file to load.
        /// </summary>
        public static string GetSQLExists(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\ApplicationExists.sql";
                case "applicationSetting": return @"Application Setting\ApplicationSettingExists.sql";
                case "authorisation": return @"Authorisation\AuthorisationExists.sql";
                case "component": return @"Component\ComponentExists.sql";
                case "connection": return @"Connection\ConnectionExists.sql";
                case "downtime": return @"Downtime\DowntimeExists.sql";
                case "game": return @"Game\GameExists.sql";
                case "machine": return @"Machine\MachineExists.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the xExists sql filter.
        /// </summary>
        public static string GetSQLFilter(string entity)
        {
            switch (entity)
            {
                case "application": return @"
where [Name] = @Name";
                case "applicationSetting": return @"
where ApplicationId = @ApplicationId
and [Name] = @Name";
                case "authorisation": return @"
where Phrase = @Phrase";
                case "component": return @"
where [Name] = @Name";
                case "connection": return @"
where IPAddress = @IPAddress
and [Port] = @Port";
                case "downtime": return @"
where Time = @Time";
                case "game": return @"
where [Name] = @Name
and [Version] = @Version";
                case "machine": return @"
where HostName = @HostName";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the xExists sql filter.
        /// </summary>
        public static string GetSQLFilterId(string entity)
        {
            switch (entity)
            {
                case "application": return @"
where ApplicationId = @ApplicationId";
                case "applicationSetting": return @"
where ApplicationSettingId = @ApplicationSettingId";
                case "authorisation": return @"
where PhraseId = @PhraseId";
                case "component": return @"
where ComponentId = @ComponentId";
                case "connection": return @"
where ConnectionId = @ConnectionId";
                case "downtime": return @"
where DowntimeId = @DowntimeId";
                case "game": return @"
where GameId = @GameId";
                case "machine": return @"
where MachineId = @MachineId";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Createx sql file to load.
        /// </summary>
        public static string GetSQLCreate(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\CreateApplication.sql";
                case "applicationSetting": return @"Application Setting\CreateApplicationSetting.sql";
                case "authorisation": return @"Authorisation\CreateAuthorisation.sql";
                case "component": return @"Component\CreateComponent.sql";
                case "connection": return @"Connection\CreateConnection.sql";
                case "downtime": return @"Downtime\CreateDowntime.sql";
                case "game": return @"Game\CreateGame.sql";
                case "machine": return @"Machine\CreateMachine.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the xUpdated sql file to load.
        /// </summary>
        public static string GetSQLUpdated(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\ApplicationUpdated.sql";
                case "applicationSetting": return @"Application Setting\ApplicationSettingUpdated.sql";
                case "authorisation": return @"Authorisation\AuthorisationUpdated.sql";
                case "component": return @"Component\ComponentUpdated.sql";
                case "connection": return @"Connection\ConnectionUpdated.sql";
                case "downtime": return @"Downtime\DowntimeUpdated.sql";
                case "game": return @"Game\GameUpdated.sql";
                case "machine": return @"Machine\MachineUpdated.sql";
                default: return "Unknown.sql";
            }
        }

        /// <summary>
        /// Returns the Deletex sql file to load.
        /// </summary>
        public static string GetSQLDelete(string entity)
        {
            switch (entity)
            {
                case "application": return @"Application\DeleteApplication.sql";
                case "applicationSetting": return @"Application Setting\DeleteApplicationSetting.sql";
                case "authorisation": return @"Authorisation\DeleteAuthorisation.sql";
                case "component": return @"Component\DeleteComponent.sql";
                case "connection": return @"Connection\DeleteConnection.sql";
                case "downtime": return @"Downtime\DeleteDowntime.sql";
                case "game": return @"Game\DeleteGame.sql";
                case "machine": return @"Machine\DeleteMachine.sql";
                default: return "Unknown.sql";
            }
        }

        #endregion

        #region SQLParameters

        /// <summary>
        /// Returns the parameters for the paginated Getx sql.
        /// </summary>
        public static SqlParameter[] GetParametersGet(string entity, int pageSize, int pageNumber)
        {
            switch (entity)
            {
                case "application":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the Getx sql.
        /// </summary>
        public static SqlParameter[] GetParametersGetSingle(string entity, int entityId)
        {
            switch (entity)
            {
                case "application":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = entityId }
                    };
                case "applicationSetting":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = entityId }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PhraseId", SqlDbType.Int) { Value = entityId }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ComponentId", SqlDbType.Int) { Value = entityId }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ConnectionId", SqlDbType.Int) { Value = entityId }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@DowntimeId", SqlDbType.Int) { Value = entityId }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@GameId", SqlDbType.Int) { Value = entityId }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@MachineId", SqlDbType.Int) { Value = entityId }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the xExists sql.
        /// </summary>
        public static SqlParameter[] GetParameterExists(string entity, object record, int? parentEntityId = null)
        {
            switch (entity)
            {
                case "application":
                    ApplicationModel application = record as ApplicationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = application.Name }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = parentEntityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = applicationSetting.Name }
                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@Port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Time", SqlDbType.VarChar) { Value = downtime.Time }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@Version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@HostName", SqlDbType.VarChar) { Value = machine.HostName }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the xExists sql.
        /// </summary>
        public static SqlParameter[] GetParameterExists(string entity, int entityId)
        {
            switch (entity)
            {
                case "application":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = entityId }
                    };
                case "applicationSetting":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationSettingId", SqlDbType.Int) { Value = entityId }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@PhraseId", SqlDbType.Int) { Value = entityId }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ComponentId", SqlDbType.Int) { Value = entityId }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@ConnectionId", SqlDbType.Int) { Value = entityId }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@DowntimeId", SqlDbType.Int) { Value = entityId }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@GameId", SqlDbType.Int) { Value = entityId }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@MachineId", SqlDbType.Int) { Value = entityId }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the Createx sql.
        /// </summary>
        public static SqlParameter[] GetParametersCreate(string entity, object record, int? parentEntityId = null)
        {
            switch (entity)
            {
                case "application":
                    ApplicationModel application = record as ApplicationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = application.Name },
                        new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = application.Phrase }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = parentEntityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = applicationSetting.Name },
                        new SqlParameter("@Required", SqlDbType.Bit) { Value = applicationSetting.Required }
                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@Port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Time", SqlDbType.VarChar) { Value = downtime.Time }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@Version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@HostName", SqlDbType.VarChar) { Value = machine.HostName }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the xUpdated sql.
        /// </summary>
        public static SqlParameter[] GetParametersUpdated(string entity, object record, int entityId)
        {
            switch (entity)
            {
                case "application":
                    ApplicationModel application = record as ApplicationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = application.Name }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationSettingId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = applicationSetting.Name },
                        new SqlParameter("@Required", SqlDbType.Bit) { Value = applicationSetting.Required }

                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@PhraseId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ComponentId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ConnectionId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@IPAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@Port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@DowntimeId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Time", SqlDbType.VarChar) { Value = downtime.Time }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@GameId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@Name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@Version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@MachineId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@HostName", SqlDbType.VarChar) { Value = machine.HostName }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        #endregion

        /// <summary>
        /// Returns the data reader mappings for the given entity.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappings(string entity)
        {
            switch (entity)
            {
                case "application": return ConfigurationDataReaderMapping.ApplicationMapper;
                case "applicationSetting": return ConfigurationDataReaderMapping.ApplicationSettingMapper;
                case "authorisation": return ConfigurationDataReaderMapping.AuthorisationMapper;
                case "component": return ConfigurationDataReaderMapping.ComponentMapper;
                case "connection": return ConfigurationDataReaderMapping.ConnectionMapper;
                case "downtime": return ConfigurationDataReaderMapping.DowntimeMapper;
                case "game": return ConfigurationDataReaderMapping.GameMapper;
                case "machine": return ConfigurationDataReaderMapping.MachineMapper;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the request model in the correct format.
        /// </summary>
        public static object GetRequestObject(string entity, object request)
        {
            switch (entity)
            {
                case "application": return request as ApplicationModel;
                case "applicationSetting": return request as ApplicationSettingModel;
                case "authorisation": return request as AuthorisationModel;
                case "component": return request as ComponentModel;
                case "connection": return request as ConnectionModel;
                case "downtime": return request as DowntimeModel;
                case "game": return request as GameModel;
                case "machine": return request as MachineModel;
                default: return null;
            }
        }
    }
}