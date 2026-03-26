// Copyright © - Unpublished - Toby Hunter
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
                case "application": return "GetApplication.sql";
                case "applicationSetting": return "GetApplicationSetting.sql";
                case "authorisation": return "GetAuthorisation.sql";
                case "component": return "GetComponent.sql";
                case "connection": return "GetConnection.sql";
                case "downtime": return "GetDowntime.sql";
                case "game": return "GetGame.sql";
                case "machine": return "GetMachine.sql";
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
                case "application": return "GetTotalApplication.sql";
                case "applicationSetting": return "GetTotalApplicationSetting.sql";
                case "authorisation": return "GetTotalAuthorisation.sql";
                case "component": return "GetTotalComponent.sql";
                case "connection": return "GetTotalConnection.sql";
                case "downtime": return "GetTotalDowntime.sql";
                case "game": return "GetTotalGame.sql";
                case "machine": return "GetTotalMachine.sql";
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
                case "application": return "ApplicationExists.sql";
                case "applicationSetting": return "ApplicationSettingExists.sql";
                case "authorisation": return "AuthorisationExists.sql";
                case "component": return "ComponentExists.sql";
                case "connection": return "ConnectionExists.sql";
                case "downtime": return "DowntimeExists.sql";
                case "game": return "GameExists.sql";
                case "machine": return "MachineExists.sql";
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
                case "application": return "CreateApplication.sql";
                case "applicationSetting": return "CreateApplicationSetting.sql";
                case "authorisation": return "CreateAuthorisation.sql";
                case "component": return "CreateComponent.sql";
                case "connection": return "CreateConnection.sql";
                case "downtime": return "CreateDowntime.sql";
                case "game": return "CreateGame.sql";
                case "machine": return "CreateMachine.sql";
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
                case "application": return "ApplicationUpdated.sql";
                case "applicationSetting": return "ApplicationSettingUpdated.sql";
                case "authorisation": return "AuthorisationUpdated.sql";
                case "component": return "ComponentUpdated.sql";
                case "connection": return "ConnectionUpdated.sql";
                case "downtime": return "DowntimeUpdated.sql";
                case "game": return "GameUpdated.sql";
                case "machine": return "MachineUpdated.sql";
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
    }
}