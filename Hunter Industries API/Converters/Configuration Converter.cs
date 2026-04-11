// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings;
using HunterIndustriesAPI.Models.Requests.Bodies.Configuration;
using Newtonsoft.Json;
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
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "authorisation": return @"
order by PhraseId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "component": return @"
order by ComponentId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "connection": return @"
order by ConnectionId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "downtime": return @"
order by DowntimeId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "game": return @"
order by GameId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
                case "machine": return @"
order by MachineId asc
offset (@pageSize * (@pageNumber - 1)) rows
fetch next @pageSize rows only";
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
where [Name] = @name";
                case "applicationSetting": return @"
where ApplicationId = @applicationId
and [Name] = @name";
                case "authorisation": return @"
where Phrase = @phrase";
                case "component": return @"
where [Name] = @name";
                case "connection": return @"
where IPAddress = @ipAddress
and [Port] = @port";
                case "downtime": return @"
where Time = @time
and Duration = @duration";
                case "game": return @"
where [Name] = @name
and [Version] = @version";
                case "machine": return @"
where HostName = @hostName";
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
where [Application].ApplicationId = @applicationId";
                case "applicationSetting": return @"
where ApplicationSettingId = @applicationSettingId";
                case "authorisation": return @"
where PhraseId = @phraseId";
                case "component": return @"
where ComponentId = @componentId";
                case "connection": return @"
where ConnectionId = @connectionId";
                case "downtime": return @"
where DowntimeId = @downtimeId";
                case "game": return @"
where GameId = @gameId";
                case "machine": return @"
where MachineId = @machineId";
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
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize },
                        new SqlParameter("@pageNumber", SqlDbType.Int) { Value = pageNumber }
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
                        new SqlParameter("@applicationId", SqlDbType.Int) { Value = entityId }
                    };
                case "applicationSetting":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@applicationSettingId", SqlDbType.Int) { Value = entityId }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@phraseId", SqlDbType.Int) { Value = entityId }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@componentId", SqlDbType.Int) { Value = entityId }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@connectionId", SqlDbType.Int) { Value = entityId }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@downtimeId", SqlDbType.Int) { Value = entityId }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@gameId", SqlDbType.Int) { Value = entityId }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@machineId", SqlDbType.Int) { Value = entityId }
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
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = application.Name }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@applicationId", SqlDbType.Int) { Value = parentEntityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = applicationSetting.Name }
                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@time", SqlDbType.VarChar) { Value = downtime.Time },
                        new SqlParameter("@duration", SqlDbType.Int) { Value= downtime.Duration }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@hostName", SqlDbType.VarChar) { Value = machine.HostName }
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
                        new SqlParameter("@applicationId", SqlDbType.Int) { Value = entityId }
                    };
                case "applicationSetting":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@applicationSettingId", SqlDbType.Int) { Value = entityId }
                    };
                case "authorisation":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@phraseId", SqlDbType.Int) { Value = entityId }
                    };
                case "component":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@componentId", SqlDbType.Int) { Value = entityId }
                    };
                case "connection":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@connectionId", SqlDbType.Int) { Value = entityId }
                    };
                case "downtime":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@downtimeId", SqlDbType.Int) { Value = entityId }
                    };
                case "game":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@gameId", SqlDbType.Int) { Value = entityId }
                    };
                case "machine":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@machineId", SqlDbType.Int) { Value = entityId }
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
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = application.Name },
                        new SqlParameter("@phrase", SqlDbType.VarChar) { Value = application.Phrase }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@applicationId", SqlDbType.Int) { Value = parentEntityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = applicationSetting.Name },
                        new SqlParameter("@type", SqlDbType.VarChar) { Value = applicationSetting.Type },
                        new SqlParameter("@required", SqlDbType.Bit) { Value = applicationSetting.Required }
                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@time", SqlDbType.VarChar) { Value = downtime.Time },
                        new SqlParameter("@duration", SqlDbType.Int) { Value= downtime.Duration }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@hostName", SqlDbType.VarChar) { Value = machine.HostName }
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
                        new SqlParameter("@applicationId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = application.Name }
                    };
                case "applicationSetting":
                    ApplicationSettingModel applicationSetting = record as ApplicationSettingModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@applicationSettingId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = applicationSetting.Name },
                        new SqlParameter("@type", SqlDbType.VarChar) { Value = applicationSetting.Type },
                        new SqlParameter("@required", SqlDbType.Bit) { Value = applicationSetting.Required }

                    };
                case "authorisation":
                    AuthorisationModel authorisation = record as AuthorisationModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@phraseId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@phrase", SqlDbType.VarChar) { Value = authorisation.Phrase }
                    };
                case "component":
                    ComponentModel component = record as ComponentModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@componentId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = component.Name }
                    };
                case "connection":
                    ConnectionModel connection = record as ConnectionModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@connectionId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@ipAddress", SqlDbType.VarChar) { Value = connection.IPAddress },
                        new SqlParameter("@port", SqlDbType.Int) { Value = connection.Port }
                    };
                case "downtime":
                    DowntimeModel downtime = record as DowntimeModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@downtimeId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@time", SqlDbType.VarChar) { Value = downtime.Time },
                        new SqlParameter("@duration", SqlDbType.Int) { Value= downtime.Duration }
                    };
                case "game":
                    GameModel game = record as GameModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@gameId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = game.Name },
                        new SqlParameter("@version", SqlDbType.VarChar) { Value = game.Version }
                    };
                case "machine":
                    MachineModel machine = record as MachineModel;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@machineId", SqlDbType.Int) { Value = entityId },
                        new SqlParameter("@hostName", SqlDbType.VarChar) { Value = machine.HostName }
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
                case "application": return JsonConvert.DeserializeObject<ApplicationModel>(request.ToString());
                case "applicationSetting": return JsonConvert.DeserializeObject<ApplicationSettingModel>(request.ToString());
                case "authorisation": return JsonConvert.DeserializeObject<AuthorisationModel>(request.ToString());
                case "component": return JsonConvert.DeserializeObject<ComponentModel>(request.ToString());
                case "connection": return JsonConvert.DeserializeObject<ConnectionModel>(request.ToString());
                case "downtime": return JsonConvert.DeserializeObject<DowntimeModel>(request.ToString());
                case "game": return JsonConvert.DeserializeObject<GameModel>(request.ToString());
                case "machine": return JsonConvert.DeserializeObject<MachineModel>(request.ToString());
                default: return null;
            }
        }
    }
}