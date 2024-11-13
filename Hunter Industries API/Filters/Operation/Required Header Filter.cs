using HunterIndustriesAPI.Controllers;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Operation
{
    /// <summary>
    /// </summary>
    public class RequiredHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// Adds the required authorisation to the Swagger UI depending on the controller.
        /// </summary>
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(TokenController))
            {
                operation.parameters.Add(new Parameter
                {
                    name = "authorization",
                    @in = "header",
                    required = true,
                    type = "string",
                    description = "Authorization header in the format: Basic {encodedCredentials}."
                });
            }
        }
    }
}