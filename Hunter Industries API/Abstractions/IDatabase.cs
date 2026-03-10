using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Abstractions
{
    /// <summary>
    /// Interface for the database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// </summary>
        Task<(List<T>, Exception)> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters);
        /// <summary>
        /// </summary>
        Task<(T, Exception)> QuerySingle<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters);
        /// <summary>
        /// </summary>
        Task<(int, Exception)> Execute(string sql, params SqlParameter[] parameters);
        /// <summary>
        /// </summary>
        Task<(object, Exception)> ExecuteScalar(string sql, params SqlParameter[] parameters);
    }
}
