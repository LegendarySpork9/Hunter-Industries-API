// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.Tests.ControlPanel.Converters
{
    [TestClass]
    public class ApplicationSettingConverterTest
    {
        #region GetDataType

        /// <summary>
        /// Tests whether the GetDataType method returns the input when given an unknown type.
        /// </summary>
        [TestMethod]
        public void TestGetDataType()
        {
            string expected = "Boolean";
            string actual = ApplicationSettingConverter.GetDataType("Boolean");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Byte (0 -> 255)" when given "Byte".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeByte()
        {
            string expected = "Byte (0 -> 255)";
            string actual = ApplicationSettingConverter.GetDataType("Byte");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Currancy" when given "Decimal".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeDecimal()
        {
            string expected = "Currancy";
            string actual = ApplicationSettingConverter.GetDataType("Decimal");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Math Integer" when given "Double".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeDouble()
        {
            string expected = "Math Integer";
            string actual = ApplicationSettingConverter.GetDataType("Double");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Integer (~32 KB)" when given "Int16".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeInt16()
        {
            string expected = "Integer (~32 KB)";
            string actual = ApplicationSettingConverter.GetDataType("Int16");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Integer (~2 GB)" when given "Int32".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeInt32()
        {
            string expected = "Integer (~2 GB)";
            string actual = ApplicationSettingConverter.GetDataType("Int32");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Integer (~8 EB)" when given "Int64".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeInt64()
        {
            string expected = "Integer (~8 EB)";
            string actual = ApplicationSettingConverter.GetDataType("Int64");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Integer (~139 BB)" when given "Int128".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeInt128()
        {
            string expected = "Integer (~139 BB)";
            string actual = ApplicationSettingConverter.GetDataType("Int128");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "SByte (-128 -> 127)" when given "SByte".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeSByte()
        {
            string expected = "SByte (-128 -> 127)";
            string actual = ApplicationSettingConverter.GetDataType("SByte");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Float" when given "Single".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeSingle()
        {
            string expected = "Float";
            string actual = ApplicationSettingConverter.GetDataType("Single");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Tests whether the GetDataType method returns "Duration" when given "TimeSpan".
        /// </summary>
        [TestMethod]
        public void TestGetDataTypeTimeSpan()
        {
            string expected = "Duration";
            string actual = ApplicationSettingConverter.GetDataType("TimeSpan");

            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
