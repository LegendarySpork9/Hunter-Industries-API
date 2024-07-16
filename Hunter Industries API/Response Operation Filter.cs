// Copyright © - unpublished - Toby Hunter
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HunterIndustriesAPI
{
    internal class ResponseOperationFilter : IOperationFilter
    {
        // Removes the default bad request schema.
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request" });
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}