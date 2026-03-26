// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HunterIndustriesAPI.Tests.Functions
{
    [TestClass]
    public class ConfigurationFunctionTest
    {
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
            List<SqlParameter> parameters = new List<SqlParameter>
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
            List<SqlParameter> parameters = new List<SqlParameter>
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
            List<SqlParameter> parameters = new List<SqlParameter>
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
            List<SqlParameter> parameters = new List<SqlParameter>();

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
            List<SqlParameter> parameters = new List<SqlParameter>
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
    }
}
