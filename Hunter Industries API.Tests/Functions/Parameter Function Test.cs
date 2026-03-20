// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Hunter_Industries_API.Tests.Functions
{
    [TestClass]
    public class ParameterFunctionTest
    {
        #region FormatParameters (string, object)

        /// <summary>
        /// Tests whether the FormatParameters method returns an empty array when given no arguments.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersEmpty()
        {
            string[] actual = ParameterFunction.FormatParameters(parameters: (string)null, model: null);

            Assert.AreEqual(0, actual.Length);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct array when given a SQL format string.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersString()
        {
            string[] actual = ParameterFunction.FormatParameters("\"value1\",\"value2\"");

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("value1", actual[0]);
            Assert.AreEqual("value2", actual[1]);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct array when given a model.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersModel()
        {
            object model = new { Name = "Test", Value = 1 };

            string[] actual = ParameterFunction.FormatParameters(null, model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("Test", actual[0]);
            Assert.AreEqual("1", actual[1]);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns an empty string for null model properties.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersModelNullProperty()
        {
            object model = new { Name = (string)null };

            string[] actual = ParameterFunction.FormatParameters(null, model);

            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual("", actual[0]);
        }

        /// <summary>
        /// Tests whether the FormatParameters method handles a model with a list property.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersModelList()
        {
            object model = new { Items = new List<string> { "a", "b" } };

            string[] actual = ParameterFunction.FormatParameters(null, model);

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("a", actual[0]);
            Assert.AreEqual("b", actual[1]);
        }

        #endregion

        #region FormatParameters (string[], bool)

        /// <summary>
        /// Tests whether the FormatParameters method returns null when given a null array.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersArrayNull()
        {
            string actual = ParameterFunction.FormatParameters((string[])null, false);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct log format when given a string array.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersArrayLog()
        {
            string expected = "\"value1\", \"value2\"";

            string actual = ParameterFunction.FormatParameters(new string[] { "value1", "value2" }, false);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct SQL format when given a string array.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersArraySQL()
        {
            string expected = "\"value1\",\"value2\"";

            string actual = ParameterFunction.FormatParameters(new string[] { "value1", "value2" }, true);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method replaces empty values with null in log format.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersArrayNullValue()
        {
            string expected = "\"value1\", \"null\"";

            string actual = ParameterFunction.FormatParameters(new string[] { "value1", "" }, false);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method replaces empty values with null in SQL format.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersArrayNullValueSQL()
        {
            string expected = "\"value1\",\"null\"";

            string actual = ParameterFunction.FormatParameters(new string[] { "value1", "" }, true);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region FormatParameters (object)

        /// <summary>
        /// Tests whether the FormatParameters method returns an empty string when given a null model.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectNull()
        {
            string actual = ParameterFunction.FormatParameters((object)null);

            Assert.AreEqual(string.Empty, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct log format when given a model.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObject()
        {
            string expected = "\"Test\", \"1\"";
            object model = new { Name = "Test", Value = 1 };

            string actual = ParameterFunction.FormatParameters(model);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns null for null properties when given a model.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectNullProperty()
        {
            string expected = "\"null\"";
            object model = new { Name = (string)null };

            string actual = ParameterFunction.FormatParameters(model);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method handles a model with a list property.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectList()
        {
            string expected = "\"a\", \"b\"";
            object model = new { Items = new List<string> { "a", "b" } };

            string actual = ParameterFunction.FormatParameters(model);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region FormatParameters (object, bool)

        /// <summary>
        /// Tests whether the FormatParameters method returns an empty string when given a null list object.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersListNull()
        {
            string actual = ParameterFunction.FormatParameters((object)null, true);

            Assert.AreEqual(string.Empty, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct format when given a key value pair list.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersListKeyPair()
        {
            string expected = "\"User\", \"Assistant API\"";
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Add", "User"),
                new KeyValuePair<string, string>("Remove", "Assistant API")
            };

            string actual = ParameterFunction.FormatParameters(list, true);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct format when given a regular list.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersListRegular()
        {
            string expected = "\"User\", \"Assistant API\"";
            List<string> list = new List<string> { "User", "Assistant API" };

            string actual = ParameterFunction.FormatParameters(list, false);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region FormatParameters (List<object>, bool)

        /// <summary>
        /// Tests whether the FormatParameters method returns null when given a null object list.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectListNull()
        {
            string actual = ParameterFunction.FormatParameters((List<object>)null);

            Assert.IsNull(actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct format when given an object list.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectListValues()
        {
            string expected = "value1, value2";

            string actual = ParameterFunction.FormatParameters(new List<object> { "value1", "value2" });

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the FormatParameters method returns the correct audit format when given an object list.
        /// </summary>
        [TestMethod]
        public void TestFormatParametersObjectListAudit()
        {
            string expected = "\"value1\",\"value2\"";

            string actual = ParameterFunction.FormatParameters(new List<object> { "value1", "value2" }, true);

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
