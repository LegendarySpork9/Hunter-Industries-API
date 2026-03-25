// Copyright © - Unpublished - Toby Hunter
namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class ConfigurationConverter
    {
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
        public static string GetSQLExits(string entity)
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
                case "machine": return "GetMachine.sql";
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
    }
}