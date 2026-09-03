// Copyright © - Unpublished - Toby Hunter
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HunterIndustriesAPI.IntegrationTests.API.Helpers
{
    public class HttpContextSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue =
            new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

        /// <summary>
        /// Posts a callback to the synchronization context queue.
        /// </summary>
        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        /// <summary>
        /// Sends a callback to the synchronization context synchronously.
        /// </summary>
        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }

        /// <summary>
        /// Creates a copy of this synchronization context.
        /// </summary>
        public override SynchronizationContext CreateCopy() => this;

        /// <summary>
        /// Runs an async action within the given HTTP context.
        /// </summary>
        public static void Run(
            HttpContext context,
            Func<Task> asyncAction)
        {
            HttpContextSynchronizationContext syncContext = new HttpContextSynchronizationContext();
            SynchronizationContext previousContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(syncContext);
            HttpContext.Current = context;

            Task task = asyncAction();
            task.ContinueWith(_ => syncContext._queue.CompleteAdding(), TaskScheduler.Default);

            while (syncContext._queue.TryTake(
                out KeyValuePair<SendOrPostCallback, object> workItem,
                Timeout.Infinite))
            {
                HttpContext.Current = context;
                workItem.Key(workItem.Value);
            }

            SynchronizationContext.SetSynchronizationContext(previousContext);
            task.GetAwaiter().GetResult();
        }
    }

    public static class LocalDbTestHelper
    {
        private const string MasterConnection = @"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=true;";

        /// <summary>
        /// Creates a new LocalDB test database with a unique name.
        /// </summary>
        public static (string connectionString, string databaseName) CreateDatabase()
        {
            string databaseName = $"HunterAPI_Test_{Guid.NewGuid():N}";
            string connectionString = $@"Server=(localdb)\MSSQLLocalDB;Database={databaseName};Integrated Security=true;";

            using (SqlConnection conn = new(MasterConnection))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    $"CREATE DATABASE [{databaseName}]",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return (
                connectionString,
                databaseName);
        }

        /// <summary>
        /// Creates the full database schema from the prepared SQL file.
        /// </summary>
        public static void CreateSchema(string connectionString)
        {
            string schemaFile = Path.Combine(
                GetProjectRoot(),
                "Prepared SQL",
                "Generate and Populate API Tables.sql");
            string schemaSql = File.ReadAllText(schemaFile);

            string[] batches = Regex.Split(
                schemaSql,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                foreach (string batch in batches)
                {
                    string trimmed = batch.Trim();

                    if (string.IsNullOrEmpty(trimmed))
                    {
                        continue;
                    } 

                    if (trimmed.StartsWith(
                        "USE ",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    using (SqlCommand cmd = new(
                        trimmed,
                        conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Drops the test database.
        /// </summary>
        public static void DropDatabase(string databaseName)
        {
            using (SqlConnection conn = new(MasterConnection))
            {
                conn.Open();

                using (SqlCommand cmd = new(
                    $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Clears all data from the database and reseeds lookup tables.
        /// </summary>
        public static void ClearData(string connectionString)
        {
            string[] tablesToClear =
            [
                "PortfolioItemMetric",
                "PortfolioItemEnvironment",
                "PortfolioItemLanguage",
                "PortfolioItemFramework",
                "PortfolioItemImage",
                "PortfolioItemBuildHistory",
                "ComponentInformation",
                "ServerAlert",
                "LoginAttempt",
                "Change",
                "PortfolioItem",
                "Media",
                "ServerInformation",
                "AssistantInformation",
                "AuditHistory",
                "UserSetting",
                "UserScope",
                "ApplicationSetting",
                "Application",
                "LLMModel",
                "APIUser",
                "Authorisation",
                "ErrorLog",
                "VersionHistory",
                "Domain",
                "PortfolioFilter",
                "Location",
                "[User]",
                "Connection",
                "Downtime",
                "Game",
                "Machine",
                "Component",
                "ComponentStatus",
                "Deletion",
                "Endpoint",
                "EndpointVersion",
                "Method",
                "Scope",
                "ServerAlertStatus",
                "StatusCode",
                "[Version]",
                "LLMCompany",
                "MediaType",
                "PortfolioItemType",
                "Framework",
                "Language",
                "Environment"
            ];

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                foreach (string table in tablesToClear)
                {
                    string tableName = table.StartsWith("[") ? table : $"[{table}]";

                    using (SqlCommand cmd = new(
                        $"DELETE FROM {tableName}",
                        conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                ReseedLookupTables(conn);
            }
        }

        /// <summary>
        /// Reseeds the lookup tables with their default values.
        /// </summary>
        private static void ReseedLookupTables(SqlConnection conn)
        {
            string[] seedStatements =
            [
                "SET IDENTITY_INSERT [Component] ON; INSERT INTO [Component] (ComponentId, [Name], IsDeleted) VALUES (1,'PC',0),(2,'Server',0),(3,'Connection',0); SET IDENTITY_INSERT [Component] OFF;",
                "SET IDENTITY_INSERT [ComponentStatus] ON; INSERT INTO [ComponentStatus] (ComponentStatusId, [Value]) VALUES (1,'Online'),(2,'Offline'),(3,'Unknown'); SET IDENTITY_INSERT [ComponentStatus] OFF;",
                "SET IDENTITY_INSERT [Deletion] ON; INSERT INTO [Deletion] (StatusId, [Value]) VALUES (1,'True'),(2,'False'); SET IDENTITY_INSERT [Deletion] OFF;",
                "SET IDENTITY_INSERT [Endpoint] ON; INSERT INTO [Endpoint] (EndpointId, [Value]) VALUES (1,'/auth/token'),(2,'/audithistory'),(3,'/assistant/config'),(4,'/assistant/version'),(5,'/assistant/deletion'),(6,'/assistant/location'),(7,'/user'),(8,'/UserSetting'),(9,'/serverstatus/serverinformation'),(10,'/serverstatus/serverevent'),(11,'/serverstatus/serveralert'),(12,'/errorlog'),(13,'/configuration'),(14,'/statistic'),(15,'/media'),(16,'/portfolio'),(17,'/portfolio/filter'); SET IDENTITY_INSERT [Endpoint] OFF;",
                "SET IDENTITY_INSERT [EndpointVersion] ON; INSERT INTO [EndpointVersion] (EndpointVersionId, [Value]) VALUES (1,'v1.0'),(2,'v1.1'),(3,'v2.0'),(4,'v2.1'),(5,'v2.2'); SET IDENTITY_INSERT [EndpointVersion] OFF;",
                "SET IDENTITY_INSERT [Method] ON; INSERT INTO [Method] (MethodId, [Value]) VALUES (1,'GET'),(2,'POST'),(3,'PATCH'),(4,'DELETE'); SET IDENTITY_INSERT [Method] OFF;",
                "SET IDENTITY_INSERT [Scope] ON; INSERT INTO [Scope] (ScopeId, [Value]) VALUES (1,'User'),(2,'Assistant API'),(3,'Book Reader API'),(4,'Control Panel API'),(5,'Server Status API'),(6,'Media API'),(7,'Portfolio API'); SET IDENTITY_INSERT [Scope] OFF;",
                "SET IDENTITY_INSERT [ServerAlertStatus] ON; INSERT INTO [ServerAlertStatus] (AlertStatusId, [Value]) VALUES (1,'Reported'),(2,'Investigating'),(3,'Resolved'); SET IDENTITY_INSERT [ServerAlertStatus] OFF;",
                "SET IDENTITY_INSERT [StatusCode] ON; INSERT INTO [StatusCode] (StatusId, [Value]) VALUES (1,'200 OK'),(2,'201 Created'),(3,'204 No Content'),(4,'400 Bad Request'),(5,'401 Unauthorized'),(6,'403 Forbidden'),(7,'404 Not Found'),(8,'500 Internal Server Error'); SET IDENTITY_INSERT [StatusCode] OFF;",
                "SET IDENTITY_INSERT [Version] ON; INSERT INTO [Version] (VersionId, [Value]) VALUES (1,'0.0.0'); SET IDENTITY_INSERT [Version] OFF;"
            ];

            foreach (string sql in seedStatements)
            {
                using (SqlCommand cmd = new(
                    sql,
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Returns the path to the API SQL files directory.
        /// </summary>
        public static string GetSqlFilesPath()
        {
            return Path.Combine(
                GetProjectRoot(),
                "Hunter Industries API", "SQL");
        }

        /// <summary>
        /// Returns the root directory of the project.
        /// </summary>
        private static string GetProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                ".."));
        }
    }
}
