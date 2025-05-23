using HunterIndustriesAPI.Controllers.Assistant;
using HunterIndustriesAPI.Controllers.ServerStatus;
using HunterIndustriesAPI.Controllers.User;
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
                if (operation.responses.TryGetValue("200", out existingResponse) && operation.operationId == "Config_Post")
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

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(UserController))
            {
                if (operation.responses.TryGetValue("200", out existingResponse) && operation.operationId == "User_Post")
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
                                    example = "A user with the username already exists."
                                }
                            }
                        }
                    };
                }

                if (operation.responses.TryGetValue("200", out existingResponse) && operation.operationId == "User_Delete")
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
                                    example = "The given user has been deleted."
                                }
                            }
                        }
                    };
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(UserSettingsController))
            {
                if (operation.responses.TryGetValue("200", out existingResponse) && operation.operationId == "UserSettings_Post")
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
                                    example = "A user setting with the setting name for the application already exists."
                                }
                            }
                        }
                    };
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ServerInformationController))
            {
                if (operation.responses.TryGetValue("200", out existingResponse) && operation.operationId == "ServerInformation_Post")
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
                                    example = "A server with the details provided already exists."
                                }
                            }
                        }
                    };
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ServerEventController))
            {
                if (operation.operationId == "ServerEvent_Post")
                {
                    operation.responses.Remove("200");
                }
            }

            if (apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(ServerAlertController))
            {
                if (operation.operationId == "ServerAlert_Post")
                {
                    operation.responses.Remove("200");
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