// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Tests.Functions
{
    [TestClass]
    public class ConfigurationFunctionTest
    {
        #region CleanParameterArray Tests

        /// <summary>
        /// Tests whether the CleanParameterArray method removes parameters for null properties.
        /// </summary>
        [TestMethod]
        public void TestCleanParameterArrayRemovesNullProperties()
        {
            object model = new
            {
                Name = "Test",
                Value = (string)null
            };
            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", "Test"),
                new SqlParameter("@Value", "SomeValue")
            };

            SqlParameter[] actual = ConfigurationFunction.CleanParameterArray(model, parameters);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("@Name", actual[0].ParameterName);
        }

        /// <summary>
        /// Tests whether the CleanParameterArray method retains all parameters when no properties are null.
        /// </summary>
        [TestMethod]
        public void TestCleanParameterArrayRetainsAllWhenNoNulls()
        {
            object model = new
            {
                Name = "Test",
                Value = "Present"
            };
            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", "Test"),
                new SqlParameter("@Value", "Present")
            };

            SqlParameter[] actual = ConfigurationFunction.CleanParameterArray(model, parameters);

            Assert.AreEqual(2, actual.Length);
        }

        /// <summary>
        /// Tests whether the CleanParameterArray method removes all parameters when all properties are null.
        /// </summary>
        [TestMethod]
        public void TestCleanParameterArrayRemovesAllWhenAllNull()
        {
            object model = new
            {
                Name = (string)null,
                Value = (string)null
            };
            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", "Test"),
                new SqlParameter("@Value", "SomeValue")
            };

            SqlParameter[] actual = ConfigurationFunction.CleanParameterArray(model, parameters);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the CleanParameterArray method returns an empty array when given an empty model and parameter list.
        /// </summary>
        [TestMethod]
        public void TestCleanParameterArrayEmptyModelAndParameters()
        {
            object model = new { };
            SqlParameter[] parameters = Array.Empty<SqlParameter>();

            SqlParameter[] actual = ConfigurationFunction.CleanParameterArray(model, parameters);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the CleanParameterArray method correctly handles a model with a mix of null and non-null properties.
        /// </summary>
        [TestMethod]
        public void TestCleanParameterArrayMixedNullAndNonNull()
        {
            object model = new
            {
                First = "A",
                Second = (string)null,
                Third = "C"
            };
            SqlParameter[] parameters =
            {
                new SqlParameter("@First", "A"),
                new SqlParameter("@Second", "B"),
                new SqlParameter("@Third", "C")
            };

            SqlParameter[] actual = ConfigurationFunction.CleanParameterArray(model, parameters);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("@First", actual[0].ParameterName);
            Assert.AreEqual("@Third", actual[1].ParameterName);
        }

        #endregion

        #region CleanSQL Tests

        /// <summary>
        /// Tests whether the CleanSQL method removes SQL lines for null properties.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLRemovesLinesForNullProperties()
        {
            object model = new
            {
                Name = "Test",
                Value = (string)null
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Name = @Name",
                "and Value = @Value"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            string expected = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Name = @Name"
            });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method retains all lines when no properties are null.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLRetainsAllLinesWhenNoNulls()
        {
            object model = new
            {
                Name = "Test",
                Value = "Present"
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Name = @Name",
                "and Value = @Value"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            Assert.AreEqual(sql, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method replaces the first and with where when the where line is removed.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLReplacesAndWithWhereWhenWhereLineRemoved()
        {
            object model = new
            {
                Name = (string)null,
                Value = "Present"
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Name = @Name",
                "and Value = @Value"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            string expected = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Value = @Value"
            });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method leaves SQL unchanged when given an empty model.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLEmptyModelLeavesUnchanged()
        {
            object model = new { };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where Id = 1"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            Assert.AreEqual(sql, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method correctly handles a model with a mix of null and non-null properties.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLMixedNullAndNonNull()
        {
            object model = new
            {
                First = "A",
                Second = (string)null,
                Third = "C"
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where First = @First",
                "and Second = @Second",
                "and Third = @Third"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            string expected = string.Join(Environment.NewLine, new[]
            {
                "select *",
                "from Table with (nolock)",
                "where First = @First",
                "and Third = @Third"
            });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method removes the trailing comma from the last SET line when a property is null.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLRemovesTrailingCommaFromLastSetLine()
        {
            object model = new
            {
                Name = "Test",
                Value = (string)null
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "update Table",
                "set [Name] = @Name,",
                "[Value] = @Value",
                "where Id = @Id"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            string expected = string.Join(Environment.NewLine, new[]
            {
                "update Table",
                "set [Name] = @Name",
                "where Id = @Id"
            });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the CleanSQL method removes the trailing comma from the last SET line when the last property is null.
        /// </summary>
        [TestMethod]
        public void TestCleanSQLRemovesTrailingCommaWhenLastPropertyNull()
        {
            object model = new
            {
                Name = "Test",
                Type = "String",
                Value = (string)null
            };
            string sql = string.Join(Environment.NewLine, new[]
            {
                "update Table set",
                "[Name] = @Name,",
                "[Type] = @Type,",
                "[Value] = @Value",
                "where Id = @Id"
            });

            string actual = ConfigurationFunction.CleanSQL(model, sql);

            string expected = string.Join(Environment.NewLine, new[]
            {
                "update Table set",
                "[Name] = @Name,",
                "[Type] = @Type",
                "where Id = @Id"
            });

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
