// Copyright © - Unpublished - Toby Hunter
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
        public static SqlParameter[] CleanParameterArray(object model, List<SqlParameter> parameters)
        {
            PropertyInfo[] properties = GetProperties(model);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(model);

                if (value == null)
                {
                    parameters.Remove(parameters.First(p => p.ParameterName == $"@{property.Name}"));
                }
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Returns all the properties in the model.
        /// </summary>
        private static PropertyInfo[] GetProperties(object model) => model.GetType().GetProperties();
    }
}