// Copyright © - 11/06/2026 - Toby Hunter
using HunterIndustriesAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HunterIndustriesAPI.Tests.API.Services
{
    [TestClass]
    public class ModelValidationServiceTest
    {
        private ModelValidationService _Service;

        [TestInitialize]
        public void Setup()
        {
            _Service = new();
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when given a null model.
        /// </summary>
        [TestMethod]
        public void TestIsValidNullModel()
        {
            bool actual = _Service.IsValid(null);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when all string properties are valid and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidAllPropertiesValid()
        {
            TestModelTwoStrings model = new() { Name = "Test", Value = "Data" };

            bool actual = _Service.IsValid(
                model,
                allRequired: true);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when one property is valid and one is null with allRequired false.
        /// </summary>
        [TestMethod]
        public void TestIsValidOnePropertyValidNotAllRequired()
        {
            TestModelTwoStrings model = new() { Name = "Test", Value = null };

            bool actual = _Service.IsValid(
                model,
                allRequired: false);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when one property is null and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidOnePropertyNullAllRequired()
        {
            TestModelTwoStrings model = new() { Name = "Test", Value = null };

            bool actual = _Service.IsValid(
                model,
                allRequired: true);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when a null property is ignored with allRequired true.
        /// </summary>
        [TestMethod]
        public void TestIsValidIgnoreProperties()
        {
            TestModelTwoStrings model = new() { Name = "Test", Value = null };

            bool actual = _Service.IsValid(
                model,
                allRequired: true,
                ignoreProperties: ["Value"]);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when a null property is in propertiesAllowedNulls with allRequired true.
        /// </summary>
        [TestMethod]
        public void TestIsValidPropertiesAllowedNulls()
        {
            TestModelTwoStrings model = new() { Name = "Test", Value = null };

            bool actual = _Service.IsValid(
                model,
                allRequired: true,
                propertiesAllowedNulls: ["Value"]);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has an int property.
        /// </summary>
        [TestMethod]
        public void TestIsValidIntProperty()
        {
            TestModelInt model = new() { Count = 5 };

            bool actual = _Service.IsValid(model);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has a bool property.
        /// </summary>
        [TestMethod]
        public void TestIsValidBoolProperty()
        {
            TestModelBool model = new() { IsActive = true };

            bool actual = _Service.IsValid(model);

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns false when a string property is empty and allRequired is true.
        /// </summary>
        [TestMethod]
        public void TestIsValidEmptyStringAllRequired()
        {
            TestModelTwoStrings model = new() { Name = "", Value = "" };

            bool actual = _Service.IsValid(
                model,
                allRequired: true);

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Checks whether the IsValid method returns true when the model has a valid List property.
        /// </summary>
        [TestMethod]
        public void TestIsValidListProperty()
        {
            TestModelList model = new() { Items = ["Item1", "Item2"] };

            bool actual = _Service.IsValid(model);

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
