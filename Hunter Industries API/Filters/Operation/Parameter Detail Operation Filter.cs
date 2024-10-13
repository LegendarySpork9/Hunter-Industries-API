﻿using HunterIndustriesAPI.Controllers;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Operation
{
    /// <summary>
    /// </summary>
    public class ParameterDetailOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Alters the details of the parameters on the Swagger UI depending on the controller.
        /// </summary>
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(TokenController))
            {
                foreach(Parameter param in operation.parameters)
                {
                    if (param.name == "request")
                    {
                        param.name = "phrase";
                        param.type = "string";
                        param.schema = null;
                    }
                }
            }
        }
    }
}