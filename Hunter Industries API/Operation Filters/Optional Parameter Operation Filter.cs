// Copyright © - unpublished - Toby Hunter
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HunterIndustriesAPI.Services
{
    public class OptionalParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (actionAttributes.OfType<MakeFiltersOptionalAttribute>().Any())
            {
                foreach (var parameter in operation.Parameters)
                {
                    if (parameter != null)
                    {
                        parameter.Required = false;
                    }
                }
            }
        }
    }

    public class MakeFiltersOptionalAttribute : Attribute { }
}
