// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.Services
{
    [TestClass]
    public class ModelValidationServiceTest
    {
        private ModelValidationService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new ModelValidationService();
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when given a null model.
        /// </summary>
        [TestMethod]
        public void TestIsValidNullModel()
        {
            bool actual = _service.IsValid(null);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when all string properties are valid and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidAllPropertiesValid()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "Test", Value = "Data" };

            bool actual = _service.IsValid(model, allRequired: true);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when one property is valid and one is null with allRequired false.
        /// </summary>
        [TestMethod]
        public void TestIsValidOnePropertyValidNotAllRequired()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "Test", Value = null };

            bool actual = _service.IsValid(model, allRequired: false);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when one property is null and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidOnePropertyNullAllRequired()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "Test", Value = null };

            bool actual = _service.IsValid(model, allRequired: true);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when a null property is ignored with allRequired true.
        /// </summary>
        [TestMethod]
        public void TestIsValidIgnoreProperties()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "Test", Value = null };

            bool actual = _service.IsValid(model, allRequired: true, ignoreProperties: new string[] { "Value" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when a null property is in propertiesAllowedNulls with allRequired true.
        /// </summary>
        [TestMethod]
        public void TestIsValidPropertiesAllowedNulls()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "Test", Value = null };

            bool actual = _service.IsValid(model, allRequired: true, propertiesAllowedNulls: new string[] { "Value" });

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has an int property.
        /// </summary>
        [TestMethod]
        public void TestIsValidIntProperty()
        {
            TestModelInt model = new TestModelInt { Count = 5 };

            bool actual = _service.IsValid(model);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has a bool property.
        /// </summary>
        [TestMethod]
        public void TestIsValidBoolProperty()
        {
            TestModelBool model = new TestModelBool { IsActive = true };

            bool actual = _service.IsValid(model);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when a string property is empty and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidEmptyStringAllRequired()
        {
            TestModelTwoStrings model = new TestModelTwoStrings { Name = "", Value = "" };

            bool actual = _service.IsValid(model, allRequired: true);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has a valid List property.
        /// </summary>
        [TestMethod]
        public void TestIsValidListProperty()
        {
            TestModelList model = new TestModelList { Items = new List<string> { "Item1", "Item2" } };

            bool actual = _service.IsValid(model);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test model with two string properties.
        /// </summary>
        private class TestModelTwoStrings
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// A test model with an int property.
        /// </summary>
        private class TestModelInt
        {
            public int Count { get; set; }
        }

        /// <summary>
        /// A test model with a bool property.
        /// </summary>
        private class TestModelBool
        {
            public bool IsActive { get; set; }
        }

        /// <summary>
        /// A test model with a List property.
        /// </summary>
        private class TestModelList
        {
            public List<string> Items { get; set; }
        }
    }
}
