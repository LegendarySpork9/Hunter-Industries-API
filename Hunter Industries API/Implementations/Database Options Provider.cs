// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Abstractions;
using HunterIndustriesAPI.Models;

namespace HunterIndustriesAPI.Implementations
{
    /// <summary>
    /// </summary>
    public class DatabaseOptionsProvider :IDatabaseOptions
    {
        /// <summary>
        /// Returns the ConnectionString from the DatabaseModel.
        /// </summary>
        public string ConnectionString => DatabaseModel.ConnectionString;

        /// <summary>
        /// Returns the SQLFiles from the DatabaseModel.
        /// </summary>
        public string SQLFiles => DatabaseModel.SQLFiles;
    }
}