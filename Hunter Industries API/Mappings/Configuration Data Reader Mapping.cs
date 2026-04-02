// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Objects.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace HunterIndustriesAPI.Mappings
{
    /// <summary>
    /// </summary>
    public static class ConfigurationDataReaderMapping
    {
        /// <summary>
        /// The SQL row to model mappings for the application record.
        /// </summary>
        public static readonly Func<IDataReader, ApplicationRecord> ApplicationMapper = reader =>
        {
            ApplicationRecord application = new ApplicationRecord
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Authorisation = new AuthorisationRecord()
                {
                    Id = reader.GetInt32(2),
                    Phrase = reader.GetString(3),
                },
                Settings = new List<ApplicationSettingRecord>(),
                IsDeleted = reader.GetBoolean(9)
            };

            if (!reader.IsDBNull(4))
            {
                application.Settings.Add(new ApplicationSettingRecord()
                {
                    Id = reader.GetInt32(4),
                    Name = reader.GetString(5),
                    Type = reader.GetString(6),
                    Required = reader.GetBoolean(7),
                    IsDeleted = reader.GetBoolean(8)
                });
            }

            return application;
        };

        /// <summary>
        /// The SQL row to model mappings for the application setting record.
        /// </summary>
        public static readonly Func<IDataReader, ApplicationSettingRecord> ApplicationSettingMapper = reader =>
        {
            ApplicationSettingRecord applicationSetting = new ApplicationSettingRecord()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = reader.GetString(2),
                Required = reader.GetBoolean(3),
                IsDeleted = reader.GetBoolean(4)
            };

            return applicationSetting;
        };

        /// <summary>
        /// The SQL row to model mappings for the authorisation record.
        /// </summary>
        public static readonly Func<IDataReader, AuthorisationRecord> AuthorisationMapper = reader =>
        {
            AuthorisationRecord authorisation = new AuthorisationRecord()
            {
                Id = reader.GetInt32(0),
                Phrase = reader.GetString(1),
                IsDeleted = reader.GetBoolean(2)
            };

            return authorisation;
        };

        /// <summary>
        /// The SQL row to model mappings for the component record.
        /// </summary>
        public static readonly Func<IDataReader, ComponentRecord> ComponentMapper = reader =>
        {
            ComponentRecord component = new ComponentRecord()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                IsDeleted = reader.GetBoolean(2)
            };

            return component;
        };

        /// <summary>
        /// The SQL row to model mappings for the connection record.
        /// </summary>
        public static readonly Func<IDataReader, ConfigurationConnectionRecord> ConnectionMapper = reader =>
        {
            ConfigurationConnectionRecord connection = new ConfigurationConnectionRecord()
            {
                Id = reader.GetInt32(0),
                IPAddress = reader.GetString(1),
                Port = reader.GetInt32(2),
                IsDeleted = reader.GetBoolean(3)
            };

            return connection;
        };

        /// <summary>
        /// The SQL row to model mappings for the downtime record.
        /// </summary>
        public static readonly Func<IDataReader, ConfigurationDowntimeRecord> DowntimeMapper = reader =>
        {
            ConfigurationDowntimeRecord downtime = new ConfigurationDowntimeRecord()
            {
                Id = reader.GetInt32(0),
                Time = reader.GetString(1),
                Duration = reader.GetInt32(2),
                IsDeleted = reader.GetBoolean(3)
            };

            return downtime;
        };

        /// <summary>
        /// The SQL row to model mappings for the game record.
        /// </summary>
        public static readonly Func<IDataReader, GameRecord> GameMapper = reader =>
        {
            GameRecord game = new GameRecord()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Version = reader.GetString(2),
                IsDeleted = reader.GetBoolean(3)
            };

            return game;
        };

        /// <summary>
        /// The SQL row to model mappings for the machine record.
        /// </summary>
        public static readonly Func<IDataReader, MachineRecord> MachineMapper = reader =>
        {
            MachineRecord machine = new MachineRecord()
            {
                Id = reader.GetInt32(0),
                HostName = reader.GetString(1),
                IsDeleted = reader.GetBoolean(2)
            };

            return machine;
        };
    }
}