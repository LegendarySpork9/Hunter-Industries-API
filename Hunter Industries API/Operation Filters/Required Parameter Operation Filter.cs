// Copyright © - unpublished - Toby Hunter
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HunterIndustriesAPI.Services
{
    public class RequiredParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (actionAttributes.OfType<MakeFiltersRequiredAttribute>().Any())
            {
                foreach (var parameter in operation.Parameters)
                {
                    if (parameter != null)
                    {
                        parameter.Required = true;
                    }
                }
            }
        }
    }

    public class MakeFiltersRequiredAttribute : Attribute { }
}
