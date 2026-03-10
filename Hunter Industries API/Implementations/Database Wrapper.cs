using HunterIndustriesAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HunterIndustriesAPI.Implementations
{
    /// <summary>
    /// </summary>
    public class DatabaseWrapper : IDatabase
    {
        private readonly IDatabaseOptions _Options;

        // Sets the class's global variables.
        public DatabaseWrapper(
            IDatabaseOptions _options)
        {
            _Options = _options;
        }

        /// <summary>
        /// Returns a list of the given model from the database.
        /// </summary>
        public async Task<(List<T>, Exception)> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
        {
            List<T> results = new List<T>();
            Exception exception = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_Options.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            while (await dataReader.ReadAsync())
                            {
                                results.Add(map(dataReader));
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                exception = ex;
            }

            return (results, exception);
        }

        /// <summary>
        /// Returns the given field from the database.
        /// </summary>
        public async Task<(T, Exception)> QuerySingle<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] parameters)
        {
            T result = default;
            Exception exception = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_Options.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                result = map(dataReader);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                exception = ex;
            }

            return (result, exception);
        }

        /// <summary>
        /// Returns the result of the execution for given query.
        /// </summary>
        public async Task<(int, Exception)> Execute(string sql, params SqlParameter[] parameters)
        {
            int result = -1;
            Exception exception = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_Options.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters);

                        result = await command.ExecuteNonQueryAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                exception = ex;
            }

            return (result, exception);
        }

        /// <summary>
        /// Returns the result of the execution for given query.
        /// </summary>
        public async Task<(object, Exception)> ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            object result = null;
            Exception exception = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_Options.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters);

                        result = await command.ExecuteScalarAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                exception = ex;
            }

            return (result, exception);
        }
    }
}