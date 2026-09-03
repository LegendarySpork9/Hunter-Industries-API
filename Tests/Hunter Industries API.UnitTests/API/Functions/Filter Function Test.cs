// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Functions;
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HunterIndustriesAPI.UnitTests.API.Functions
{
    [TestClass]
    public class FilterFunctionTest
    {

        /// <summary>
        /// Checks whether a valid tag filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateTagFilter()
        {
            FilterModel request = new()
            {
                Name = "Language",
                Type = "tag",
                Values = "C#,Python"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a filter with no name returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateMissingName()
        {
            FilterModel request = new()
            {
                Type = "tag",
                Values = "C#,Python"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a tag filter with no type defaults to tag and returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateTagFilterWithNullType()
        {
            FilterModel request = new()
            {
                Name = "Language",
                Values = "C#,Python"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a tag filter with an operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateTagFilterWithOperator()
        {
            FilterModel request = new()
            {
                Name = "Language",
                Type = "tag",
                Operator = "equals"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a valid numeric filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateNumericFilter()
        {
            FilterModel request = new()
            {
                Name = "Bugs",
                Type = "numeric",
                Operator = "greater than",
                Path = "gitHubInformation.issueBreakdown.bugs",
                Values = "5"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a numeric filter with an invalid operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateNumericInvalidOperator()
        {
            FilterModel request = new()
            {
                Name = "Bugs",
                Type = "numeric",
                Operator = "contains",
                Path = "gitHubInformation.issueBreakdown.bugs",
                Values = "5"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a valid text filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateTextFilter()
        {
            FilterModel request = new()
            {
                Name = "Name Search",
                Type = "text",
                Operator = "contains",
                Path = "name",
                Values = "API"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a text filter with an invalid operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateTextInvalidOperator()
        {
            FilterModel request = new()
            {
                Name = "Name Search",
                Type = "text",
                Operator = "greater than",
                Path = "name",
                Values = "API"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a valid boolean filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateBooleanFilter()
        {
            FilterModel request = new()
            {
                Name = "Is Deleted",
                Type = "boolean",
                Operator = "is true",
                Path = "isDeleted"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a boolean filter with an invalid operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateBooleanInvalidOperator()
        {
            FilterModel request = new()
            {
                Name = "Is Deleted",
                Type = "boolean",
                Operator = "equals",
                Path = "isDeleted"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a valid null filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateNullFilter()
        {
            FilterModel request = new()
            {
                Name = "Has LLM Usage",
                Type = "null",
                Operator = "has value",
                Path = "llmUsage"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a null filter with an invalid operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateNullInvalidOperator()
        {
            FilterModel request = new()
            {
                Name = "Has LLM Usage",
                Type = "null",
                Operator = "equals",
                Path = "llmUsage"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a valid comparison filter returns no error.
        /// </summary>
        [TestMethod]
        public void TestValidateComparisonFilter()
        {
            FilterModel request = new()
            {
                Name = "More Bugs than Features",
                Type = "comparison",
                Operator = "greater than",
                Path = "gitHubInformation.issueBreakdown.bugs",
                Values = "gitHubInformation.issueBreakdown.newFeatures"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Checks whether a comparison filter with an invalid operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateComparisonInvalidOperator()
        {
            FilterModel request = new()
            {
                Name = "More Bugs than Features",
                Type = "comparison",
                Operator = "contains",
                Path = "gitHubInformation.issueBreakdown.bugs",
                Values = "gitHubInformation.issueBreakdown.newFeatures"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a non-tag filter without a path returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateMissingPath()
        {
            FilterModel request = new()
            {
                Name = "Bugs",
                Type = "numeric",
                Operator = "greater than",
                Values = "5"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether a non-tag filter without an operator returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateMissingOperator()
        {
            FilterModel request = new()
            {
                Name = "Is Deleted",
                Type = "boolean",
                Path = "isDeleted"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Checks whether an invalid filter type returns an error.
        /// </summary>
        [TestMethod]
        public void TestValidateInvalidType()
        {
            FilterModel request = new()
            {
                Name = "Invalid",
                Type = "unknown",
                Operator = "equals",
                Path = "name"
            };

            string result = FilterFunction.ValidateFilterType(request);

            Assert.IsNotNull(result);
        }

    }
}
