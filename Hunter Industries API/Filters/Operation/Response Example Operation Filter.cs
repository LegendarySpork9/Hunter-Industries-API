using HunterIndustriesAPI.Controllers;
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

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(TokenController))
            {
                if (operation.responses.TryGetValue("200", out existingResponse))
                {
                    existingResponse.schema = new Schema
                    {
                        type = "object",
                        properties = new Dictionary<string, Schema>
                        {
                            {
                                "type", new Schema
                                {
                                    type = "string",
                                    example = "Bearer"
                                }
                            },
                            {
                                "token", new Schema
                                {
                                    type = "string",
                                    example = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
                                }
                            },
                            {
                                "expiresIn", new Schema
                                {
                                    type = "int",
                                    example = 900
                                }
                            },
                            {
                                "info", new Schema
                                {
                                    type = "object",
                                    properties = new Dictionary<string, Schema>
                                    {
                                        {
                                            "applicationName", new Schema
                                            {
                                                type = "string",
                                                example = "Virtual Assistant"
                                            }
                                        },
                                        {
                                            "scope", new Schema
                                            {
                                                type = "array",
                                                items = new Schema
                                                {
                                                    type = "string",
                                                    example = "Assistant API"
                                                }
                                            }
                                        },
                                        {
                                            "issued", new Schema
                                            {
                                                type = "datetime",
                                                example = "2024-10-13T19:01:02.3282067Z"
                                            }
                                        },
                                        {
                                            "expires", new Schema
                                            {
                                                type = "datetime",
                                                example = "2024-10-13T19:16:02.3282067Z"
                                            }
                                        }
                                    }
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