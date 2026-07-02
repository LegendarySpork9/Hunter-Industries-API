// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Mappings.Portfolio;
using HunterIndustriesAPI.Objects.Portfolio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace HunterIndustriesAPI.Converters
{
    /// <summary>
    /// </summary>
    public static class PortfolioConverter
    {
        /// <summary>
        /// Returns the Getx sql file to load.
        /// </summary>
        public static string GetSQLGet(string linkedItem)
        {
            switch (linkedItem)
            {
                case "frameworks": return Path.Combine(
                    "Framework",
                    "GetFrameworks.sql");
                case "languages": return Path.Combine(
                    "Language",
                    "GetLanguages.sql");
                case "environments": return Path.Combine(
                    "Environment",
                    "GetEnvironments.sql");
                case "buildHistories": return Path.Combine(
                    "Build History",
                    "GetBuildHistories.sql");
                default: return "Unknown.sql";
            };
        }

        /// <summary>
        /// Returns the Createx sql file to load.
        /// </summary>
        public static string GetSQLCreate(string linkedItem)
        {
            switch (linkedItem)
            {
                case "frameworks": return Path.Combine(
                    "Framework",
                    "CreateFramework.sql");
                case "languages": return Path.Combine(
                    "Language",
                    "CreateLanguage.sql");
                case "environments": return Path.Combine(
                    "Environment",
                    "CreateEnvironment.sql");
                case "buildHistories": return Path.Combine(
                    "Build History",
                    "CreateBuildHistory.sql");
                case "type": return "CreateItemType.sql";
                case "llmCompany": return "CreateLLMCompany.sql";
                case "llmModel": return "CreateLLMModel.sql";
                default: return "Unknown.sql";
            };
        }

        /// <summary>
        /// Returns the CreateLinkx sql file to load.
        /// </summary>
        public static string GetSQLCreateLink(string linkedItem)
        {
            switch (linkedItem)
            {
                case "frameworks": return Path.Combine(
                    "Item",
                    "Links",
                    "CreateItemFramework.sql");
                case "languages": return Path.Combine(
                    "Item",
                    "Links",
                    "CreateItemLanguage.sql");
                case "environments": return Path.Combine(
                    "Item",
                    "Links",
                    "CreateItemEnvironment.sql");
                default: return "Unknown.sql";
            };
        }

        /// <summary>
        /// Returns the DeleteLinkx sql file to load.
        /// </summary>
        public static string GetSQLDeleteLink(string linkedItem)
        {
            switch (linkedItem)
            {
                case "frameworks": return Path.Combine(
                    "Item",
                    "Links",
                    "DeleteItemFramework.sql");
                case "languages": return Path.Combine(
                    "Item",
                    "Links",
                    "DeleteItemLanguage.sql");
                case "environments": return Path.Combine(
                    "Item",
                    "Links",
                    "DeleteItemEnvironment.sql");
                default: return "Unknown.sql";
            };
        }

        /// <summary>
        /// Returns the parameters for the Createx sql.
        /// </summary>
        public static SqlParameter[] GetParametersCreate(
            string linkedItem,
            int itemId,
            object record)
        {
            switch (linkedItem)
            {
                case "frameworks":
                    string framework = record as string;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = framework }
                    };
                case "languages":
                    string language = record as string;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = language }
                    };
                case "environments":
                    string environment = record as string;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = environment }
                    };
                case "buildHistories":
                    BuildHistoryRecord buildHistory = record as BuildHistoryRecord;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId },
                        new SqlParameter("@version", SqlDbType.VarChar) { Value = buildHistory.Version },
                        new SqlParameter("@releaseDate", SqlDbType.DateTime) { Value = buildHistory.ReleaseDate }
                    };
                case "type":
                    string type = record as string;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = type }
                    };
                case "llmCompany":
                    string llmCompany = record as string;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.VarChar) { Value = llmCompany }
                    };
                case "llmModel":
                    LLMRecord llm = record as LLMRecord;

                    return new SqlParameter[]
                    {
                        new SqlParameter("@company", SqlDbType.VarChar) { Value = llm.Company },
                        new SqlParameter("@model", SqlDbType.VarChar) { Value = llm.Model }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the parameters for the link sql.
        /// </summary>
        public static SqlParameter[] GetParametersCreateLink(
            string linkedItem,
            int itemId,
            string data)
        {
            switch (linkedItem)
            {
                case "frameworks":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId },
                        new SqlParameter("@framework", SqlDbType.VarChar) { Value = data }
                    };
                case "languages":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId },
                        new SqlParameter("@language", SqlDbType.VarChar) { Value = data }
                    };
                case "environments":
                    return new SqlParameter[]
                    {
                        new SqlParameter("@itemId", SqlDbType.Int) { Value = itemId },
                        new SqlParameter("@environment", SqlDbType.VarChar) { Value = data }
                    };
                default: return Array.Empty<SqlParameter>();
            }
        }

        /// <summary>
        /// Returns the data reader mappings for the given linked item.
        /// </summary>
        public static Func<IDataReader, object> GetDataReaderMappings(string linkedItem)
        {
            switch (linkedItem)
            {
                case "frameworks": return PortfolioDataReaderMapping.SingleLinkedItemMapper;
                case "languages": return PortfolioDataReaderMapping.SingleLinkedItemMapper;
                case "environments": return PortfolioDataReaderMapping.SingleLinkedItemMapper;
                case "buildHistories": return PortfolioDataReaderMapping.BuildHistoryMapper;
                default: return null;
            }
        }
    }
}