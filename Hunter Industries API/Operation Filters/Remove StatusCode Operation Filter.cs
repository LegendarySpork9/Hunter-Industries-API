// Copyright © - unpublished - Toby Hunter
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HunterIndustriesAPI.Services
{
    public class RemoveStatusCodeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (actionAttributes.OfType<RemoveStatusCodeAttribute>().Any())
            {
                foreach (var response in operation.Responses)
                {
                    if (response.Value.Content.ContainsKey("application/json"))
                    {
                        response.Value.Content["application/json"].Schema.Properties.Remove("StatusCode");
                    }
                }
            }
        }
    }

    public class RemoveStatusCodeAttribute : Attribute { }
}
