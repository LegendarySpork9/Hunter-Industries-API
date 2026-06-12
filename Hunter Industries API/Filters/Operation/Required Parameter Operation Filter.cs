// Copyright © - 11/06/2026 - Toby Hunter
using Swashbuckle.Swagger;
using System;
using System.Linq;

namespace HunterIndustriesAPI.Filters.Operation
{
    /// <summary>
    /// </summary>
    public class RequiredParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Makes a parameter required for the call on the Swagger UI.
        /// </summary>
        public void Apply(
            Swashbuckle.Swagger.Operation operation,
            SchemaRegistry schemaRegistry,
            System.Web.Http.Description.ApiDescription apiDescription)
        {
            bool hasRequiredAttribute = apiDescription.ActionDescriptor.GetCustomAttributes<MakeFiltersRequiredAttribute>()
                .Any();

            if (hasRequiredAttribute)
            {
                foreach (Parameter parameter in operation.parameters)
                {
                    if (parameter != null)
                    {
                        parameter.required = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    public class MakeFiltersRequiredAttribute : Attribute { }
}