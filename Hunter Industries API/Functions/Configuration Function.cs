// Copyright © - Unpublished - Toby Hunter
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class ConfigurationFunction
    {
        /// <summary>
        /// Removes any null properties from the SQL parameter list.
        /// </summary>
        public static SqlParameter[] CleanParameterArray(object model,
            SqlParameter[] parameters)
        {
            List<SqlParameter> parameterList = parameters.ToList();
            PropertyInfo[] properties = GetProperties(model);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(model);

                if (value == null)
                {
                    string paramName = $"@{property.Name.ToLower()}";

                    if (parameterList.Exists(p => p.ParameterName.ToLower() == paramName))
                    {
                        parameterList.Remove(parameterList.First(p => p.ParameterName.ToLower() == paramName));
                    }
                }
            }

            return parameterList.ToArray();
        }

        /// <summary>
        /// Removes any null properties from the SQL.
        /// </summary>
        public static string CleanSQL(object model,
            string sql)
        {
            List<string> sqlLines = sql.Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.None).ToList();
            PropertyInfo[] properties = GetProperties(model);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(model);

                if (value == null)
                {
                    string paramName = $"@{property.Name.ToLower()}";

                    if (sqlLines.Exists(s => s.ToLower()
                        .Contains(paramName)))
                    {
                        sqlLines.Remove(sqlLines.First(s => s.ToLower()
                            .Contains(paramName)));
                    }
                }
            }

            string lastSet = sqlLines.LastOrDefault(s => s.Contains(','));

            if (lastSet != null)
            {
                int index = sqlLines.IndexOf(lastSet);
                sqlLines[index] = sqlLines[index].Replace(",", "");
            }

            string whereCondition = sqlLines.Find(s => s.Contains("where"));

            if (string.IsNullOrWhiteSpace(whereCondition))
            {
                string firstAnd = sqlLines.First(s => s.Contains("and"));
                int index = sqlLines.IndexOf(firstAnd);

                whereCondition = firstAnd.Replace("and", "where");

                sqlLines[index] = whereCondition;
            }

            return string.Join(Environment.NewLine, sqlLines);
        }

        /// <summary>
        /// Returns all the properties in the model.
        /// </summary>
        private static PropertyInfo[] GetProperties(object model) => model.GetType().GetProperties();
    }
}