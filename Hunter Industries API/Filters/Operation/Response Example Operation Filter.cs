using HunterIndustriesAPI.Controllers.Assistant;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace HunterIndustriesAPI.Filters.Operation
{
    /// <summary>
    /// </summary>
    public class ResponseExampleOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Adds the response examples to the Swagger UI.
        /// </summary>
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            Response existingResponse;

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ConfigController))
            {
                if (operation.responses.TryGetValue("200", out existingResponse))
                {
                    existingResponse.schema = new Schema
                    {
                        type = "object",
                        properties = new Dictionary<string, Schema>
                        {
                            {
                                "Information", new Schema
                                {
                                    type = "string",
                                    example = "A config with the name and/or ID already exists."
                                }
                            }
                        }
                    };
                }
            }

            if (operation.responses.TryGetValue("400", out existingResponse))
            {
                existingResponse.schema = new Schema
                {
                    type = "object",
                    properties = new Dictionary<string, Schema>
                    {
                        {
                            "Error", new Schema
                            {
                                type = "string"
                            }
                        }
                    }
                };
            }

            if (operation.responses.TryGetValue("401", out existingResponse))
            {
                existingResponse.schema = new Schema
                {
                    type = "object",
                    properties = new Dictionary<string, Schema>
                    {
                        {
                            "Error", new Schema
                            {
                                type = "string"
                            }
                        }
                    }
                };
            }

            if (operation.responses.TryGetValue("404", out existingResponse))
            {
                existingResponse.schema = new Schema
                {
                    type = "object",
                    properties = new Dictionary<string, Schema>
                    {
                        {
                            "Error", new Schema
                            {
                                type = "string"
                            }
                        }
                    }
                };
            }

            if (operation.responses.TryGetValue("500", out existingResponse))
            {
                existingResponse.schema = new Schema
                {
                    type = "object",
                    properties = new Dictionary<string, Schema>
                    {
                        {
                            "Error", new Schema
                            {
                                type = "string"
                            }
                        }
                    }
                };
            }
        }
    }
}