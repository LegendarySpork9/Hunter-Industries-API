// Copyright © - Unpublished - Toby Hunter
using HunterIndustriesAPI.Models.Requests.Bodies.Portfolio;
using System;
using System.Linq;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public static class FilterFunction
    {
        /// <summary>
        /// Validates the filter type, operator, and path combination.
        /// </summary>
        public static string ValidateFilterType(FilterModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return "A name is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Type) || request.Type.Equals("tag", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(request.Operator))
                {
                    return "Tag filters must not have an operator.";
                }

                return null;
            }

            string[] validTypes = { "numeric", "text", "boolean", "null", "comparison" };

            if (!validTypes.Contains(request.Type.ToLower()))
            {
                return $"Invalid filter type '{request.Type}'. Valid types are: tag, numeric, text, boolean, null, comparison.";
            }

            if (string.IsNullOrWhiteSpace(request.Path))
            {
                return $"A path is required for '{request.Type}' filters.";
            }

            if (string.IsNullOrWhiteSpace(request.Operator))
            {
                return $"An operator is required for '{request.Type}' filters.";
            }

            switch (request.Type.ToLower())
            {
                case "numeric":
                    string[] numericOperators = { "equals", "not equals", "greater than", "less than", "between" };

                    if (!numericOperators.Contains(request.Operator.ToLower()))
                    {
                        return $"Invalid operator '{request.Operator}' for numeric filters. Valid operators are: {string.Join(", ", numericOperators)}.";
                    }

                    break;

                case "text":
                    string[] textOperators = { "contains", "not contains", "equals", "not equals", "starts with", "ends with" };

                    if (!textOperators.Contains(request.Operator.ToLower()))
                    {
                        return $"Invalid operator '{request.Operator}' for text filters. Valid operators are: {string.Join(", ", textOperators)}.";
                    }

                    break;

                case "boolean":
                    string[] booleanOperators = { "is true", "is false" };

                    if (!booleanOperators.Contains(request.Operator.ToLower()))
                    {
                        return $"Invalid operator '{request.Operator}' for boolean filters. Valid operators are: {string.Join(", ", booleanOperators)}.";
                    }

                    break;

                case "null":
                    string[] nullOperators = { "has value", "has no value" };

                    if (!nullOperators.Contains(request.Operator.ToLower()))
                    {
                        return $"Invalid operator '{request.Operator}' for null filters. Valid operators are: {string.Join(", ", nullOperators)}.";
                    }

                    break;

                case "comparison":
                    string[] comparisonOperators = { "equals", "not equals", "greater than", "less than" };

                    if (!comparisonOperators.Contains(request.Operator.ToLower()))
                    {
                        return $"Invalid operator '{request.Operator}' for comparison filters. Valid operators are: {string.Join(", ", comparisonOperators)}.";
                    }

                    break;
            }

            return null;
        }
    }
}
