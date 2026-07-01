// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Converters.Portfolio;
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
    public class GitHubService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IDatabaseOptions _Options;
        private readonly IDatabase _Database;

        /// <summary>
        /// </summary>
        // Sets the class's global variables.
        public GitHubService(
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
        /// Returns all CI statuses for the given repository.
        /// </summary>
        public async Task<Dictionary<string, string>> GetCIStatus(string repository)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"GitHubService.GetCIStatus called with the parameter \"{repository}\".");

            Dictionary<string, string> ciStatus = new Dictionary<string, string>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "GitHub",
                    "GetCIStatuses.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@repository", SqlDbType.VarChar) { Value = repository }
                };

                (List<KeyValuePair<string, string>> results, Exception ex) = await _Database.QueryGitHub(
                    sql,
                    reader => new KeyValuePair<string, string>(
                        reader.GetString(0),
                        reader.GetString(1)),
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run GitHubService.GetCIStatus.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                ciStatus = results.ToDictionary(r => r.Key, r => r.Value);
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run GitHubService.GetCIStatus.";
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
                $"GitHubService.GetCIStatus returned {ciStatus.Count} records.");
            return ciStatus;
        }

        /// <summary>
        /// Returns the issue breakdown record for the given repository.
        /// </summary>
        public async Task<GitHubIssueBreakdownRecord> GetIssueBreakdown(string repository)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"GitHubService.GetIssueBreakdown called with the parameter \"{repository}\".");

            GitHubIssueBreakdownRecord issueBreakdown = new GitHubIssueBreakdownRecord();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "GitHub",
                    "GetIssueBreakdown.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@repository", SqlDbType.VarChar) { Value = repository }
                };

                (GitHubIssueBreakdownRecord result, Exception ex) = await _Database.QuerySingleGitHub(
                    sql,
                    reader => new GitHubIssueBreakdownRecord
                    {
                        TotalIssues = reader.GetInt32(0),
                        Bugs = reader.GetInt32(1),
                        NewFeatures = reader.GetInt32(2)
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run GitHubService.GetIssueBreakdown.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                issueBreakdown = result;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run GitHubService.GetIssueBreakdown.";
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
                $"GitHubService.GetIssueBreakdown returned 1 record.");
            return issueBreakdown;
        }

        /// <summary>
        /// Returns all issue assignee breakdown records for the given repository.
        /// </summary>
        public async Task<List<GitHubIssueAssigneeBreakdownRecord>> GetIssueAssigneeBreakdown(string repository)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"GitHubService.GetIssueAssigneeBreakdown called with the parameter \"{repository}\".");

            List<GitHubIssueAssigneeBreakdownRecord> issueAssigneeBreakdown = new List<GitHubIssueAssigneeBreakdownRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "GitHub",
                    "GetIssueAssigneeBreakdown.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@repository", SqlDbType.VarChar) { Value = repository }
                };

                (List<GitHubIssueAssigneeBreakdownRecord> results, Exception ex) = await _Database.QueryGitHub(
                    sql,
                    reader => new GitHubIssueAssigneeBreakdownRecord
                    {
                        Name = reader.GetString(0),
                        Issues = reader.GetInt32(1)
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run GitHubService.GetIssueAssigneeBreakdown.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                issueAssigneeBreakdown = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run GitHubService.GetIssueAssigneeBreakdown.";
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
                $"GitHubService.GetIssueAssigneeBreakdown returned {issueAssigneeBreakdown.Count} records.");
            return issueAssigneeBreakdown;
        }

        /// <summary>
        /// Returns all issue in progress breakdown records for the given repository.
        /// </summary>
        public async Task<List<GitHubIssueInProgressBreakdownRecord>> GetIssueInProgressBreakdown(string repository)
        {
            _Logger.LogMessage(
                StandardValues.LoggerValues.Debug,
                $"GitHubService.GetIssueInProgressBreakdown called with the parameter \"{repository}\".");

            List<GitHubIssueInProgressBreakdownRecord> issueInProgressBreakdown = new List<GitHubIssueInProgressBreakdownRecord>();

            try
            {
                string sql = _FileSystem.ReadAllText(Path.Combine(
                    _Options.SQLFiles,
                    "Portfolio",
                    "GitHub",
                    "GetIssueInProgressBreakdown.sql"));
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@repository", SqlDbType.VarChar) { Value = repository }
                };

                (List<GitHubIssueInProgressBreakdownRecord> results, Exception ex) = await _Database.QueryGitHub(
                    sql,
                    reader => new GitHubIssueInProgressBreakdownRecord
                    {
                        Id = reader.GetInt32(0),
                        Assignee = reader.GetString(1),
                        Title = reader.GetString(2),
                        Type = GitHubIssueConverter.GetIssueType(reader.GetString(3))
                    },
                    parameters);

                if (ex != null)
                {
                    string message = "An error occured when trying to run GitHubService.GetIssueInProgressBreakdown.";
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Warning,
                        message);
                    _Logger.LogMessage(
                        StandardValues.LoggerValues.Error,
                        ex.ToString(),
                        message);
                }

                issueInProgressBreakdown = results;
            }

            catch (Exception ex)
            {
                string message = "An error occured when trying to run GitHubService.GetIssueInProgressBreakdown.";
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
                $"GitHubService.GetIssueInProgressBreakdown returned {issueInProgressBreakdown.Count} records.");
            return issueInProgressBreakdown;
        }
    }
}